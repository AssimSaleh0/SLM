using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SemesterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SemesterDto> CreateSemesterAsync(int userId, CreateSemesterRequest request)
        {
            var existing = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                         s.Season == request.Season &&
                                         s.Year == request.Year);

            if (existing != null)
            {
                throw new InvalidOperationException("This semester already exists");
            }

            var semester = new Semester
            {
                UserId = userId,
                Season = request.Season,
                Year = request.Year,
                IsCurrent = false,
                IsCompleted = false,
                TotalCredits = 0
            };

            await _unitOfWork.Repository<Semester>().AddAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(semester);
        }

        public async Task<SemesterDto?> GetSemesterByIdAsync(int semesterId, int userId)
        {
            var semester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.Id == semesterId && s.UserId == userId);

            return semester != null ? await MapToDtoWithEnrollmentsAsync(semester) : null;
        }

        public async Task<List<SemesterDto>> GetUserSemestersAsync(int userId)
        {
            var semesters = await _unitOfWork.Repository<Semester>()
                .FindAsync(s => s.UserId == userId);

            var semesterDtos = new List<SemesterDto>();
            foreach (var semester in semesters.OrderByDescending(s => s.Year).ThenByDescending(s => s.Season))
            {
                semesterDtos.Add(await MapToDtoWithEnrollmentsAsync(semester));
            }

            return semesterDtos;
        }

        public async Task<SemesterDto?> GetCurrentSemesterAsync(int userId)
        {
            var semester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsCurrent);

            return semester != null ? await MapToDtoWithEnrollmentsAsync(semester) : null;
        }

        public async Task<bool> UpdateSemesterAsync(int semesterId, int userId, UpdateSemesterRequest request)
        {
            var semester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.Id == semesterId && s.UserId == userId);

            if (semester == null)
            {
                return false;
            }

            if (request.IsCurrent.HasValue)
                semester.IsCurrent = request.IsCurrent.Value;
            if (request.IsCompleted.HasValue)
                semester.IsCompleted = request.IsCompleted.Value;

            await _unitOfWork.Repository<Semester>().UpdateAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSemesterAsync(int semesterId, int userId)
        {
            var semester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.Id == semesterId && s.UserId == userId);

            if (semester == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Semester>().DeleteAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetCurrentSemesterAsync(int semesterId, int userId)
        {
            var allSemesters = await _unitOfWork.Repository<Semester>()
                .FindAsync(s => s.UserId == userId);

            foreach (var sem in allSemesters)
            {
                sem.IsCurrent = sem.Id == semesterId;
                await _unitOfWork.Repository<Semester>().UpdateAsync(sem);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteSemesterAsync(int semesterId, int userId)
        {
            var semester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.Id == semesterId && s.UserId == userId);

            if (semester == null)
            {
                return false;
            }

            semester.IsCompleted = true;
            semester.IsCurrent = false;

            await _unitOfWork.Repository<Semester>().UpdateAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetTotalCreditsAsync(int semesterId, int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.SemesterId == semesterId && e.UserId == userId);

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => courseIds.Contains(c.Id));

            return courses.Sum(c => c.Credits);
        }

        public async Task<bool> ValidateCreditLoadAsync(int semesterId, int userId, int maxCredits = 18)
        {
            var totalCredits = await GetTotalCreditsAsync(semesterId, userId);
            return totalCredits <= maxCredits;
        }

        private SemesterDto MapToDto(Semester semester)
        {
            return new SemesterDto
            {
                Id = semester.Id,
                Season = semester.Season,
                Year = semester.Year,
                IsCurrent = semester.IsCurrent,
                IsCompleted = semester.IsCompleted,
                GPA = semester.GPA,
                TotalCredits = semester.TotalCredits
            };
        }

        private async Task<SemesterDto> MapToDtoWithEnrollmentsAsync(Semester semester)
        {
            var dto = MapToDto(semester);

            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.SemesterId == semester.Id);

            dto.EnrolledCourses = enrollments.Count();

            return dto;
        }
    }
}