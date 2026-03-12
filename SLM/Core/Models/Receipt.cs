Rnamespace SLM.Core.Models
{

	public class Receipt : BaseEntity
{
	public int UserId { get; set; }
	public int? BudgetCategoryId { get; set; }
	public string ImageUrl { get; set; } = string.Empty;
	public string? FileName { get; set; }
	public decimal? Amount { get; set; }
	public string? Merchant { get; set; }
	public DateTime? PurchaseDate { get; set; }
	public string? Notes { get; set; }

	// Navigation properties
	public User User { get; set; } = null!;
	public BudgetCategory? BudgetCategory { get; set; }
	public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
}