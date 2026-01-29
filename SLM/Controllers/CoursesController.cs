using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;

namespace SLM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(course);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<CourseDto>> GetCourseByCode(string code)
        {
            var course = await _courseService.GetCourseByCodeAsync(code);
            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(course);
        }

        [HttpGet("department/{department}")]
        public async Task<ActionResult<List<CourseDto>>> GetCoursesByDepartment(string department)
        {
            var courses = await _courseService.GetCoursesByDepartmentAsync(department);
            return Ok(courses);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<CourseDto>>> SearchCourses([FromQuery] string term)
        {
            var courses = await _courseService.SearchCoursesAsync(term);
            return Ok(courses);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseRequest request)
        {
            try
            {
                var course = await _courseService.CreateCourseAsync(request);
                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
        {
            var success = await _courseService.UpdateCourseAsync(id, request);
            if (!success)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(new { message = "Course updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var success = await _courseService.DeleteCourseAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(new { message = "Course deleted successfully" });
        }

        [HttpGet("{id}/prerequisites")]
        public async Task<ActionResult<List<CourseDto>>> GetPrerequisites(int id)
        {
            var prerequisites = await _courseService.GetPrerequisitesAsync(id);
            return Ok(prerequisites);
        }

        [HttpPost("{id}/prerequisites/{prerequisiteId}")]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult> AddPrerequisite(int id, int prerequisiteId)
        {
            var success = await _courseService.AddPrerequisiteAsync(id, prerequisiteId);
            if (!success)
            {
                return BadRequest(new { message = "Prerequisite already exists" });
            }
            return Ok(new { message = "Prerequisite added successfully" });
        }

        [HttpDelete("{id}/prerequisites/{prerequisiteId}")]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult> RemovePrerequisite(int id, int prerequisiteId)
        {
            var success = await _courseService.RemovePrerequisiteAsync(id, prerequisiteId);
            if (!success)
            {
                return NotFound(new { message = "Prerequisite not found" });
            }
            return Ok(new { message = "Prerequisite removed successfully" });
        }
    }
}