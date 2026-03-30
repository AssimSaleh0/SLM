namespace SLM.Core.DTOs.Financial
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public decimal? Amount { get; set; }
        public string? Merchant { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Notes { get; set; }
        public string? CategoryName { get; set; }
        public int? BudgetCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UploadReceiptRequest
    {
        public int? BudgetCategoryId { get; set; }
        public decimal? Amount { get; set; }
        public string? Merchant { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateReceiptRequest
    {
        public int? BudgetCategoryId { get; set; }
        public decimal? Amount { get; set; }
        public string? Merchant { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Notes { get; set; }
    }
}
