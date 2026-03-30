using SLM.Core.DTOs.Financial;
using Microsoft.AspNetCore.Http;

namespace SLM.Core.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptDto> UploadReceiptAsync(int userId, IFormFile file, UploadReceiptRequest request);
        Task<ReceiptDto?> GetReceiptByIdAsync(int receiptId, int userId);
        Task<List<ReceiptDto>> GetUserReceiptsAsync(int userId);
        Task<List<ReceiptDto>> GetReceiptsByCategoryAsync(int userId, int categoryId);
        Task<bool> UpdateReceiptAsync(int receiptId, int userId, UpdateReceiptRequest request);
        Task<bool> DeleteReceiptAsync(int receiptId, int userId);
        Task<string> GetReceiptImageUrlAsync(int receiptId, int userId);
    }
}