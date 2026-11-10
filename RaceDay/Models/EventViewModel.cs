using System.ComponentModel.DataAnnotations;

namespace RaceDay.web.Models;

public class EventViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Event location is required.")]
    [StringLength(150, MinimumLength = 2)]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event date is required.")]
    public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);

    [Range(1, 100000,
        ErrorMessage = "Maximum participants must be at least 1.")]
    public int MaximumParticipants { get; set; } = 100;

    [Required(ErrorMessage = "Please select an event category.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid event category.")]
    public int EventCategoryId { get; set; }

    public List<EventCategoryOption> Categories { get; set; } = new();
}

public class EventCategoryOption
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}