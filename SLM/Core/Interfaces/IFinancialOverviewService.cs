using SLM.Core.DTOs.Financial;

namespace SLM.Core.Interfaces
{
    public interface IFinancialOverviewService
    {
        Task<FinancialOverviewDto> GetFinancialOverviewAsync(int userId);
    }
}