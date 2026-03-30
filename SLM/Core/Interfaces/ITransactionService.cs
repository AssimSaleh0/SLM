using SLM.Core.DTOs.Financial;

namespace SLM.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionRequest request);
        Task<TransactionDto?> GetTransactionByIdAsync(int transactionId, int userId);
        Task<List<TransactionDto>> GetUserTransactionsAsync(int userId);
        Task<List<TransactionDto>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<TransactionDto>> GetTransactionsByCategoryAsync(int userId, int categoryId);
        Task<TransactionReportDto> GetTransactionReportAsync(int userId, DateTime startDate, DateTime endDate);
        Task<bool> UpdateTransactionAsync(int transactionId, int userId, UpdateTransactionRequest request);
        Task<bool> DeleteTransactionAsync(int transactionId, int userId);
        Task<decimal> GetTotalIncomeAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalExpensesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    }
}