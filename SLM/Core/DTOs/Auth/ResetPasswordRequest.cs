using System.ComponentModel.DataAnnotations;

namespace SLM.Core.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}