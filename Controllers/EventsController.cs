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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? type)
        {
            var query = _context.Events
                .Include(e => e.Organiser).ThenInclude(o => o.User)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(e => e.EventType == type);

            var events = await query.Select(e => new EventDto
            {
                EventId = e.EventID,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                Status = e.Status,
                OrganiserName = e.Organiser.User.FullName,
                Categories = e.EventCategories.Select(ec => new EventCategoryDto
                {
                    EventCategoryId = ec.EventCategoryID,
                    CategoryId = ec.CategoryID,
                    CategoryName = ec.Category.CategoryName,
                    DistanceKM = ec.Category.DistanceKM,
                    Price = ec.Price,
                    MaxParticipants = ec.MaxParticipants,
                    EnrolmentCount = ec.Enrolments.Count
                }).ToList()
            }).ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _context.Events
                .Include(x => x.Organiser).ThenInclude(o => o.User)
                .Include(x => x.EventCategories).ThenInclude(ec => ec.Category)
                .Include(x => x.EventCategories).ThenInclude(ec => ec.Enrolments)
                .FirstOrDefaultAsync(x => x.EventID == id);

            if (e == null) return NotFound(new { message = "Event not found." });

            return Ok(new EventDto
            {
                EventId = e.EventID,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                Status = e.Status,
                OrganiserName = e.Organiser.User.FullName,
                Categories = e.EventCategories.Select(ec => new EventCategoryDto
                {
                    EventCategoryId = ec.EventCategoryID,
                    CategoryId = ec.CategoryID,
                    CategoryName = ec.Category.CategoryName,
                    DistanceKM = ec.Category.DistanceKM,
                    Price = ec.Price,
                    MaxParticipants = ec.MaxParticipants,
                    EnrolmentCount = ec.Enrolments.Count
                }).ToList()
            });
        }

        // POST: api/events
        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.EventType != "Run" && dto.EventType != "Walk" && dto.EventType != "Cycle")
                return BadRequest(new { message = "EventType must be 'Run', 'Walk', or 'Cycle'." });

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var newEvent = new Event
            {
                OrganiserID = organiser.OrganiserID,
                EventName = dto.EventName,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            // Add categories if provided
            if (dto.Categories != null && dto.Categories.Any())
            {
                foreach (var cat in dto.Categories)
                {
                    _context.EventCategories.Add(new EventCategory
                    {
                        EventID = newEvent.EventID,
                        CategoryID = cat.CategoryId,
                        Price = cat.Price,
                        MaxParticipants = cat.MaxParticipants
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = newEvent.EventID }, new { message = "Event created.", eventId = newEvent.EventID });
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.EventID == id);
            if (eventItem == null) return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != organiser.OrganiserID)
                return Forbid();

            if (!string.IsNullOrEmpty(dto.EventName)) eventItem.EventName = dto.EventName;
            if (dto.Description != null) eventItem.Description = dto.Description;
            if (dto.EventDate.HasValue) eventItem.EventDate = dto.EventDate.Value;
            if (!string.IsNullOrEmpty(dto.Location)) eventItem.Location = dto.Location;
            if (dto.Distance.HasValue) eventItem.Distance = dto.Distance.Value;
            if (!string.IsNullOrEmpty(dto.EventType)) eventItem.EventType = dto.EventType;
            if (!string.IsNullOrEmpty(dto.Status)) eventItem.Status = dto.Status;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Event updated." });
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.EventID == id);
            if (eventItem == null) return NotFound(new { message = "Event not found." });

            if (eventItem.OrganiserID != organiser.OrganiserID)
                return Forbid();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}