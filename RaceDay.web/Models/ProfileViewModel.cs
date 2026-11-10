using System.ComponentModel.DataAnnotations;

namespace RaceDay.web.Models;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; }

    public IFormFile? ProfilePicture { get; set; }
}
