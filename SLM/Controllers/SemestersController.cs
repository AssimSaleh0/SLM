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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SemesterDto>>> GetSemesters()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var semesters = await _semesterService.GetUserSemestersAsync(userId);
            return Ok(semesters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SemesterDto>> GetSemester(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var semester = await _semesterService.GetSemesterByIdAsync(id, userId);
            if (semester == null)
            {
                return NotFound(new { message = "Semester not found" });
            }
            return Ok(semester);
        }

        [HttpGet("current")]
        public async Task<ActionResult<SemesterDto>> GetCurrentSemester()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var semester = await _semesterService.GetCurrentSemesterAsync(userId);
            if (semester == null)
            {
                return NotFound(new { message = "No current semester set" });
            }
            return Ok(semester);
        }

        [HttpPost]
        public async Task<ActionResult<SemesterDto>> CreateSemester([FromBody] CreateSemesterRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var semester = await _semesterService.CreateSemesterAsync(userId, request);
                return CreatedAtAction(nameof(GetSemester), new { id = semester.Id }, semester);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSemester(int id, [FromBody] UpdateSemesterRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _semesterService.UpdateSemesterAsync(id, userId, request);
            if (!success)
            {
                return NotFound(new { message = "Semester not found" });
            }
            return Ok(new { message = "Semester updated successfully" });
        }

        [HttpPost("{id}/set-current")]
        public async Task<ActionResult> SetCurrentSemester(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _semesterService.SetCurrentSemesterAsync(id, userId);
            return Ok(new { message = "Current semester set successfully" });
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteSemester(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _semesterService.CompleteSemesterAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Semester not found" });
            }
            return Ok(new { message = "Semester completed successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSemester(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _semesterService.DeleteSemesterAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Semester not found" });
            }
            return Ok(new { message = "Semester deleted successfully" });
        }

        [HttpGet("{id}/credits")]
        public async Task<ActionResult> GetSemesterCredits(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var credits = await _semesterService.GetTotalCreditsAsync(id, userId);
            return Ok(new { totalCredits = credits });
        }
    }
}