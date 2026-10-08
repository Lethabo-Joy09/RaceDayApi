using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventCategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Withdrawn, Completed

        // Navigation
        [ForeignKey(nameof(ParticipantID))]
        public Participant Participant { get; set; } = null!;

        [ForeignKey(nameof(EventCategoryID))]
        public EventCategory EventCategory { get; set; } = null!;

        public Result? Result { get; set; }
    }
}