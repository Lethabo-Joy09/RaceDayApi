using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }
}