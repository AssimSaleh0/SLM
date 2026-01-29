using SLM.Core.Enums;

namespace SLM.Core.DTOs.Notifications
{
    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public object? Data { get; set; }
    }
}