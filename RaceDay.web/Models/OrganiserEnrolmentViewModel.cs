
namespace RaceDay.web.Models;

public class OrganiserEnrolmentViewModel
{
    public int Id { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string ParticipantEmail { get; set; } = string.Empty;

    public string EventName { get; set; } = string.Empty;

    public string EventLocation { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime EnrolledAt { get; set; }

    public string Status { get; set; } = string.Empty;
}
