using SLM.Core.DTOs.Financial;

namespace SLM.Core.Interfaces
{
    public interface IIncomeService
    {
        Task<IncomeSourceDto> CreateIncomeSourceAsync(int userId, CreateIncomeSourceRequest request);
        Task<IncomeSourceDto?> GetIncomeSourceByIdAsync(int incomeId, int userId);
        Task<List<IncomeSourceDto>> GetUserIncomeSourcesAsync(int userId);
        Task<IncomeSummaryDto> GetIncomeSummaryAsync(int userId);
        Task<bool> UpdateIncomeSourceAsync(int incomeId, int userId, UpdateIncomeSourceRequest request);
        Task<bool> DeleteIncomeSourceAsync(int incomeId, int userId);
        Task<decimal> GetTotalIncomeByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}