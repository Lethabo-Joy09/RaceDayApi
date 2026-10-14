using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    // ---------- Read ----------
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string OrganiserName { get; set; } = string.Empty;
        public List<EventCategoryDto> Categories { get; set; } = new();
    }

    public class EventCategoryDto
    {
        public int EventCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal DistanceKM { get; set; }
        public decimal Price { get; set; }
        public int? MaxParticipants { get; set; }
        public int EnrolmentCount { get; set; }
    }

    // ---------- Create ----------
    public class CreateEventDto
    {
        [Required, MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required, Range(0.1, 999.99)]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty; // "Run", "Walk", "Cycle"

        public string Status { get; set; } = "Open";

        public List<EventCategoryInputDto>? Categories { get; set; }
    }

    public class EventCategoryInputDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required, Range(0, 99999.99)]
        public decimal Price { get; set; }

        public int? MaxParticipants { get; set; }
    }

    // ---------- Update ----------
    public class UpdateEventDto
    {
        [MaxLength(100)]
        public string? EventName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? EventDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [Range(0.1, 999.99)]
        public decimal? Distance { get; set; }

        public string? EventType { get; set; }

        public string? Status { get; set; }
    }
}