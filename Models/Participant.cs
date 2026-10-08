using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }

        // Navigation
        [ForeignKey(nameof(UserID))]
        public User User { get; set; } = null!;

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}