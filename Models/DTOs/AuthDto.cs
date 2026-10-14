using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    // ---------- Register ----------
    public class RegisterDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        // Organiser-specific (optional)
        [MaxLength(100)]
        public string? CompanyName { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        // Participant-specific (optional)
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }
    }

    // ---------- Login ----------
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ---------- Response ----------
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}