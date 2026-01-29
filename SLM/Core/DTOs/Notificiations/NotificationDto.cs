using SLM.Core.Enums;

namespace SLM.Core.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
    }
}