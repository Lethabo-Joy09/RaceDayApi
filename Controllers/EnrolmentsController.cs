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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/enrolments  (Organiser only — all enrolments)
        [HttpGet]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Enrolments
                .Include(e => e.Participant).ThenInclude(p => p.User)
                .Include(e => e.EventCategory).ThenInclude(ec => ec.Event)
                .Include(e => e.EventCategory).ThenInclude(ec => ec.Category)
                .Include(e => e.Result)
                .Select(e => MapToDto(e))
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/enrolments/my  (Participant only)
        [HttpGet("my")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMine()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var list = await _context.Enrolments
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .Include(e => e.Participant).ThenInclude(p => p.User)
                .Include(e => e.EventCategory).ThenInclude(ec => ec.Event)
                .Include(e => e.EventCategory).ThenInclude(ec => ec.Category)
                .Include(e => e.Result)
                .Select(e => MapToDto(e))
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/enrolments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _context.Enrolments
                .Include(x => x.Participant).ThenInclude(p => p.User)
                .Include(x => x.EventCategory).ThenInclude(ec => ec.Event)
                .Include(x => x.EventCategory).ThenInclude(ec => ec.Category)
                .Include(x => x.Result)
                .FirstOrDefaultAsync(x => x.EnrolmentID == id);

            if (e == null) return NotFound(new { message = "Enrolment not found." });

            // Only owner participant or organiser can view
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var role = HttpContext.Session.GetString(SessionKeys.UserRole);

            if (role == "Participant" && e.Participant.UserID != userId)
                return Forbid();

            return Ok(MapToDto(e));
        }

        // POST: api/enrolments  (Participant only)
        [HttpPost]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> Create([FromBody] CreateEnrolmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var eventCat = await _context.EventCategories
                .Include(ec => ec.Event)
                .Include(ec => ec.Enrolments)
                .FirstOrDefaultAsync(ec => ec.EventCategoryID == dto.EventCategoryId);

            if (eventCat == null) return NotFound(new { message = "Event category not found." });

            if (eventCat.Event.Status != "Open")
                return BadRequest(new { message = "Event is not open for enrolment." });

            if (eventCat.MaxParticipants.HasValue && eventCat.Enrolments.Count >= eventCat.MaxParticipants.Value)
                return Conflict(new { message = "Event category is full." });

            // Check duplicate
            var exists = await _context.Enrolments.AnyAsync(e =>
                e.ParticipantID == participant.ParticipantID &&
                e.EventCategoryID == dto.EventCategoryId);

            if (exists) return Conflict(new { message = "Already enrolled in this category." });

            var enrolment = new Enrolment
            {
                ParticipantID = participant.ParticipantID,
                EventCategoryID = dto.EventCategoryId,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = enrolment.EnrolmentID }, new { message = "Enrolment created.", enrolmentId = enrolment.EnrolmentID });
        }

        // PUT: api/enrolments/5  (Organiser only)
        [HttpPut("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEnrolmentStatusDto dto)
        {
            var validStatuses = new[] { "Pending", "Confirmed", "Withdrawn", "Completed" };
            if (!validStatuses.Contains(dto.Status))
                return BadRequest(new { message = "Invalid status." });

            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null) return NotFound(new { message = "Enrolment not found." });

            enrolment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment status updated." });
        }

        // DELETE: api/enrolments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var role = HttpContext.Session.GetString(SessionKeys.UserRole);

            var enrolment = await _context.Enrolments
                .Include(e => e.Participant)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null) return NotFound(new { message = "Enrolment not found." });

            // Participant can only withdraw own; organiser can delete any
            if (role == "Participant" && enrolment.Participant.UserID != userId)
                return Forbid();

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static EnrolmentDto MapToDto(Enrolment e) => new EnrolmentDto
        {
            EnrolmentId = e.EnrolmentID,
            ParticipantId = e.ParticipantID,
            ParticipantName = e.Participant.User.FullName,
            ParticipantEmail = e.Participant.User.Email,
            EventId = e.EventCategory.EventID,
            EventName = e.EventCategory.Event.EventName,
            EventDate = e.EventCategory.Event.EventDate,
            CategoryName = e.EventCategory.Category.CategoryName,
            Price = e.EventCategory.Price,
            EnrolmentDate = e.EnrolmentDate,
            Status = e.Status,
            Result = e.Result == null ? null : new ResultDto
            {
                ResultId = e.Result.ResultID,
                EnrolmentId = e.Result.EnrolmentID,
                FinishTime = e.Result.FinishTime,
                Position = e.Result.Position,
                Status = e.Result.Status,
                RecordedAt = e.Result.RecordedAt
            }
        };
    }
}