using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AssignmentDto> CreateAssignmentAsync(int userId, CreateAssignmentRequest request)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                throw new InvalidOperationException("Enrollment not found");
            }

            var assignment = new Assignment
            {
                EnrollmentId = request.EnrollmentId,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                MaxPoints = request.MaxPoints,
                AssignmentType = request.AssignmentType,
                IsCompleted = false
            };

            await _unitOfWork.Repository<Assignment>().AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(assignment);
        }

        public async Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId, int userId)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);

            if (assignment == null)
            {
                return null;
            }

            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == assignment.EnrollmentId && e.UserId == userId);

            return enrollment != null ? await MapToDtoAsync(assignment) : null;
        }

        public async Task<List<AssignmentDto>> GetUserAssignmentsAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId);

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();
            var assignments = await _unitOfWork.Repository<Assignment>()
                .FindAsync(a => enrollmentIds.Contains(a.EnrollmentId));

            var dtos = new List<AssignmentDto>();
            foreach (var assignment in assignments.OrderBy(a => a.DueDate))
            {
                dtos.Add(await MapToDtoAsync(assignment));
            }

            return dtos;
        }

        public async Task<List<AssignmentDto>> GetEnrollmentAssignmentsAsync(int enrollmentId, int userId)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return new List<AssignmentDto>();
            }

            var assignments = await _unitOfWork.Repository<Assignment>()
                .FindAsync(a => a.EnrollmentId == enrollmentId);

            var dtos = new List<AssignmentDto>();
            foreach (var assignment in assignments.OrderBy(a => a.DueDate))
            {
                dtos.Add(await MapToDtoAsync(assignment));
            }

            return dtos;
        }

        public async Task<List<AssignmentDto>> GetUpcomingAssignmentsAsync(int userId, int days = 7)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress);

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();
            var cutoffDate = DateTime.UtcNow.AddDays(days);

            var assignments = await _unitOfWork.Repository<Assignment>()
                .FindAsync(a => enrollmentIds.Contains(a.EnrollmentId) &&
                               !a.IsCompleted &&
                               a.DueDate <= cutoffDate &&
                               a.DueDate >= DateTime.UtcNow);

            var dtos = new List<AssignmentDto>();
            foreach (var assignment in assignments.OrderBy(a => a.DueDate))
            {
                dtos.Add(await MapToDtoAsync(assignment));
            }

            return dtos;
        }

        public async Task<List<AssignmentDto>> GetOverdueAssignmentsAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress);

            var enrollmentIds = enrollments.Select(e => e.Id).ToList();

            var assignments = await _unitOfWork.Repository<Assignment>()
                .FindAsync(a => enrollmentIds.Contains(a.EnrollmentId) &&
                               !a.IsCompleted &&
                               a.DueDate < DateTime.UtcNow);

            var dtos = new List<AssignmentDto>();
            foreach (var assignment in assignments.OrderBy(a => a.DueDate))
            {
                dtos.Add(await MapToDtoAsync(assignment));
            }

            return dtos;
        }

        public async Task<bool> UpdateAssignmentAsync(int assignmentId, int userId, UpdateAssignmentRequest request)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);

            if (assignment == null)
            {
                return false;
            }

            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == assignment.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return false;
            }

            if (request.Title != null)
                assignment.Title = request.Title;
            if (request.Description != null)
                assignment.Description = request.Description;
            if (request.DueDate.HasValue)
                assignment.DueDate = request.DueDate.Value;
            if (request.MaxPoints.HasValue)
                assignment.MaxPoints = request.MaxPoints.Value;
            if (request.EarnedPoints.HasValue)
                assignment.EarnedPoints = request.EarnedPoints.Value;
            if (request.IsCompleted.HasValue)
                assignment.IsCompleted = request.IsCompleted.Value;
            if (request.AssignmentType != null)
                assignment.AssignmentType = request.AssignmentType;

            await _unitOfWork.Repository<Assignment>().UpdateAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAssignmentAsync(int assignmentId, int userId)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);

            if (assignment == null)
            {
                return false;
            }

            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == assignment.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Assignment>().DeleteAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAssignmentCompletedAsync(int assignmentId, int userId)
        {
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(assignmentId);

            if (assignment == null)
            {
                return false;
            }

            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == assignment.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return false;
            }

            assignment.IsCompleted = true;
            await _unitOfWork.Repository<Assignment>().UpdateAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<AssignmentDto> MapToDtoAsync(Assignment assignment)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>().GetByIdAsync(assignment.EnrollmentId);
            var course = enrollment != null
                ? await _unitOfWork.Repository<Course>().GetByIdAsync(enrollment.CourseId)
                : null;

            var daysUntilDue = (assignment.DueDate - DateTime.UtcNow).Days;

            return new AssignmentDto
            {
                Id = assignment.Id,
                EnrollmentId = assignment.EnrollmentId,
                CourseCode = course?.CourseCode ?? "",
                CourseName = course?.CourseName ?? "",
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                MaxPoints = assignment.MaxPoints,
                EarnedPoints = assignment.EarnedPoints,
                IsCompleted = assignment.IsCompleted,
                AssignmentType = assignment.AssignmentType,
                DaysUntilDue = daysUntilDue
            };
        }
    }
}