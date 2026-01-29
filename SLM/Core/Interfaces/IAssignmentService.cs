using SLM.Core.DTOs.Academic;

namespace SLM.Core.Interfaces
{
    public interface IAssignmentService
    {
        Task<AssignmentDto> CreateAssignmentAsync(int userId, CreateAssignmentRequest request);
        Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId, int userId);
        Task<List<AssignmentDto>> GetUserAssignmentsAsync(int userId);
        Task<List<AssignmentDto>> GetEnrollmentAssignmentsAsync(int enrollmentId, int userId);
        Task<List<AssignmentDto>> GetUpcomingAssignmentsAsync(int userId, int days = 7);
        Task<List<AssignmentDto>> GetOverdueAssignmentsAsync(int userId);
        Task<bool> UpdateAssignmentAsync(int assignmentId, int userId, UpdateAssignmentRequest request);
        Task<bool> DeleteAssignmentAsync(int assignmentId, int userId);
        Task<bool> MarkAssignmentCompletedAsync(int assignmentId, int userId);
    }
}