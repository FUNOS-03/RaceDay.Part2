
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
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public ResultsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/results/my
    // Participants can view their own results.
    [HttpGet("my")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyResults()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var results = await _context.RaceResults
            .AsNoTracking()
            .Where(r => r.Enrolment!.UserId == userId.Value)
            .Include(r => r.Enrolment!)
                .ThenInclude(e => e.Event!)
                    .ThenInclude(e => e.EventCategory)
            .OrderBy(r => r.FinishingPosition)
            .Select(r => new
            {
                r.Id,
                EventName = r.Enrolment!.Event!.Name,
                Category = r.Enrolment.Event!.EventCategory!.Name,
                r.FinishingPosition,
                r.FinishTime,
                r.RecordedAt
            })
            .ToListAsync();

        return Ok(results);
    }

    // GET: api/results
    // Organisers can view all recorded results.
    [HttpGet]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetAllResults()
    {
        var results = await _context.RaceResults
            .AsNoTracking()
            .Include(r => r.Enrolment!)
                .ThenInclude(e => e.User)
            .Include(r => r.Enrolment!)
                .ThenInclude(e => e.Event)
            .OrderBy(r => r.Enrolment!.EventId)
            .ThenBy(r => r.FinishingPosition)
            .Select(r => new
            {
                r.Id,
                ParticipantName = r.Enrolment!.User!.FullName,
                ParticipantEmail = r.Enrolment.User!.Email,
                EventName = r.Enrolment.Event!.Name,
                r.FinishingPosition,
                r.FinishTime,
                r.RecordedAt
            })
            .ToListAsync();

        return Ok(results);
    }

    // POST: api/results
    // Organisers record results for existing enrolments.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> RecordResult(
        [FromBody] RecordResultRequest request)
    {
        if (request.FinishTime <= TimeSpan.Zero)
        {
            return BadRequest(new
            {
                message = "Finish time must be greater than zero."
            });
        }

        var enrolment = await _context.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.Id == request.EnrolmentId);

        if (enrolment == null)
        {
            return NotFound(new
            {
                message = "Enrolment not found."
            });
        }

        var existingResult = await _context.RaceResults
            .AnyAsync(r => r.EnrolmentId == request.EnrolmentId);

        if (existingResult)
        {
            return Conflict(new
            {
                message = "A result has already been recorded for this enrolment."
            });
        }

        var positionTaken = await _context.RaceResults
            .AnyAsync(r =>
                r.Enrolment!.EventId == enrolment.EventId &&
                r.FinishingPosition == request.FinishingPosition);

        if (positionTaken)
        {
            return Conflict(new
            {
                message = "This finishing position is already assigned in this event."
            });
        }

        var result = new RaceResult
        {
            EnrolmentId = request.EnrolmentId,
            FinishingPosition = request.FinishingPosition,
            FinishTime = request.FinishTime,
            RecordedAt = DateTime.UtcNow
        };

        _context.RaceResults.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAllResults),
            new { },
            new
            {
                result.Id,
                result.EnrolmentId,
                result.FinishingPosition,
                result.FinishTime,
                result.RecordedAt
            });
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

public class RecordResultRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int EnrolmentId { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Range(1, 100000)]
    public int FinishingPosition { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public TimeSpan FinishTime { get; set; }
}
