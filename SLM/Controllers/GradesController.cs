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
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GradeDto>>> GetGrades()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var grades = await _gradeService.GetUserGradesAsync(userId);
            return Ok(grades);
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<ActionResult<List<GradeDto>>> GetSemesterGrades(int semesterId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var grades = await _gradeService.GetSemesterGradesAsync(semesterId, userId);
            return Ok(grades);
        }

        [HttpGet("gpa")]
        public async Task<ActionResult<GPACalculationDto>> GetGPA()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var gpa = await _gradeService.CalculateGPAAsync(userId);
            return Ok(gpa);
        }

        [HttpGet("gpa/semester/{semesterId}")]
        public async Task<ActionResult> GetSemesterGPA(int semesterId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var gpa = await _gradeService.CalculateSemesterGPAAsync(semesterId, userId);
            return Ok(new { semesterGPA = gpa });
        }

        [HttpGet("gpa/cumulative")]
        public async Task<ActionResult> GetCumulativeGPA()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var gpa = await _gradeService.CalculateCumulativeGPAAsync(userId);
            return Ok(new { cumulativeGPA = gpa });
        }

        [HttpPost("submit")]
        public async Task<ActionResult> SubmitGrade([FromBody] SubmitGradeRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _gradeService.SubmitGradeAsync(userId, request);
                return Ok(new { message = "Grade submitted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("update-semester-gpas")]
        public async Task<ActionResult> UpdateSemesterGPAs()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _gradeService.UpdateSemesterGPAsAsync(userId);
            return Ok(new { message = "Semester GPAs updated successfully" });
        }
    }
}