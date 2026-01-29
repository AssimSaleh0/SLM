using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GradeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> SubmitGradeAsync(int userId, SubmitGradeRequest request)
        {
            var enrollment = await _unitOfWork.Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId && e.UserId == userId);

            if (enrollment == null)
            {
                throw new InvalidOperationException("Enrollment not found");
            }

            enrollment.Grade = request.Grade;
            enrollment.GradePoint = ConvertGradeToGPA(request.Grade);
            enrollment.IsCompleted = true;
            enrollment.IsInProgress = false;

            await _unitOfWork.Repository<Enrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            // Update semester GPA
            await UpdateSemesterGPAsAsync(userId);

            return true;
        }

        public async Task<List<GradeDto>> GetUserGradesAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsCompleted && e.Grade != null);

            var dtos = new List<GradeDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToGradeDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<List<GradeDto>> GetSemesterGradesAsync(int semesterId, int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.SemesterId == semesterId &&
                               e.UserId == userId &&
                               e.IsCompleted &&
                               e.Grade != null);

            var dtos = new List<GradeDto>();
            foreach (var enrollment in enrollments)
            {
                dtos.Add(await MapToGradeDtoAsync(enrollment));
            }

            return dtos;
        }

        public async Task<GPACalculationDto> CalculateGPAAsync(int userId)
        {
            var currentSemester = await _unitOfWork.Repository<Semester>()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsCurrent);

            var currentSemesterGPA = currentSemester != null
                ? await CalculateSemesterGPAAsync(currentSemester.Id, userId)
                : 0;

            var cumulativeGPA = await CalculateCumulativeGPAAsync(userId);

            var completedEnrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsCompleted);

            var inProgressEnrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress);

            var completedCourseIds = completedEnrollments.Select(e => e.CourseId).ToList();
            var completedCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => completedCourseIds.Contains(c.Id));

            var inProgressCourseIds = inProgressEnrollments.Select(e => e.CourseId).ToList();
            var inProgressCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => inProgressCourseIds.Contains(c.Id));

            var semesters = await _unitOfWork.Repository<Semester>()
                .FindAsync(s => s.UserId == userId && s.IsCompleted);

            var semesterGPAs = new List<SemesterGPADto>();
            foreach (var semester in semesters)
            {
                var gpa = await CalculateSemesterGPAAsync(semester.Id, userId);
                semesterGPAs.Add(new SemesterGPADto
                {
                    SemesterName = $"{semester.Season} {semester.Year}",
                    GPA = gpa,
                    Credits = semester.TotalCredits
                });
            }

            return new GPACalculationDto
            {
                CurrentSemesterGPA = currentSemesterGPA,
                CumulativeGPA = cumulativeGPA,
                TotalCreditsCompleted = completedCourses.Sum(c => c.Credits),
                TotalCreditsInProgress = inProgressCourses.Sum(c => c.Credits),
                SemesterGPAs = semesterGPAs
            };
        }

        public async Task<decimal> CalculateSemesterGPAAsync(int semesterId, int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.SemesterId == semesterId &&
                               e.UserId == userId &&
                               e.IsCompleted &&
                               e.Grade != null);

            if (!enrollments.Any())
            {
                return 0;
            }

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => courseIds.Contains(c.Id));

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var enrollment in enrollments)
            {
                var course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                if (course != null && enrollment.GradePoint.HasValue)
                {
                    totalPoints += enrollment.GradePoint.Value * course.Credits;
                    totalCredits += course.Credits;
                }
            }

            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        }

        public async Task<decimal> CalculateCumulativeGPAAsync(int userId)
        {
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId &&
                               e.IsCompleted &&
                               e.Grade != null);

            if (!enrollments.Any())
            {
                return 0;
            }

            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => courseIds.Contains(c.Id));

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var enrollment in enrollments)
            {
                var course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                if (course != null && enrollment.GradePoint.HasValue)
                {
                    totalPoints += enrollment.GradePoint.Value * course.Credits;
                    totalCredits += course.Credits;
                }
            }

            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        }

        public async Task<bool> UpdateSemesterGPAsAsync(int userId)
        {
            var semesters = await _unitOfWork.Repository<Semester>()
                .FindAsync(s => s.UserId == userId);

            foreach (var semester in semesters)
            {
                var gpa = await CalculateSemesterGPAAsync(semester.Id, userId);
                semester.GPA = gpa;

                var enrollments = await _unitOfWork.Repository<Enrollment>()
                    .FindAsync(e => e.SemesterId == semester.Id);

                var courseIds = enrollments.Select(e => e.CourseId).ToList();
                var courses = await _unitOfWork.Repository<Course>()
                    .FindAsync(c => courseIds.Contains(c.Id));

                semester.TotalCredits = courses.Sum(c => c.Credits);

                await _unitOfWork.Repository<Semester>().UpdateAsync(semester);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        //doublecheck this method
        public decimal ConvertGradeToGPA(string grade)
        {
            return grade?.ToUpper() switch
            {
                "A+" => 4.0m,
                "A" => 4.0m,
                "A-" => 3.7m,
                "B+" => 3.3m,
                "B" => 3.0m,
                "B-" => 2.7m,
                "C+" => 2.3m,
                "C" => 2.0m,
                "C-" => 1.7m,
                "D+" => 1.3m,
                "D" => 1.0m,
                "D-" => 0.7m,
                "F" => 0.0m,
                _ => 0.0m
            };
        }

        private async Task<GradeDto> MapToGradeDtoAsync(Enrollment enrollment)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(enrollment.CourseId);
            var semester = await _unitOfWork.Repository<Semester>().GetByIdAsync(enrollment.SemesterId);

            return new GradeDto
            {
                EnrollmentId = enrollment.Id,
                CourseCode = course?.CourseCode ?? "",
                CourseName = course?.CourseName ?? "",
                Credits = course?.Credits ?? 0,
                Grade = enrollment.Grade,
                GradePoint = enrollment.GradePoint,
                SemesterName = semester != null ? $"{semester.Season} {semester.Year}" : ""
            };
        }
    }
}