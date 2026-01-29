using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> CreateEnrollmentAsync(int userId, CreateEnrollmentRequest request);
        Task<EnrollmentDto?> GetEnrollmentByIdAsync(int enrollmentId, int userId);
        Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(int userId);
        Task<List<EnrollmentDto>> GetSemesterEnrollmentsAsync(int semesterId, int userId);
        Task<List<EnrollmentDto>> GetCompletedCoursesAsync(int userId);
        Task<List<EnrollmentDto>> GetInProgressCoursesAsync(int userId);
        Task<List<EnrollmentDto>> GetPlannedCoursesAsync(int userId);
        Task<bool> UpdateEnrollmentAsync(int enrollmentId, int userId, UpdateEnrollmentRequest request);
        Task<bool> DeleteEnrollmentAsync(int enrollmentId, int userId);
        Task<bool> DropCourseAsync(int enrollmentId, int userId);
    }
}