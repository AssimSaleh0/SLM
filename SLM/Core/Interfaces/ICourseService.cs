using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseDto?> GetCourseByIdAsync(int courseId);
        Task<CourseDto?> GetCourseByCodeAsync(string courseCode);
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<List<CourseDto>> GetCoursesByDepartmentAsync(string department);
        Task<List<CourseDto>> SearchCoursesAsync(string searchTerm);
        Task<bool> UpdateCourseAsync(int courseId, UpdateCourseRequest request);
        Task<bool> DeleteCourseAsync(int courseId);
        Task<bool> AddPrerequisiteAsync(int courseId, int prerequisiteCourseId);
        Task<bool> RemovePrerequisiteAsync(int courseId, int prerequisiteCourseId);
        Task<List<CourseDto>> GetPrerequisitesAsync(int courseId);
        Task<bool> CheckPrerequisitesMetAsync(int userId, int courseId);
    }
}