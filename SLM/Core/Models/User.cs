using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? StudentId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}