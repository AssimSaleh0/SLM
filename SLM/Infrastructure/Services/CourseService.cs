using Microsoft.EntityFrameworkCore;
using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using SLM.Core.Models;

namespace SLM.Infrastructure.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseRequest request)
        {
            var existingCourse = await _unitOfWork.Repository<Course>()
                .FirstOrDefaultAsync(c => c.CourseCode == request.CourseCode);

            if (existingCourse != null)
            {
                throw new InvalidOperationException("A course with this code already exists");
            }

            var course = new Course
            {
                CourseCode = request.CourseCode,
                CourseName = request.CourseName,
                Description = request.Description,
                Credits = request.Credits,
                Department = request.Department,
                Level = request.Level
            };

            await _unitOfWork.Repository<Course>().AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Add prerequisites
            foreach (var prereqId in request.PrerequisiteCourseIds)
            {
                var prerequisite = new CoursePrerequisite
                {
                    CourseId = course.Id,
                    PrerequisiteCourseId = prereqId,
                    IsRequired = true
                };

                await _unitOfWork.Repository<CoursePrerequisite>().AddAsync(prerequisite);
            }

            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(course);
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId);
            return course != null ? await MapToDtoAsync(course) : null;
        }

        public async Task<CourseDto?> GetCourseByCodeAsync(string courseCode)
        {
            var course = await _unitOfWork.Repository<Course>()
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            return course != null ? await MapToDtoAsync(course) : null;
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _unitOfWork.Repository<Course>().GetAllAsync();
            var courseDtos = new List<CourseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(await MapToDtoAsync(course));
            }

            return courseDtos;
        }

        public async Task<List<CourseDto>> GetCoursesByDepartmentAsync(string department)
        {
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => c.Department == department);
            var courseDtos = new List<CourseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(await MapToDtoAsync(course));
            }

            return courseDtos;
        }

        public async Task<List<CourseDto>> SearchCoursesAsync(string searchTerm)
        {
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => c.CourseCode.Contains(searchTerm) ||
                               c.CourseName.Contains(searchTerm) ||
                               c.Description.Contains(searchTerm));
            var courseDtos = new List<CourseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(await MapToDtoAsync(course));
            }

            return courseDtos;
        }

        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseRequest request)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId);

            if (course == null)
            {
                return false;
            }

            if (request.CourseName != null)
                course.CourseName = request.CourseName;
            if (request.Description != null)
                course.Description = request.Description;
            if (request.Credits.HasValue)
                course.Credits = request.Credits.Value;
            if (request.Department != null)
                course.Department = request.Department;
            if (request.Level.HasValue)
                course.Level = request.Level.Value;

            await _unitOfWork.Repository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId);

            if (course == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Course>().DeleteAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddPrerequisiteAsync(int courseId, int prerequisiteCourseId)
        {
            var existing = await _unitOfWork.Repository<CoursePrerequisite>()
                .FirstOrDefaultAsync(cp => cp.CourseId == courseId &&
                                          cp.PrerequisiteCourseId == prerequisiteCourseId);

            if (existing != null)
            {
                return false;
            }

            var prerequisite = new CoursePrerequisite
            {
                CourseId = courseId,
                PrerequisiteCourseId = prerequisiteCourseId,
                IsRequired = true
            };

            await _unitOfWork.Repository<CoursePrerequisite>().AddAsync(prerequisite);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemovePrerequisiteAsync(int courseId, int prerequisiteCourseId)
        {
            var prerequisite = await _unitOfWork.Repository<CoursePrerequisite>()
                .FirstOrDefaultAsync(cp => cp.CourseId == courseId &&
                                          cp.PrerequisiteCourseId == prerequisiteCourseId);

            if (prerequisite == null)
            {
                return false;
            }

            await _unitOfWork.Repository<CoursePrerequisite>().DeleteAsync(prerequisite);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<CourseDto>> GetPrerequisitesAsync(int courseId)
        {
            var prerequisites = await _unitOfWork.Repository<CoursePrerequisite>()
                .FindAsync(cp => cp.CourseId == courseId);

            var courseIds = prerequisites.Select(p => p.PrerequisiteCourseId).ToList();
            var courses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => courseIds.Contains(c.Id));

            var courseDtos = new List<CourseDto>();
            foreach (var course in courses)
            {
                courseDtos.Add(await MapToDtoAsync(course));
            }

            return courseDtos;
        }

        public async Task<bool> CheckPrerequisitesMetAsync(int userId, int courseId)
        {
            var prerequisites = await _unitOfWork.Repository<CoursePrerequisite>()
                .FindAsync(cp => cp.CourseId == courseId && cp.IsRequired);

            foreach (var prereq in prerequisites)
            {
                var completed = await _unitOfWork.Repository<Enrollment>()
                    .AnyAsync(e => e.UserId == userId &&
                                  e.CourseId == prereq.PrerequisiteCourseId &&
                                  e.IsCompleted);

                if (!completed)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<CourseDto> MapToDtoAsync(Course course)
        {
            var prerequisites = await _unitOfWork.Repository<CoursePrerequisite>()
                .FindAsync(cp => cp.CourseId == course.Id);

            var prereqCourseIds = prerequisites.Select(p => p.PrerequisiteCourseId).ToList();
            var prereqCourses = await _unitOfWork.Repository<Course>()
                .FindAsync(c => prereqCourseIds.Contains(c.Id));

            return new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Credits = course.Credits,
                Department = course.Department,
                Level = course.Level,
                PrerequisiteCodes = prereqCourses.Select(c => c.CourseCode).ToList()
            };
        }
    }
}