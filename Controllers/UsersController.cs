using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi.Data;
using RaceDayApi.Helpers;
using RaceDayApi.Models.DTOs;

namespace RaceDayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize] // Any logged-in user
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null) return Unauthorized(new { message = "Not logged in." });

            var user = await _context.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return NotFound(new { message = "User not found." });

            var profile = new UserProfileDto
            {
                UserId = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyName = user.Organiser?.CompanyName,
                ContactPhone = user.Organiser?.ContactPhone,
                DateOfBirth = user.Participant?.DateOfBirth,
                EmergencyContact = user.Participant?.EmergencyContact,
                EmergencyPhone = user.Participant?.EmergencyPhone
            };

            return Ok(profile);
        }

        // PUT: api/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null) return Unauthorized(new { message = "Not logged in." });

            var user = await _context.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return NotFound(new { message = "User not found." });

            // Update email if provided and unique
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserID != user.UserID);
                if (emailTaken) return Conflict(new { message = "Email already in use." });
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.NewPassword))
                user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);

            // Role-specific updates
            if (user.Role == "Organiser" && user.Organiser != null)
            {
                if (dto.CompanyName != null) user.Organiser.CompanyName = dto.CompanyName;
                if (dto.ContactPhone != null) user.Organiser.ContactPhone = dto.ContactPhone;
            }
            else if (user.Role == "Participant" && user.Participant != null)
            {
                if (dto.DateOfBirth.HasValue) user.Participant.DateOfBirth = dto.DateOfBirth;
                if (dto.EmergencyContact != null) user.Participant.EmergencyContact = dto.EmergencyContact;
                if (dto.EmergencyPhone != null) user.Participant.EmergencyPhone = dto.EmergencyPhone;
            }

            await _context.SaveChangesAsync();

            // Refresh session name if changed
            HttpContext.Session.SetString(SessionKeys.UserName, user.FullName);
            HttpContext.Session.SetString(SessionKeys.UserEmail, user.Email);

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}