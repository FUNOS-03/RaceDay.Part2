

using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class Race
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Race name is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Race name must be between 3 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Race location is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Race location must be between 2 and 100 characters.")]
    public string Location { get; set; } = string.Empty;

    [Range(typeof(DateTime), "2000-01-01", "2100-12-31",
        ErrorMessage = "Race date must be between 1 January 2000 and 31 December 2100.")]
    public DateTime RaceDate { get; set; }
}
