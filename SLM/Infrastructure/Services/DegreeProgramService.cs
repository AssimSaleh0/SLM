using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class DegreeProgramService : IDegreeProgramService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DegreeProgramService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DegreeProgramDto> CreateDegreeProgramAsync(CreateDegreeProgramRequest request)
        {
            var existing = await _unitOfWork.Repository<DegreeProgram>()
                .FirstOrDefaultAsync(dp => dp.ProgramName == request.ProgramName);

            if (existing != null)
            {
                throw new InvalidOperationException("A program with this name already exists");
            }

            var program = new DegreeProgram
            {
                ProgramName = request.ProgramName,
                Department = request.Department,
                Description = request.Description,
                TotalCreditsRequired = request.TotalCreditsRequired
            };

            await _unitOfWork.Repository<DegreeProgram>().AddAsync(program);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(program);
        }

        public async Task<DegreeProgramDto?> GetDegreeProgramByIdAsync(int programId)
        {
            var program = await _unitOfWork.Repository<DegreeProgram>().GetByIdAsync(programId);
            return program != null ? await MapToDtoAsync(program) : null;
        }

        public async Task<List<DegreeProgramDto>> GetAllDegreeProgramsAsync()
        {
            var programs = await _unitOfWork.Repository<DegreeProgram>().GetAllAsync();
            var dtos = new List<DegreeProgramDto>();

            foreach (var program in programs)
            {
                dtos.Add(await MapToDtoAsync(program));
            }

            return dtos;
        }

        public async Task<bool> EnrollUserInProgramAsync(int userId, int programId, DateTime startDate)
        {
            var existing = await _unitOfWork.Repository<UserDegreeProgram>()
                .FirstOrDefaultAsync(udp => udp.UserId == userId &&
                                           udp.DegreeProgramId == programId &&
                                           udp.IsActive);

            if (existing != null)
            {
                throw new InvalidOperationException("User is already enrolled in this program");
            }

            var userProgram = new UserDegreeProgram
            {
                UserId = userId,
                DegreeProgramId = programId,
                StartDate = startDate,
                IsActive = true
            };

            await _unitOfWork.Repository<UserDegreeProgram>().AddAsync(userProgram);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<DegreeProgressDto?> GetUserProgressAsync(int userId)
        {
            var userProgram = await _unitOfWork.Repository<UserDegreeProgram>()
                .FirstOrDefaultAsync(udp => udp.UserId == userId && udp.IsActive);

            if (userProgram == null)
            {
                return null;
            }

            var program = await _unitOfWork.Repository<DegreeProgram>()
                .GetByIdAsync(userProgram.DegreeProgramId);

            if (program == null)
            {
                return null;
            }

            var requirements = await _unitOfWork.Repository<DegreeRequirement>()
                .FindAsync(dr => dr.DegreeProgramId == program.Id);

            var completedEnrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsCompleted);

            var inProgressEnrollments = await _unitOfWork.Repository<Enrollment>()
                .FindAsync(e => e.UserId == userId && e.IsInProgress);

            var completedCourseIds = completedEnrollments.Select(e => e.CourseId).ToList();
            var inProgressCourseIds = inProgressEnrollments.Select(e => e.CourseId).ToList();
            var requiredCourseIds = requirements.Where(r => r.IsRequired).Select(r => r.CourseId).ToList();

            var completedCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => completedCourseIds.Contains(c.Id));

            var inProgressCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => inProgressCourseIds.Contains(c.Id));

            var remainingCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => requiredCourseIds.Contains(c.Id) &&
                               !completedCourseIds.Contains(c.Id) &&
                               !inProgressCourseIds.Contains(c.Id));

            var completedCredits = completedCourses.Sum(c => c.Credits);
            var remainingCredits = program.TotalCreditsRequired - completedCredits;
            var progressPercentage = (decimal)completedCredits / program.TotalCreditsRequired * 100;

            return new DegreeProgressDto
            {
                Program = await MapToDtoAsync(program),
                CompletedCredits = completedCredits,
                RemainingCredits = remainingCredits,
                ProgressPercentage = Math.Round(progressPercentage, 2),
                CompletedCourses = completedCourses.Select(MapCourseToDto).ToList(),
                InProgressCourses = inProgressCourses.Select(MapCourseToDto).ToList(),
                RemainingRequiredCourses = remainingCourses.Select(MapCourseToDto).ToList()
            };
        }

        public async Task<List<CourseDto>> GetProgramRequiredCoursesAsync(int programId)
        {
            var requirements = await _unitOfWork.Repository<DegreeRequirement>()
                .FindAsync(dr => dr.DegreeProgramId == programId && dr.IsRequired);

            var courseIds = requirements.Select(r => r.CourseId).ToList();
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => courseIds.Contains(c.Id));

            return courses.Select(MapCourseToDto).ToList();
        }

        public async Task<bool> AddCourseRequirementAsync(int programId, int courseId, bool isRequired, int? recommendedSemester)
        {
            var existing = await _unitOfWork.Repository<DegreeRequirement>()
                .FirstOrDefaultAsync(dr => dr.DegreeProgramId == programId && dr.CourseId == courseId);

            if (existing != null)
            {
                return false;
            }

            var requirement = new DegreeRequirement
            {
                DegreeProgramId = programId,
                CourseId = courseId,
                IsRequired = isRequired,
                IsElective = !isRequired,
                RecommendedSemester = recommendedSemester
            };

            await _unitOfWork.Repository<DegreeRequirement>().AddAsync(requirement);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCourseRequirementAsync(int programId, int courseId)
        {
            var requirement = await _unitOfWork.Repository<DegreeRequirement>()
                .FirstOrDefaultAsync(dr => dr.DegreeProgramId == programId && dr.CourseId == courseId);

            if (requirement == null)
            {
                return false;
            }

            await _unitOfWork.Repository<DegreeRequirement>().DeleteAsync(requirement);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<DegreeProgramDto> MapToDtoAsync(DegreeProgram program)
        {
            var requirements = await _unitOfWork.Repository<DegreeRequirement>()
                .FindAsync(dr => dr.DegreeProgramId == program.Id);

            return new DegreeProgramDto
            {
                Id = program.Id,
                ProgramName = program.ProgramName,
                Department = program.Department,
                Description = program.Description,
                TotalCreditsRequired = program.TotalCreditsRequired,
                RequiredCourses = requirements.Count(r => r.IsRequired),
                ElectiveCourses = requirements.Count(r => r.IsElective)
            };
        }

        private CourseDto MapCourseToDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Credits = course.Credits,
                Department = course.Department,
                Level = course.Level
            };
        }
    }
}