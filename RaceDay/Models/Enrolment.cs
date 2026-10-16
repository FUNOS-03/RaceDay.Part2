
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class Enrolment
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }

    [Required]
    public int EventId { get; set; }

    public Event? Event { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [StringLength(20)]
    public string Status { get; set; } = "Enrolled";
}

