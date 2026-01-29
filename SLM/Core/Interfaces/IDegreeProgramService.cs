using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface IDegreeProgramService
    {
        Task<DegreeProgramDto> CreateDegreeProgramAsync(CreateDegreeProgramRequest request);
        Task<DegreeProgramDto?> GetDegreeProgramByIdAsync(int programId);
        Task<List<DegreeProgramDto>> GetAllDegreeProgramsAsync();
        Task<bool> EnrollUserInProgramAsync(int userId, int programId, DateTime startDate);
        Task<DegreeProgressDto?> GetUserProgressAsync(int userId);
        Task<List<CourseDto>> GetProgramRequiredCoursesAsync(int programId);
        Task<bool> AddCourseRequirementAsync(int programId, int courseId, bool isRequired, int? recommendedSemester);
        Task<bool> RemoveCourseRequirementAsync(int programId, int courseId);
    }
}