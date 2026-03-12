namespace SLM.Core.Models
{
    public class IncomeSource : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Scholarship, Part-time Job, Loan, etc.
        public decimal Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}