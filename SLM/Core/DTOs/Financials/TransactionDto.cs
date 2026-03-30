using SLM.Core.Enums;

namespace SLM.Core.DTOs.Financial
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Merchant { get; set; }
        public string? Notes { get; set; }
        public string? CategoryName { get; set; }
        public int? BudgetCategoryId { get; set; }
        public int? ReceiptId { get; set; }
    }

    public class CreateTransactionRequest
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Merchant { get; set; }
        public string? Notes { get; set; }
        public int? BudgetCategoryId { get; set; }
    }

    public class UpdateTransactionRequest
    {
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Merchant { get; set; }
        public string? Notes { get; set; }
        public int? BudgetCategoryId { get; set; }
    }

    public class TransactionReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
    }
}