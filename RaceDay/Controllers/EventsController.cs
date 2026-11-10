
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _context;
    private readonly BlobStorageService _blobStorageService;

    private const long MaxBannerSize = 5 * 1024 * 1024;

    private static readonly string[] AllowedImageTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public EventsController(
        RaceDayContext context,
        BlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }

    // GET: api/events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/events/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        var raceEvent = await _context.Events
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (raceEvent is null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        return Ok(raceEvent);
    }

    // POST: api/events
    // Only Organisers can create events.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxBannerSize + 1024 * 1024)]
    public async Task<ActionResult<Event>> CreateEvent(
        [FromForm] Event newEvent,
        [FromForm] IFormFile? bannerImage)
    {
        var validationErrors = ValidateEvent(newEvent);

        if (validationErrors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(validationErrors));
        }

        var categoryExists = await _context.EventCategories
            .AnyAsync(c => c.Id == newEvent.EventCategoryId);

        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The selected event category does not exist."
            });
        }

        if (bannerImage != null)
        {
            var imageError = ValidateBanner(bannerImage);

            if (imageError != null)
            {
                return BadRequest(new
                {
                    message = imageError
                });
            }

            await using var stream = bannerImage.OpenReadStream();

            newEvent.BannerUrl =
                await _blobStorageService.UploadEventBannerAsync(
                    stream,
                    bannerImage.FileName,
                    bannerImage.ContentType);
        }

        newEvent.Name = newEvent.Name.Trim();
        newEvent.Description = newEvent.Description?.Trim();
        newEvent.Location = newEvent.Location.Trim();

        newEvent.EventCategory = null;

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvent),
            new { id = newEvent.Id },
            newEvent);
    }

    // PUT: api/events/1
    // Only Organisers can update events.
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxBannerSize + 1024 * 1024)]
    public async Task<ActionResult<Event>> UpdateEvent(
        int id,
        [FromForm] Event updatedEvent,
        [FromForm] IFormFile? bannerImage)
    {
        var existingEvent = await _context.Events.FindAsync(id);

        if (existingEvent is null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        var validationErrors = ValidateEvent(updatedEvent);

        if (validationErrors.Count > 0)
        {
            return BadRequest(new ValidationProblemDetails(validationErrors));
        }

        var categoryExists = await _context.EventCategories
            .AnyAsync(c => c.Id == updatedEvent.EventCategoryId);

        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The selected event category does not exist."
            });
        }

        if (bannerImage != null)
        {
            var imageError = ValidateBanner(bannerImage);

            if (imageError != null)
            {
                return BadRequest(new
                {
                    message = imageError
                });
            }

            await using var stream = bannerImage.OpenReadStream();

            existingEvent.BannerUrl =
                await _blobStorageService.UploadEventBannerAsync(
                    stream,
                    bannerImage.FileName,
                    bannerImage.ContentType);
        }

        existingEvent.Name = updatedEvent.Name.Trim();
        existingEvent.Description = updatedEvent.Description?.Trim();
        existingEvent.Location = updatedEvent.Location.Trim();
        existingEvent.EventDate = updatedEvent.EventDate;
        existingEvent.MaximumParticipants =
            updatedEvent.MaximumParticipants;
        existingEvent.EventCategoryId =
            updatedEvent.EventCategoryId;

        await _context.SaveChangesAsync();

        return Ok(await _context.Events
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .FirstAsync(e => e.Id == id));
    }

    // DELETE: api/events/1
    // Only Organisers can delete events.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var raceEvent = await _context.Events.FindAsync(id);

        if (raceEvent is null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        _context.Events.Remove(raceEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidateBanner(IFormFile bannerImage)
    {
        if (bannerImage.Length == 0 ||
            bannerImage.Length > MaxBannerSize)
        {
            return "The banner image must be smaller than 5 MB.";
        }

        if (!AllowedImageTypes.Contains(
            bannerImage.ContentType.ToLowerInvariant()))
        {
            return "Only JPG, PNG and WEBP banner images are allowed.";
        }

        return null;
    }

    private static Dictionary<string, string[]> ValidateEvent(
        Event raceEvent)
    {
        var errors = new Dictionary<string, string[]>();

        var results =
            new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        var validationContext =
            new System.ComponentModel.DataAnnotations.ValidationContext(
                raceEvent);

        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            raceEvent,
            validationContext,
            results,
            validateAllProperties: true);

        foreach (var result in results)
        {
            foreach (var member in result.MemberNames.DefaultIfEmpty(""))
            {
                if (!errors.ContainsKey(member))
                {
                    errors[member] = Array.Empty<string>();
                }

                errors[member] = errors[member]
                    .Append(result.ErrorMessage ?? "Invalid value.")
                    .ToArray();
            }
        }

        return errors;
    }
}


