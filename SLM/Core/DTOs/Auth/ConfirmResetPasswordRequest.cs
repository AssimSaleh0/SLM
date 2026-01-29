using System.ComponentModel.DataAnnotations;

namespace SLM.Core.DTOs.Auth
{
    public class ConfirmResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}