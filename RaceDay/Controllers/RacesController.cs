
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RacesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public RacesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/races
    // Any authenticated user can view all races.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        var races = await _context.Races.ToListAsync();
        return Ok(races);
    }

    // GET: api/races/1
    // Any authenticated user can view a race.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Race>> GetRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race is null)
        {
            return NotFound();
        }

        return Ok(race);
    }

    // POST: api/races
    // Only Organisers can create races.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        var validationErrors = ValidateRace(race);

        if (validationErrors.Count > 0)
        {
            return BadRequest(
                new ValidationProblemDetails(validationErrors));
        }

        _context.Races.Add(race);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRace),
            new { id = race.Id },
            race);
    }

    // PUT: api/races/1
    // Only Organisers can update races.
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<Race>> UpdateRace(
        int id,
        Race updatedRace)
    {
        var race = await _context.Races.FindAsync(id);

        if (race is null)
        {
            return NotFound();
        }

        var validationErrors = ValidateRace(updatedRace);

        if (validationErrors.Count > 0)
        {
            return BadRequest(
                new ValidationProblemDetails(validationErrors));
        }

        race.Name = updatedRace.Name;
        race.Location = updatedRace.Location;
        race.RaceDate = updatedRace.RaceDate;

        await _context.SaveChangesAsync();

        return Ok(race);
    }

    // DELETE: api/races/1
    // Only Organisers can delete races.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race is null)
        {
            return NotFound();
        }

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static Dictionary<string, string[]> ValidateRace(Race race)
    {
        var errors = new Dictionary<string, string[]>();
        var results = new List<ValidationResult>();
        var validationContext = new ValidationContext(race);

        Validator.TryValidateObject(
            race,
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
