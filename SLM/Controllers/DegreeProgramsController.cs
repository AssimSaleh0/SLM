using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLM.Core.DTOs.Academic;
using SLM.Core.Interfaces;
using System.Security.Claims;

namespace SLM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DegreeProgramsController : ControllerBase
    {
        private readonly IDegreeProgramService _degreeProgramService;

        public DegreeProgramsController(IDegreeProgramService degreeProgramService)
        {
            _degreeProgramService = degreeProgramService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DegreeProgramDto>>> GetAllPrograms()
        {
            var programs = await _degreeProgramService.GetAllDegreeProgramsAsync();
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DegreeProgramDto>> GetProgram(int id)
        {
            var program = await _degreeProgramService.GetDegreeProgramByIdAsync(id);
            if (program == null)
            {
                return NotFound(new { message = "Program not found" });
            }
            return Ok(program);
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult<List<CourseDto>>> GetProgramCourses(int id)
        {
            var courses = await _degreeProgramService.GetProgramRequiredCoursesAsync(id);
            return Ok(courses);
        }

        [HttpGet("my-progress")]
        public async Task<ActionResult<DegreeProgressDto>> GetMyProgress()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var progress = await _degreeProgramService.GetUserProgressAsync(userId);
            if (progress == null)
            {
                return NotFound(new { message = "No active degree program found" });
            }
            return Ok(progress);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult<DegreeProgramDto>> CreateProgram([FromBody] CreateDegreeProgramRequest request)
        {
            try
            {
                var program = await _degreeProgramService.CreateDegreeProgramAsync(request);
                return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/enroll")]
        public async Task<ActionResult> EnrollInProgram(int id, [FromBody] DateTime startDate)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _degreeProgramService.EnrollUserInProgramAsync(userId, id, startDate);
                return Ok(new { message = "Enrolled in program successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{programId}/courses/{courseId}")]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult> AddCourseRequirement(
            int programId,
            int courseId,
            [FromQuery] bool isRequired = true,
            [FromQuery] int? recommendedSemester = null)
        {
            var success = await _degreeProgramService.AddCourseRequirementAsync(
                programId, courseId, isRequired, recommendedSemester);

            if (!success)
            {
                return BadRequest(new { message = "Course requirement already exists" });
            }
            return Ok(new { message = "Course requirement added successfully" });
        }

        [HttpDelete("{programId}/courses/{courseId}")]
        [Authorize(Roles = "Administrator,AcademicAdvisor")]
        public async Task<ActionResult> RemoveCourseRequirement(int programId, int courseId)
        {
            var success = await _degreeProgramService.RemoveCourseRequirementAsync(programId, courseId);
            if (!success)
            {
                return NotFound(new { message = "Course requirement not found" });
            }
            return Ok(new { message = "Course requirement removed successfully" });
        }
    }
}