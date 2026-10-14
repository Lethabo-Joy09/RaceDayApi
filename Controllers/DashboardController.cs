using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi.Data;
using RaceDayApi.Helpers;

namespace RaceDayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize]
    public class DashboardController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public DashboardController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/organiser  (Organiser only)
        [HttpGet("organiser")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> OrganiserDashboard()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var eventIds = await _context.Events
                .Where(e => e.OrganiserID == organiser.OrganiserID)
                .Select(e => e.EventID)
                .ToListAsync();

            var totalEvents = eventIds.Count;

            var totalEnrolments = await _context.Enrolments
                .Where(e => eventIds.Contains(e.EventCategory.EventID))
                .CountAsync();

            var totalResults = await _context.Results
                .Where(r => eventIds.Contains(r.Enrolment.EventCategory.EventID))
                .CountAsync();

            var upcomingEvents = await _context.Events
                .Where(e => e.OrganiserID == organiser.OrganiserID && e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .Take(5)
                .Select(e => new { e.EventID, e.EventName, e.EventDate, e.Location })
                .ToListAsync();

            return Ok(new
            {
                totalEvents,
                totalEnrolments,
                totalResults,
                upcomingEvents
            });
        }

        // GET: api/dashboard/participant  (Participant only)
        [HttpGet("participant")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> ParticipantDashboard()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var totalEnrolments = await _context.Enrolments
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .CountAsync();

            var totalResults = await _context.Results
                .Where(r => r.Enrolment.ParticipantID == participant.ParticipantID)
                .CountAsync();

            var upcomingEvents = await _context.Enrolments
                .Where(e => e.ParticipantID == participant.ParticipantID
                            && e.EventCategory.Event.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventCategory.Event.EventDate)
                .Select(e => new
                {
                    EventName = e.EventCategory.Event.EventName,
                    EventDate = e.EventCategory.Event.EventDate,
                    Category = e.EventCategory.Category.CategoryName
                })
                .ToListAsync();

            return Ok(new
            {
                totalEnrolments,
                totalResults,
                upcomingEvents
            });
        }
    }
}