using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public ResultDto? Result { get; set; }
    }

    public class CreateEnrolmentDto
    {
        [Required]
        public int EventCategoryId { get; set; }
    }

    public class UpdateEnrolmentStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty; // Pending, Confirmed, Withdrawn, Completed
    }
}