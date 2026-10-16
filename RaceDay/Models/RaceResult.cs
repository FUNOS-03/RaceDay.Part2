
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class RaceResult
{
    public int Id { get; set; }

    [Required]
    public int EnrolmentId { get; set; }

    public Enrolment? Enrolment { get; set; }

    [Range(1, 100000,
        ErrorMessage = "Finishing position must be greater than zero.")]
    public int FinishingPosition { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}

