using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface ISemesterService
    {
        Task<SemesterDto> CreateSemesterAsync(int userId, CreateSemesterRequest request);
        Task<SemesterDto?> GetSemesterByIdAsync(int semesterId, int userId);
        Task<List<SemesterDto>> GetUserSemestersAsync(int userId);
        Task<SemesterDto?> GetCurrentSemesterAsync(int userId);
        Task<bool> UpdateSemesterAsync(int semesterId, int userId, UpdateSemesterRequest request);
        Task<bool> DeleteSemesterAsync(int semesterId, int userId);
        Task<bool> SetCurrentSemesterAsync(int semesterId, int userId);
        Task<bool> CompleteSemesterAsync(int semesterId, int userId);
        Task<int> GetTotalCreditsAsync(int semesterId, int userId);
        Task<bool> ValidateCreditLoadAsync(int semesterId, int userId, int maxCredits = 18);
    }
}