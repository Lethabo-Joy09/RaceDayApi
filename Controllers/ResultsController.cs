using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi.Data;
using RaceDayApi.Helpers;
using RaceDayApi.Models;
using RaceDayApi.Models.DTOs;

namespace RaceDayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/results/my  (Participant only)
        [HttpGet("my")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Participant).ThenInclude(p => p.User)
                .Include(r => r.Enrolment).ThenInclude(e => e.EventCategory).ThenInclude(ec => ec.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e.EventCategory).ThenInclude(ec => ec.Category)
                .Where(r => r.Enrolment.ParticipantID == participant.ParticipantID)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultID,
                    EnrolmentId = r.EnrolmentID,
                    ParticipantName = r.Enrolment.Participant.User.FullName,
                    EventName = r.Enrolment.EventCategory.Event.EventName,
                    CategoryName = r.Enrolment.EventCategory.Category.CategoryName,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    Status = r.Status,
                    RecordedAt = r.RecordedAt
                }).ToListAsync();

            return Ok(results);
        }

        // GET: api/results/event/5  (all roles)
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Participant).ThenInclude(p => p.User)
                .Include(r => r.Enrolment).ThenInclude(e => e.EventCategory).ThenInclude(ec => ec.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e.EventCategory).ThenInclude(ec => ec.Category)
                .Where(r => r.Enrolment.EventCategory.EventID == eventId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultID,
                    EnrolmentId = r.EnrolmentID,
                    ParticipantName = r.Enrolment.Participant.User.FullName,
                    EventName = r.Enrolment.EventCategory.Event.EventName,
                    CategoryName = r.Enrolment.EventCategory.Category.CategoryName,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    Status = r.Status,
                    RecordedAt = r.RecordedAt
                }).ToListAsync();

            return Ok(results);
        }

        // POST: api/results/enrolment/5  (Organiser only)
        [HttpPost("enrolment/{enrolmentId}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create(int enrolmentId, [FromBody] CreateResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var validStatuses = new[] { "Completed", "Disqualified", "DidNotFinish" };
            if (!validStatuses.Contains(dto.Status))
                return BadRequest(new { message = "Invalid status." });

            var enrolment = await _context.Enrolments.FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId);
            if (enrolment == null) return NotFound(new { message = "Enrolment not found." });

            var exists = await _context.Results.AnyAsync(r => r.EnrolmentID == enrolmentId);
            if (exists) return Conflict(new { message = "Result already captured. Use PUT to update." });

            var result = new Result
            {
                EnrolmentID = enrolmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position,
                Status = dto.Status,
                RecordedAt = DateTime.UtcNow
            };

            _context.Results.Add(result);

            // Mark enrolment as completed
            if (dto.Status == "Completed")
                enrolment.Status = "Completed";

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result captured.", resultId = result.ResultID });
        }

        // PUT: api/results/5  (Organiser only)
        [HttpPut("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResultDto dto)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound(new { message = "Result not found." });

            if (dto.FinishTime.HasValue) result.FinishTime = dto.FinishTime.Value;
            if (dto.Position.HasValue) result.Position = dto.Position.Value;
            if (!string.IsNullOrEmpty(dto.Status)) result.Status = dto.Status;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Result updated." });
        }
    }
}