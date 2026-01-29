namespace SLM.Core.DTOs.Notifications
{
    public class NotificationPreferenceDto
    {
        public bool PushEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool SmsEnabled { get; set; }
        public bool BudgetAlerts { get; set; }
        public bool DeadlineReminders { get; set; }
        public bool JobAlerts { get; set; }
        public bool EventReminders { get; set; }
        public bool BillReminders { get; set; }
    }
}