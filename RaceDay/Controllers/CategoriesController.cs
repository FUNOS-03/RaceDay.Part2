
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public CategoriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/categories
    // Any authenticated user can view categories.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventCategory>>> GetCategories()
    {
        var categories = await _context.EventCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/categories/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventCategory>> GetCategory(int id)
    {
        var category = await _context.EventCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        return Ok(category);
    }

    // POST: api/categories
    // Only Organisers can create categories.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<EventCategory>> CreateCategory(
        EventCategory category)
    {
        var name = category.Name.Trim();

        var exists = await _context.EventCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());

        if (exists)
        {
            return Conflict(new
            {
                message = "A category with this name already exists."
            });
        }

        category.Name = name;
        category.Description = category.Description?.Trim();

        _context.EventCategories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Id },
            category);
    }

    // PUT: api/categories/1
    // Only Organisers can update categories.
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<EventCategory>> UpdateCategory(
        int id,
        EventCategory updatedCategory)
    {
        var category = await _context.EventCategories.FindAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        var name = updatedCategory.Name.Trim();

        var duplicateExists = await _context.EventCategories
            .AnyAsync(c =>
                c.Id != id &&
                c.Name.ToLower() == name.ToLower());

        if (duplicateExists)
        {
            return Conflict(new
            {
                message = "Another category with this name already exists."
            });
        }

        category.Name = name;
        category.Description = updatedCategory.Description?.Trim();

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    // DELETE: api/categories/1
    // Only Organisers can delete categories.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.EventCategories.FindAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        var hasEvents = await _context.Events
            .AnyAsync(e => e.EventCategoryId == id);

        if (hasEvents)
        {
            return Conflict(new
            {
                message = "Cannot delete a category that has events."
            });
        }

        _context.EventCategories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
