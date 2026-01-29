using SLM.Core.DTOs.Notifications;

namespace SLM.Core.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<NotificationDto?> GetNotificationByIdAsync(int notificationId, int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<NotificationPreferenceDto?> GetPreferencesAsync(int userId);
        Task<bool> UpdatePreferencesAsync(int userId, UpdateNotificationPreferenceRequest request);
        Task SendPushNotificationAsync(int userId, string title, string message);
        Task SendEmailNotificationAsync(int userId, string subject, string body);
    }
}