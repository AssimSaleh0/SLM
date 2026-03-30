namespace SLM.Core.DTOs.Financial
{
    public class IncomeSourceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
    }

    public class CreateIncomeSourceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateIncomeSourceRequest
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public bool? IsRecurring { get; set; }
        public string? Description { get; set; }
    }

    public class IncomeSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal MonthlyRecurringIncome { get; set; }
        public List<IncomeSourceDto> IncomeSources { get; set; } = new();
        public Dictionary<string, decimal> IncomeByType { get; set; } = new();
    }

    public class FinancialOverviewDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalBudgetAllocated { get; set; }
        public decimal TotalBudgetSpent { get; set; }
        public decimal UpcomingBills { get; set; }
        public int OverBudgetCategories { get; set; }
        public int OverdueBills { get; set; }
    }
}