using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Transaction : BaseEntity
    {
        public int UserId { get; set; }
        public int? BudgetCategoryId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Merchant { get; set; }
        public string? Notes { get; set; }
        public int? ReceiptId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public BudgetCategory? BudgetCategory { get; set; }
        public Receipt? Receipt { get; set; }
    }
}