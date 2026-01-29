using SLM.Core.DTOs.Auth;

namespace SLM.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> RequestPasswordResetAsync(ResetPasswordRequest request);
        Task<bool> ConfirmPasswordResetAsync(ConfirmResetPasswordRequest request);
        Task<bool> ConfirmEmailAsync(int userId, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
    }
}