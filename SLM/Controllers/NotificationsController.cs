using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLM.Core.DTOs.Notifications;
using SLM.Core.Interfaces;
using System.Security.Claims;

namespace SLM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications([FromQuery] bool unreadOnly = false)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notification = await _notificationService.GetNotificationByIdAsync(id, userId);

            if (notification == null)
            {
                return NotFound(new { message = "Notification not found" });
            }

            return Ok(notification);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPost("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _notificationService.MarkAsReadAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Notification not found" });
            }

            return Ok(new { message = "Notification marked as read" });
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "All notifications marked as read" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _notificationService.DeleteNotificationAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Notification not found" });
            }

            return Ok(new { message = "Notification deleted" });
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<NotificationPreferenceDto>> GetPreferences()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var preferences = await _notificationService.GetPreferencesAsync(userId);

            if (preferences == null)
            {
                return NotFound(new { message = "Preferences not found" });
            }

            return Ok(preferences);
        }

        [HttpPut("preferences")]
        public async Task<ActionResult> UpdatePreferences([FromBody] UpdateNotificationPreferenceRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _notificationService.UpdatePreferencesAsync(userId, request);

            if (!success)
            {
                return NotFound(new { message = "Preferences not found" });
            }

            return Ok(new { message = "Preferences updated successfully" });
        }
    }
}