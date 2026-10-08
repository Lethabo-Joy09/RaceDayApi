using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi.Models
{
    public class EventCategory
    {
        [Key]
        public int EventCategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? MaxParticipants { get; set; }

        // Navigation
        [ForeignKey(nameof(EventID))]
        public Event Event { get; set; } = null!;

        [ForeignKey(nameof(CategoryID))]
        public Category Category { get; set; } = null!;

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}