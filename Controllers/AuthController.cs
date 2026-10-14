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
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate role
            if (dto.Role != "Organiser" && dto.Role != "Participant")
                return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

            // Check if email exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return Conflict(new { message = "Email already registered." });

            // Create user
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create role-specific profile
            if (dto.Role == "Organiser")
            {
                var organiser = new Organiser
                {
                    UserID = user.UserID,
                    CompanyName = dto.CompanyName,
                    ContactPhone = dto.ContactPhone
                };
                _context.Organisers.Add(organiser);
            }
            else
            {
                var participant = new Participant
                {
                    UserID = user.UserID,
                    DateOfBirth = dto.DateOfBirth,
                    EmergencyContact = dto.EmergencyContact,
                    EmergencyPhone = dto.EmergencyPhone
                };
                _context.Participants.Add(participant);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new AuthResponseDto
            {
                UserId = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Message = "Registration successful."
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            // Store user info in session
            HttpContext.Session.SetInt32(SessionKeys.UserId, user.UserID);
            HttpContext.Session.SetString(SessionKeys.UserRole, user.Role);
            HttpContext.Session.SetString(SessionKeys.UserEmail, user.Email);
            HttpContext.Session.SetString(SessionKeys.UserName, user.FullName);

            return Ok(new AuthResponseDto
            {
                UserId = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Message = "Login successful."
            });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }

        // GET: api/auth/me
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
                return Unauthorized(new { message = "Not logged in." });

            return Ok(new
            {
                UserId = userId,
                Role = HttpContext.Session.GetString(SessionKeys.UserRole),
                Email = HttpContext.Session.GetString(SessionKeys.UserEmail),
                FullName = HttpContext.Session.GetString(SessionKeys.UserName)
            });
        }
    }
}