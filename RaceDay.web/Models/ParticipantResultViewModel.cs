
namespace RaceDay.web.Models;

public class ParticipantResultViewModel
{
    public int Id { get; set; }

    public string EventName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int FinishingPosition { get; set; }

    public TimeSpan FinishTime { get; set; }

    public DateTime RecordedAt { get; set; }
}

