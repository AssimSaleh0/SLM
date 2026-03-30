using SLM.Core.DTOs.Financial;

namespace SLM.Core.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetCategoryDto> CreateBudgetCategoryAsync(int userId, CreateBudgetCategoryRequest request);
        Task<BudgetCategoryDto?> GetBudgetCategoryByIdAsync(int categoryId, int userId);
        Task<List<BudgetCategoryDto>> GetUserBudgetCategoriesAsync(int userId);
        Task<BudgetSummaryDto> GetBudgetSummaryAsync(int userId);
        Task<bool> UpdateBudgetCategoryAsync(int categoryId, int userId, UpdateBudgetCategoryRequest request);
        Task<bool> DeleteBudgetCategoryAsync(int categoryId, int userId);
        Task<bool> UpdateSpentAmountAsync(int categoryId, decimal amount);
        Task<List<BudgetCategoryDto>> GetOverBudgetCategoriesAsync(int userId);
    }
}