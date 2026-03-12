namespace SLM.Core.DTOs.Financial
{
    public class BudgetCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal PercentageUsed { get; set; }
        public string? IconName { get; set; }
        public string? ColorCode { get; set; }
        public bool IsOverBudget { get; set; }
    }

    public class CreateBudgetCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal AllocatedAmount { get; set; }
        public string? IconName { get; set; }
        public string? ColorCode { get; set; }
    }

    public class UpdateBudgetCategoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? AllocatedAmount { get; set; }
        public string? IconName { get; set; }
        public string? ColorCode { get; set; }
    }

    public class BudgetSummaryDto
    {
        public decimal TotalAllocated { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal PercentageUsed { get; set; }
        public List<BudgetCategoryDto> Categories { get; set; } = new();
        public List<BudgetCategoryDto> OverBudgetCategories { get; set; } = new();
    }
}