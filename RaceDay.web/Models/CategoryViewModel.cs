using System.ComponentModel.DataAnnotations;

namespace RaceDay.web.Models;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Category name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}