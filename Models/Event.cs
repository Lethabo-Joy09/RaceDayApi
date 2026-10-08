using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganiserID { get; set; }

        [Required, MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        // New required fields for Part 2
        [Required]
        public decimal Distance { get; set; }

        [Required, MaxLength(20)]
        public string EventType { get; set; } = string.Empty; // "Run", "Walk", "Cycle"

        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // Open, Closed, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(OrganiserID))]
        public Organiser Organiser { get; set; } = null!;

        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    }
}