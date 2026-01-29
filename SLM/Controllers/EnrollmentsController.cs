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
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EnrollmentDto>>> GetEnrollments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id, userId);
            if (enrollment == null)
            {
                return NotFound(new { message = "Enrollment not found" });
            }
            return Ok(enrollment);
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetSemesterEnrollments(int semesterId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollments = await _enrollmentService.GetSemesterEnrollmentsAsync(semesterId, userId);
            return Ok(enrollments);
        }

        [HttpGet("completed")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetCompletedCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollments = await _enrollmentService.GetCompletedCoursesAsync(userId);
            return Ok(enrollments);
        }

        [HttpGet("in-progress")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetInProgressCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollments = await _enrollmentService.GetInProgressCoursesAsync(userId);
            return Ok(enrollments);
        }

        [HttpGet("planned")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetPlannedCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var enrollments = await _enrollmentService.GetPlannedCoursesAsync(userId);
            return Ok(enrollments);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(userId, request);
                return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _enrollmentService.UpdateEnrollmentAsync(id, userId, request);
            if (!success)
            {
                return NotFound(new { message = "Enrollment not found" });
            }
            return Ok(new { message = "Enrollment updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEnrollment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _enrollmentService.DeleteEnrollmentAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Enrollment not found" });
            }
            return Ok(new { message = "Enrollment deleted successfully" });
        }

        [HttpPost("{id}/drop")]
        public async Task<ActionResult> DropCourse(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _enrollmentService.DropCourseAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Enrollment not found" });
            }
            return Ok(new { message = "Course dropped successfully" });
        }
    }
}