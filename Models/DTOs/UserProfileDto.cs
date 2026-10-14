using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Organiser-specific
        public string? CompanyName { get; set; }
        public string? ContactPhone { get; set; }

        // Participant-specific
        public DateTime? DateOfBirth { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
    }

    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [EmailAddress, MaxLength(255)]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }

        // Organiser-specific
        [MaxLength(100)]
        public string? CompanyName { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        // Participant-specific
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }
    }
}