
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly RaceDayContext _context;
    private readonly BlobStorageService _blobStorageService;

    private const long MaxPictureSize = 5 * 1024 * 1024;

    public ProfileController(
        RaceDayContext context,
        BlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }

    // GET: api/profile/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "The user identity in the token is invalid."
            });
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User profile not found."
            });
        }

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.ProfilePictureUrl,
            user.CreatedAt
        });
    }

    // PUT: api/profile/me
    [HttpPut("me")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxPictureSize + 1024 * 1024)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromForm] UpdateProfileRequest request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "The user identity in the token is invalid."
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User profile not found."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            request.FullName.Trim().Length < 2 ||
            request.FullName.Trim().Length > 100)
        {
            return BadRequest(new
            {
                message = "Full name must be between 2 and 100 characters."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email) ||
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                .IsValid(request.Email.Trim()))
        {
            return BadRequest(new
            {
                message = "Enter a valid email address."
            });
        }

        var email = request.Email.Trim();

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email && u.Id != userId);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "This email address is already registered."
            });
        }

        if (request.ProfilePicture != null)
        {
            var picture = request.ProfilePicture;

            if (picture.Length == 0 || picture.Length > MaxPictureSize)
            {
                return BadRequest(new
                {
                    message = "Profile pictures must be smaller than 5 MB."
                });
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(picture.ContentType.ToLowerInvariant()))
            {
                return BadRequest(new
                {
                    message = "Only JPG, PNG and WEBP images are allowed."
                });
            }

            await using var stream = picture.OpenReadStream();

            user.ProfilePictureUrl =
                await _blobStorageService.UploadProfilePictureAsync(
                    stream,
                    picture.FileName,
                    picture.ContentType);
        }

        user.FullName = request.FullName.Trim();
        user.Email = email;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully.",
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.ProfilePictureUrl
        });
    }
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public IFormFile? ProfilePicture { get; set; }
}
