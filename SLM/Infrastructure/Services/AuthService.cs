using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SLM.Core.DTOs.Auth;
using SLM.Core.Interfaces;
using SLM.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SLM.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _resetTokens;
        private readonly Dictionary<string, string> _emailTokens;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _resetTokens = new Dictionary<string, string>();
            _emailTokens = new Dictionary<string, string>();
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var existingStudentId = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.StudentId == request.StudentId);

            if (existingStudentId != null)
            {
                throw new InvalidOperationException("User with this student ID already exists");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                StudentId = request.StudentId,
                DateOfBirth = request.DateOfBirth,
                EmailConfirmed = false
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var studentRole = await _unitOfWork.Repository<Role>()
                .FirstOrDefaultAsync(r => r.Name == "Student");

            if (studentRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = studentRole.Id
                };

                await _unitOfWork.Repository<UserRole>().AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
            }

            var notificationPreference = new NotificationPreference
            {
                UserId = user.Id
            };

            await _unitOfWork.Repository<NotificationPreference>().AddAsync(notificationPreference);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user, new List<string> { "Student" });

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Roles = new List<string> { "Student" }
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userRoles = await _unitOfWork.Repository<UserRole>()
                .FindAsync(ur => ur.UserId == user.Id);

            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var roles = await _unitOfWork.Repository<Role>()
                .FindAsync(r => roleIds.Contains(r.Id));

            var roleNames = roles.Select(r => r.Name).ToList();

            var token = GenerateJwtToken(user, roleNames);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Roles = roleNames
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }

            user.PasswordHash = HashPassword(request.NewPassword);
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(ResetPasswordRequest request)
        {
            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return true;
            }

            var token = GenerateRandomToken();
            _resetTokens[token] = user.Email;

            return true;
        }

        public async Task<bool> ConfirmPasswordResetAsync(ConfirmResetPasswordRequest request)
        {
            if (!_resetTokens.TryGetValue(request.Token, out var email))
            {
                throw new InvalidOperationException("Invalid or expired reset token");
            }

            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            user.PasswordHash = HashPassword(request.NewPassword);
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _resetTokens.Remove(request.Token);

            return true;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(int userId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var token = GenerateRandomToken();
            _emailTokens[token] = user.Email;

            return token;
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!_emailTokens.TryGetValue(token, out var email) || email != user.Email)
            {
                throw new InvalidOperationException("Invalid or expired confirmation token");
            }

            user.EmailConfirmed = true;
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _emailTokens.Remove(token);

            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }

        private string GenerateJwtToken(User user, List<string> roles)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyHere123!@#"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "StudentERP",
                audience: _configuration["Jwt:Audience"] ?? "StudentERPUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRandomToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}