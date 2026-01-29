using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EnrollmentDto> CreateEnrollmentAsync(int userId, CreateEnrollmentRequest request)
        {
            var existing = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.UserId == userId &&
                                         e.SemesterId == request.SemesterId &&
                                         e.CourseId == request.CourseId);

            if (existing != null)
            {
                throw new InvalidOperationException("Already enrolled in this course for this semester");
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                SemesterId = request.SemesterId,
                CourseId = request.CourseId,
                IsPlanned = request.IsPlanned,
                IsInProgress = !request.IsPlanned,
                IsCompleted = false
            };

            await _unitOfWork.Repository<Enrollment>().AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(enrollment);
        }

        public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(int enrollmentId, int userId)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId);

            return enrollment != null ? await MapToDtoAsync(enrollment) : null;
        }

        public async Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId);

            var dtos = new List<EnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<List<EnrollmentDto>> GetSemesterEnrollmentsAsync(int semesterId, int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.SemesterId == semesterId && e.UserId == userId);

            var dtos = new List<EnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<List<EnrollmentDto>> GetCompletedCoursesAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsCompleted);

            var dtos = new List<EnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<List<EnrollmentDto>> GetInProgressCoursesAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress && !e.IsCompleted);

            var dtos = new List<EnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<List<EnrollmentDto>> GetPlannedCoursesAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsPlanned);

            var dtos = new List<EnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<bool> UpdateEnrollmentAsync(int enrollmentId, int userId, UpdateEnrollmentRequest request)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return false;
            }

            if (request.Grade != null)
                enrollment.Grade = request.Grade;
            if (request.IsCompleted.HasValue)
                enrollment.IsCompleted = request.IsCompleted.Value;
            if (request.IsInProgress.HasValue)
                enrollment.IsInProgress = request.IsInProgress.Value;

            await _unitOfWork.Repository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEnrollmentAsync(int enrollmentId, int userId)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Enrollment>().DeleteAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DropCourseAsync(int enrollmentId, int userId)
        {
            return await DeleteEnrollmentAsync(enrollmentId, userId);
        }

        private async Task<EnrollmentDto> MapToDtoAsync(Enrollment enrollment)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(enrollment.CourseId);

            return new EnrollmentDto
            {
                Id = enrollment.Id,
                SemesterId = enrollment.SemesterId,
                CourseId = enrollment.CourseId,
                CourseCode = course?.CourseCode ?? "",
                CourseName = course?.CourseName ?? "",
                Credits = course?.Credits ?? 0,
                Grade = enrollment.Grade,
                GradePoint = enrollment.GradePoint,
                IsCompleted = enrollment.IsCompleted,
                IsInProgress = enrollment.IsInProgress,
                IsPlanned = enrollment.IsPlanned
            };
        }
    }
}