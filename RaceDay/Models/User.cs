
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // Store a password hash, never a plain-text password.
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Organiser|Participant)$",
        ErrorMessage = "Role must be Organiser or Participant.")]
    public string Role { get; set; } = "Participant";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
