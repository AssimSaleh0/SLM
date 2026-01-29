using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLM.Core.DTOs.Auth;
using SLM.Core.Interfaces;
using System.Security.Claims;

namespace SLM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _authService.ChangePasswordAsync(userId, request);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> RequestPasswordReset([FromBody] ResetPasswordRequest request)
        {
            await _authService.RequestPasswordResetAsync(request);
            return Ok(new { message = "Password reset instructions sent to email" });
        }

        [HttpPost("reset-password/confirm")]
        public async Task<ActionResult> ConfirmPasswordReset([FromBody] ConfirmResetPasswordRequest request)
        {
            try
            {
                await _authService.ConfirmPasswordResetAsync(request);
                return Ok(new { message = "Password reset successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _authService.ConfirmEmailAsync(userId, token);
                return Ok(new { message = "Email confirmed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resend-confirmation")]
        public async Task<ActionResult> ResendEmailConfirmation()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var token = await _authService.GenerateEmailConfirmationTokenAsync(userId);
                return Ok(new { message = "Confirmation email sent", token });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}