using SLM.Core.DTOs.Notifications;
using SLM.Core.Enums;
using SLM.Core.Interfaces;
using SLM.Core.Models;
using System.Text.Json;

namespace SLM.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                ActionUrl = request.ActionUrl,
                Data = request.Data != null ? JsonSerializer.Serialize(request.Data) : null
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var preferences = await _unitOfWork.Repository<NotificationPreference>()
                .FirstOrDefaultAsync(p => p.UserId == request.UserId);

            if (preferences != null)
            {
                if (ShouldSendNotification(preferences, request.Type))
                {
                    if (preferences.PushEnabled)
                    {
                        await SendPushNotificationAsync(request.UserId, request.Title, request.Message);
                    }

                    if (preferences.EmailEnabled)
                    {
                        await SendEmailNotificationAsync(request.UserId, request.Title, request.Message);
                    }
                }
            }

            return MapToDto(notification);
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var query = unreadOnly
                ? await _unitOfWork.Repository<Notification>()
                    .FindAsync(n => n.UserId == userId && !n.IsRead)
                : await _unitOfWork.Repository<Notification>()
                    .FindAsync(n => n.UserId == userId);

            return query.OrderByDescending(n => n.CreatedAt)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            return notification != null ? MapToDto(notification) : null;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Notification>().UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.UserId == userId && !n.IsRead);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<Notification>().UpdateRangeAsync(unreadNotifications);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Notification>().DeleteAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _unitOfWork.Repository<Notification>()
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<NotificationPreferenceDto?> GetPreferencesAsync(int userId)
        {
            var preferences = await _unitOfWork.Repository<NotificationPreference>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preferences == null)
            {
                return null;
            }

            return new NotificationPreferenceDto
            {
                PushEnabled = preferences.PushEnabled,
                EmailEnabled = preferences.EmailEnabled,
                SmsEnabled = preferences.SmsEnabled,
                BudgetAlerts = preferences.BudgetAlerts,
                DeadlineReminders = preferences.DeadlineReminders,
                JobAlerts = preferences.JobAlerts,
                EventReminders = preferences.EventReminders,
                BillReminders = preferences.BillReminders
            };
        }

        public async Task<bool> UpdatePreferencesAsync(int userId, UpdateNotificationPreferenceRequest request)
        {
            var preferences = await _unitOfWork.Repository<NotificationPreference>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preferences == null)
            {
                return false;
            }

            if (request.PushEnabled.HasValue)
                preferences.PushEnabled = request.PushEnabled.Value;
            if (request.EmailEnabled.HasValue)
                preferences.EmailEnabled = request.EmailEnabled.Value;
            if (request.SmsEnabled.HasValue)
                preferences.SmsEnabled = request.SmsEnabled.Value;
            if (request.BudgetAlerts.HasValue)
                preferences.BudgetAlerts = request.BudgetAlerts.Value;
            if (request.DeadlineReminders.HasValue)
                preferences.DeadlineReminders = request.DeadlineReminders.Value;
            if (request.JobAlerts.HasValue)
                preferences.JobAlerts = request.JobAlerts.Value;
            if (request.EventReminders.HasValue)
                preferences.EventReminders = request.EventReminders.Value;
            if (request.BillReminders.HasValue)
                preferences.BillReminders = request.BillReminders.Value;

            await _unitOfWork.Repository<NotificationPreference>().UpdateAsync(preferences);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task SendPushNotificationAsync(int userId, string title, string message)
        {
            await Task.CompletedTask;
        }

        public async Task SendEmailNotificationAsync(int userId, string subject, string body)
        {
            await Task.CompletedTask;
        }

        private bool ShouldSendNotification(NotificationPreference preferences, NotificationType type)
        {
            return type switch
            {
                NotificationType.BudgetOverage => preferences.BudgetAlerts,
                NotificationType.Deadline => preferences.DeadlineReminders,
                NotificationType.JobOffer => preferences.JobAlerts,
                NotificationType.EventReminder => preferences.EventReminders,
                NotificationType.BillPayment => preferences.BillReminders,
                NotificationType.General => true,
                _ => true
            };
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                ActionUrl = notification.ActionUrl
            };
        }
    }
}