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
    public class StudyPlansController : ControllerBase
    {
        private readonly IStudyPlanService _studyPlanService;

        public StudyPlansController(IStudyPlanService studyPlanService)
        {
            _studyPlanService = studyPlanService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudyPlanDto>>> GetStudyPlans()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plans = await _studyPlanService.GetUserStudyPlansAsync(userId);
            return Ok(plans);
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<List<StudyPlanDto>>> GetStudyPlansByDate(DateTime date)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plans = await _studyPlanService.GetStudyPlansByDateAsync(userId, date);
            return Ok(plans);
        }

        [HttpGet("weekly")]
        public async Task<ActionResult<List<StudyPlanSummaryDto>>> GetWeeklyStudyPlan([FromQuery] DateTime? startDate = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var date = startDate ?? DateTime.UtcNow;
            var plans = await _studyPlanService.GetWeeklyStudyPlanAsync(userId, date);
            return Ok(plans);
        }

        [HttpPost]
        public async Task<ActionResult<StudyPlanDto>> CreateStudyPlan([FromBody] CreateStudyPlanRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var plan = await _studyPlanService.CreateStudyPlanAsync(userId, request);
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("generate")]
        public async Task<ActionResult<List<StudyPlanDto>>> GenerateStudyPlan([FromBody] GenerateStudyPlanRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plans = await _studyPlanService.GenerateStudyPlanAsync(userId, request);
            return Ok(plans);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudyPlan(int id, [FromBody] CreateStudyPlanRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _studyPlanService.UpdateStudyPlanAsync(id, userId, request);
            if (!success)
            {
                return NotFound(new { message = "Study plan not found" });
            }
            return Ok(new { message = "Study plan updated successfully" });
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> MarkStudyPlanCompleted(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _studyPlanService.MarkStudyPlanCompletedAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Study plan not found" });
            }
            return Ok(new { message = "Study plan marked as completed" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudyPlan(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _studyPlanService.DeleteStudyPlanAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Study plan not found" });
            }
            return Ok(new { message = "Study plan deleted successfully" });
        }
    }
}