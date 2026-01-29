using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface IStudyPlanService
    {
        Task<StudyPlanDto> CreateStudyPlanAsync(int userId, CreateStudyPlanRequest request);
        Task<List<StudyPlanDto>> GetUserStudyPlansAsync(int userId);
        Task<List<StudyPlanDto>> GetStudyPlansByDateAsync(int userId, DateTime date);
        Task<List<StudyPlanSummaryDto>> GetWeeklyStudyPlanAsync(int userId, DateTime startDate);
        Task<List<StudyPlanDto>> GenerateStudyPlanAsync(int userId, GenerateStudyPlanRequest request);
        Task<bool> UpdateStudyPlanAsync(int studyPlanId, int userId, CreateStudyPlanRequest request);
        Task<bool> MarkStudyPlanCompletedAsync(int studyPlanId, int userId);
        Task<bool> DeleteStudyPlanAsync(int studyPlanId, int userId);
    }
}