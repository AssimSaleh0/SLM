using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface IGradeService
    {
        Task<bool> SubmitGradeAsync(int userId, SubmitGradeRequest request);
        Task<List<GradeDto>> GetUserGradesAsync(int userId);
        Task<List<GradeDto>> GetSemesterGradesAsync(int semesterId, int userId);
        Task<GPACalculationDto> CalculateGPAAsync(int userId);
        Task<decimal> CalculateSemesterGPAAsync(int semesterId, int userId);
        Task<decimal> CalculateCumulativeGPAAsync(int userId);
        Task<bool> UpdateSemesterGPAsAsync(int userId);
        decimal ConvertGradeToGPA(string grade);
    }
}