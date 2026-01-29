using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
        public string? Data { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}