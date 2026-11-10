
namespace RaceDay.web.Models;

public class EnrolmentViewModel
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string EventName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime EnrolledAt { get; set; }

    public string Status { get; set; } = string.Empty;
}

