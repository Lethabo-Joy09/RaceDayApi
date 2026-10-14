using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    public class ResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeSpan FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }

    public class CreateResultDto
    {
        [Required]
        public TimeSpan FinishTime { get; set; }

        public int? Position { get; set; }

        public string Status { get; set; } = "Completed"; // Completed, Disqualified, DidNotFinish
    }

    public class UpdateResultDto
    {
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string? Status { get; set; }
    }
}