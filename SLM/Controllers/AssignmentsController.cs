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
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AssignmentDto>>> GetAssignments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignments = await _assignmentService.GetUserAssignmentsAsync(userId);
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDto>> GetAssignment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id, userId);
            if (assignment == null)
            {
                return NotFound(new { message = "Assignment not found" });
            }
            return Ok(assignment);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<List<AssignmentDto>>> GetEnrollmentAssignments(int enrollmentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignments = await _assignmentService.GetEnrollmentAssignmentsAsync(enrollmentId, userId);
            return Ok(assignments);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<AssignmentDto>>> GetUpcomingAssignments([FromQuery] int days = 7)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignments = await _assignmentService.GetUpcomingAssignmentsAsync(userId, days);
            return Ok(assignments);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<List<AssignmentDto>>> GetOverdueAssignments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignments = await _assignmentService.GetOverdueAssignmentsAsync(userId);
            return Ok(assignments);
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentDto>> CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var assignment = await _assignmentService.CreateAssignmentAsync(userId, request);
                return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAssignment(int id, [FromBody] UpdateAssignmentRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _assignmentService.UpdateAssignmentAsync(id, userId, request);
            if (!success)
            {
                return NotFound(new { message = "Assignment not found" });
            }
            return Ok(new { message = "Assignment updated successfully" });
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> MarkAssignmentCompleted(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _assignmentService.MarkAssignmentCompletedAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Assignment not found" });
            }
            return Ok(new { message = "Assignment marked as completed" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAssignment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _assignmentService.DeleteAssignmentAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Assignment not found" });
            }
            return Ok(new { message = "Assignment deleted successfully" });
        }
    }
}