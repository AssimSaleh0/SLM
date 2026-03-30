using SLM.Core.DTOs.Financial;
using SLM.Core.Enums;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBudgetService _budgetService;

        public TransactionService(IUnitOfWork unitOfWork, IBudgetService budgetService)
        {
            _unitOfWork = unitOfWork;
            _budgetService = budgetService;
        }

        public async Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionRequest request)
        {
            if (request.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Transaction description is required.");

            var transaction = new Transaction
            {
                UserId = userId,
                Type = request.Type,
                Amount = request.Amount,
                Description = request.Description.Trim(),
                TransactionDate = request.TransactionDate,
                Merchant = request.Merchant?.Trim(),
                Notes = request.Notes?.Trim(),
                BudgetCategoryId = request.BudgetCategoryId
            };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Repository<Transaction>().AddAsync(transaction);

                if (transaction.Type == TransactionType.Expense && transaction.BudgetCategoryId.HasValue)
                {
                    await _budgetService.UpdateSpentAmountAsync(transaction.BudgetCategoryId.Value, transaction.Amount);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await MapToDtoAsync(transaction);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> UpdateTransactionAsync(int transactionId, int userId, UpdateTransactionRequest request)
        {
            var transaction = await _unitOfWork.Repository<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (transaction == null)
                return false;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Reverse old effect first
                if (transaction.Type == TransactionType.Expense && transaction.BudgetCategoryId.HasValue)
                {
                    await _budgetService.UpdateSpentAmountAsync(transaction.BudgetCategoryId.Value, -transaction.Amount);
                }

                if (request.Amount.HasValue)
                {
                    if (request.Amount.Value <= 0)
                        throw new ArgumentException("Transaction amount must be greater than zero.");

                    transaction.Amount = request.Amount.Value;
                }

                if (request.Description != null)
                {
                    if (string.IsNullOrWhiteSpace(request.Description))
                        throw new ArgumentException("Transaction description cannot be empty.");

                    transaction.Description = request.Description.Trim();
                }

                if (request.TransactionDate.HasValue)
                    transaction.TransactionDate = request.TransactionDate.Value;

                if (request.Merchant != null)
                    transaction.Merchant = request.Merchant.Trim();

                if (request.Notes != null)
                    transaction.Notes = request.Notes.Trim();

                //  category update only happens when a value is provided.
                if (request.BudgetCategoryId.HasValue)
                    transaction.BudgetCategoryId = request.BudgetCategoryId.Value;

                // Apply new effect
                if (transaction.Type == TransactionType.Expense && transaction.BudgetCategoryId.HasValue)
                {
                    await _budgetService.UpdateSpentAmountAsync(transaction.BudgetCategoryId.Value, transaction.Amount);
                }

                await _unitOfWork.Repository<Transaction>().UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId, int userId)
        {
            var transaction = await _unitOfWork.Repository<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (transaction == null)
                return false;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (transaction.Type == TransactionType.Expense && transaction.BudgetCategoryId.HasValue)
                {
                    await _budgetService.UpdateSpentAmountAsync(transaction.BudgetCategoryId.Value, -transaction.Amount);
                }

                await _unitOfWork.Repository<Transaction>().DeleteAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<TransactionDto> MapToDtoAsync(Transaction transaction)
        {
            string? categoryName = null;

            if (transaction.BudgetCategoryId.HasValue)
            {
                var category = await _unitOfWork.Repository<BudgetCategory>()
                    .GetByIdAsync(transaction.BudgetCategoryId.Value);

                categoryName = category?.Name;
            }

            return new TransactionDto
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Amount,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                Merchant = transaction.Merchant,
                Notes = transaction.Notes,
                BudgetCategoryId = transaction.BudgetCategoryId,
                ReceiptId = transaction.ReceiptId,
                CategoryName = categoryName
            };
        }
    }
}