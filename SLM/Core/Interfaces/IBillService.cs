using SLM.Core.DTOs.Financial;

namespace SLM.Core.Interfaces
{
    public interface IBillService
    {
        Task<BillDto> CreateBillAsync(int userId, CreateBillRequest request);
        Task<BillDto?> GetBillByIdAsync(int billId, int userId);
        Task<List<BillDto>> GetUserBillsAsync(int userId);
        Task<List<BillDto>> GetUpcomingBillsAsync(int userId, int days = 30);
        Task<List<BillDto>> GetOverdueBillsAsync(int userId);
        Task<BillSummaryDto> GetBillSummaryAsync(int userId);
        Task<bool> UpdateBillAsync(int billId, int userId, UpdateBillRequest request);
        Task<bool> MarkBillAsPaidAsync(int billId, int userId);
        Task<bool> DeleteBillAsync(int billId, int userId);
        Task GenerateRecurringBillsAsync(int userId);
    }
}