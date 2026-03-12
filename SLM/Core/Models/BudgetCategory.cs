namespace SLM.Core.Models
{
    public class BudgetCategory : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public string? IconName { get; set; }
        public string? ColorCode { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}