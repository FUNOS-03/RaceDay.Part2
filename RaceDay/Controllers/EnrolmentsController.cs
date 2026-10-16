
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EnrolmentsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/enrolments/my
    // A user can view only their own enrolments.
    [HttpGet("my")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var enrolments = await _context.Enrolments
            .AsNoTracking()
            .Where(e => e.UserId == userId.Value)
            .Include(e => e.Event)
                .ThenInclude(e => e!.EventCategory)
            .OrderByDescending(e => e.EnrolledAt)
            .Select(e => new
            {
                e.Id,
                e.EventId,
                EventName = e.Event!.Name,
                e.Event.Location,
                e.Event.EventDate,
                Category = e.Event.EventCategory!.Name,
                e.EnrolledAt,
                e.Status
            })
            .ToListAsync();

        return Ok(enrolments);
    }

    // POST: api/enrolments
    // A Participant enrols themselves in an event.
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> EnrolInEvent(
        [FromBody] EnrolmentRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var raceEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId);

        if (raceEvent == null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        if (raceEvent.EventDate <= DateTime.UtcNow)
        {
            return BadRequest(new
            {
                message = "You cannot enrol in an event that has already started."
            });
        }

        var alreadyEnrolled = await _context.Enrolments
            .AnyAsync(e =>
                e.UserId == userId.Value &&
                e.EventId == request.EventId);

        if (alreadyEnrolled)
        {
            return Conflict(new
            {
                message = "You are already enrolled in this event."
            });
        }

        var enrolmentCount = await _context.Enrolments
            .CountAsync(e =>
                e.EventId == request.EventId &&
                e.Status == "Enrolled");

        if (enrolmentCount >= raceEvent.MaximumParticipants)
        {
            return Conflict(new
            {
                message = "This event has reached its participant limit."
            });
        }

        var enrolment = new Enrolment
        {
            UserId = userId.Value,
            EventId = request.EventId,
            EnrolledAt = DateTime.UtcNow,
            Status = "Enrolled"
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMyEnrolments),
            new { },
            new
            {
                enrolment.Id,
                enrolment.UserId,
                enrolment.EventId,
                enrolment.EnrolledAt,
                enrolment.Status
            });
    }

    // DELETE: api/enrolments/1
    // Participants can cancel only their own enrolments.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var enrolment = await _context.Enrolments
            .FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.UserId == userId.Value);

        if (enrolment == null)
        {
            return NotFound(new
            {
                message = "Enrolment not found."
            });
        }

        if (await _context.RaceResults
            .AnyAsync(r => r.EnrolmentId == enrolment.Id))
        {
            return Conflict(new
            {
                message = "An enrolment with a recorded result cannot be cancelled."
            });
        }

        _context.Enrolments.Remove(enrolment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}

public class EnrolmentRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Range(
        1, int.MaxValue,
        ErrorMessage = "A valid event ID is required.")]
    public int EventId { get; set; }
}

