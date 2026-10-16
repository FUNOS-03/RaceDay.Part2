
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Event name must be between 3 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500,
        ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Event location is required.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "Location must be between 2 and 150 characters.")]
    public string Location { get; set; } = string.Empty;

    [Range(typeof(DateTime), "2000-01-01", "2100-12-31",
        ErrorMessage = "Event date must be between 1 January 2000 and 31 December 2100.")]
    public DateTime EventDate { get; set; }

    [Range(1, 100000,
        ErrorMessage = "Maximum participants must be between 1 and 100000.")]
    public int MaximumParticipants { get; set; }

    [Required]
    public int EventCategoryId { get; set; }

    public EventCategory? EventCategory { get; set; }
}
