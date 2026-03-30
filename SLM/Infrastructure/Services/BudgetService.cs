using SLM.Core.DTOs.Financial;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BudgetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BudgetCategoryDto> CreateBudgetCategoryAsync(int userId, CreateBudgetCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Budget category name is required.");

            if (request.AllocatedAmount < 0)
                throw new ArgumentException("Allocated amount cannot be negative.");

            var existingCategory = await _unitOfWork.Repository<BudgetCategory>()
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.Name.ToLower() == request.Name.Trim().ToLower());

            if (existingCategory != null)
                throw new InvalidOperationException("A budget category with this name already exists.");

            var category = new BudgetCategory
            {
                UserId = userId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                AllocatedAmount = request.AllocatedAmount,
                SpentAmount = 0,
                IconName = request.IconName,
                ColorCode = request.ColorCode
            };

            await _unitOfWork.Repository<BudgetCategory>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<BudgetCategoryDto?> GetBudgetCategoryByIdAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Repository<BudgetCategory>()
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            return category == null ? null : MapToDto(category);
        }

        public async Task<List<BudgetCategoryDto>> GetUserBudgetCategoriesAsync(int userId)
        {
            var categories = await _unitOfWork.Repository<BudgetCategory>()
                .FindAsync(c => c.UserId == userId);

            return categories
                .OrderBy(c => c.Name)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int userId)
        {
            var categories = await _unitOfWork.Repository<BudgetCategory>()
                .FindAsync(c => c.UserId == userId);

            var categoryDtos = categories
                .OrderBy(c => c.Name)
                .Select(MapToDto)
                .ToList();

            var totalAllocated = categories.Sum(c => c.AllocatedAmount);
            var totalSpent = categories.Sum(c => c.SpentAmount);
            var totalRemaining = totalAllocated - totalSpent;

            return new BudgetSummaryDto
            {
                TotalAllocated = totalAllocated,
                TotalSpent = totalSpent,
                TotalRemaining = totalRemaining,
                PercentageUsed = totalAllocated <= 0 ? 0 : Math.Round((totalSpent / totalAllocated) * 100, 2),
                Categories = categoryDtos,
                OverBudgetCategories = categoryDtos.Where(c => c.IsOverBudget).ToList()
            };
        }

        public async Task<bool> UpdateBudgetCategoryAsync(int categoryId, int userId, UpdateBudgetCategoryRequest request)
        {
            var category = await _unitOfWork.Repository<BudgetCategory>()
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var duplicate = await _unitOfWork.Repository<BudgetCategory>()
                    .FirstOrDefaultAsync(c =>
                        c.UserId == userId &&
                        c.Id != categoryId &&
                        c.Name.ToLower() == request.Name.Trim().ToLower());

                if (duplicate != null)
                    throw new InvalidOperationException("Another budget category with this name already exists.");

                category.Name = request.Name.Trim();
            }

            if (request.Description != null)
                category.Description = request.Description.Trim();

            if (request.AllocatedAmount.HasValue)
            {
                if (request.AllocatedAmount.Value < 0)
                    throw new ArgumentException("Allocated amount cannot be negative.");

                category.AllocatedAmount = request.AllocatedAmount.Value;
            }

            if (request.IconName != null)
                category.IconName = request.IconName;

            if (request.ColorCode != null)
                category.ColorCode = request.ColorCode;

            await _unitOfWork.Repository<BudgetCategory>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBudgetCategoryAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Repository<BudgetCategory>()
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
                return false;

            var hasTransactions = await _unitOfWork.Repository<Transaction>()
                .AnyAsync(t => t.BudgetCategoryId == categoryId && t.UserId == userId);

            var hasReceipts = await _unitOfWork.Repository<Receipt>()
                .AnyAsync(r => r.BudgetCategoryId == categoryId && r.UserId == userId);

            if (hasTransactions || hasReceipts)
                throw new InvalidOperationException("Cannot delete a budget category that is linked to transactions or receipts.");

            await _unitOfWork.Repository<BudgetCategory>().DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task UpdateSpentAmountAsync(int categoryId, decimal amount)
        {
            var category = await _unitOfWork.Repository<BudgetCategory>().GetByIdAsync(categoryId);

            if (category == null)
                throw new InvalidOperationException("Budget category not found.");

            category.SpentAmount += amount;

            if (category.SpentAmount < 0)
                category.SpentAmount = 0;

            await _unitOfWork.Repository<BudgetCategory>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<BudgetCategoryDto>> GetOverBudgetCategoriesAsync(int userId)
        {
            var categories = await _unitOfWork.Repository<BudgetCategory>()
                .FindAsync(c => c.UserId == userId && c.SpentAmount > c.AllocatedAmount);

            return categories
                .OrderByDescending(c => c.SpentAmount - c.AllocatedAmount)
                .Select(MapToDto)
                .ToList();
        }

        private static BudgetCategoryDto MapToDto(BudgetCategory category)
        {
            var remaining = category.AllocatedAmount - category.SpentAmount;
            var percentageUsed = category.AllocatedAmount <= 0
                ? 0
                : Math.Round((category.SpentAmount / category.AllocatedAmount) * 100, 2);

            return new BudgetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                AllocatedAmount = category.AllocatedAmount,
                SpentAmount = category.SpentAmount,
                RemainingAmount = remaining,
                PercentageUsed = percentageUsed,
                IconName = category.IconName,
                ColorCode = category.ColorCode,
                IsOverBudget = category.SpentAmount > category.AllocatedAmount
            };
        }
    }
}