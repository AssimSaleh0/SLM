namespace SLM.Core.Models
{
    public class NotificationPreference : BaseEntity
    {
        public int UserId { get; set; }
        public bool PushEnabled { get; set; } = true;
        public bool EmailEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
        public bool BudgetAlerts { get; set; } = true;
        public bool DeadlineReminders { get; set; } = true;
        public bool JobAlerts { get; set; } = true;
        public bool EventReminders { get; set; } = true;
        public bool BillReminders { get; set; } = true;

        // Navigation properties
        public User User { get; set; } = null!;
    }
}