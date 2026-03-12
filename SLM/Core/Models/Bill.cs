using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Bill : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public BillStatus Status { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringDayOfMonth { get; set; }
        public string? Description { get; set; }
        public DateTime? PaidDate { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}