using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        public int? Position { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Completed"; // Completed, Disqualified, DidNotFinish

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(EnrolmentID))]
        public Enrolment Enrolment { get; set; } = null!;
    }
}