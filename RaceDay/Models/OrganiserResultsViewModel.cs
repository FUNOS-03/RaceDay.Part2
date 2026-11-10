
using System.ComponentModel.DataAnnotations;

namespace RaceDay.web.Models;

public class OrganiserResultsViewModel
{
    public List<OrganiserEnrolmentViewModel> Enrolments { get; set; } = new();

    public List<RecordedRaceResultViewModel> Results { get; set; } = new();

    [Required(ErrorMessage = "Please select a participant enrolment.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid enrolment.")]
    public int EnrolmentId { get; set; }

    [Required(ErrorMessage = "Finishing position is required.")]
    [Range(1, 100000, ErrorMessage = "Enter a valid finishing position.")]
    public int FinishingPosition { get; set; }

    [Required(ErrorMessage = "Finish time is required.")]
    public string FinishTime { get; set; } = string.Empty;
}

public class RecordedRaceResultViewModel
{
    public int Id { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string ParticipantEmail { get; set; } = string.Empty;

    public string EventName { get; set; } = string.Empty;

    public int FinishingPosition { get; set; }

    public TimeSpan FinishTime { get; set; }

    public DateTime RecordedAt { get; set; }
}
