
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly BlobStorageService _blobStorageService;

    public FilesController(BlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    // GET: api/files/profiles/example.jpg
    // GET: api/files/event-banners/example.jpg
    [HttpGet("{*blobName}")]
    public async Task<IActionResult> GetFile(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName) ||
            !(blobName.StartsWith("profiles/") ||
              blobName.StartsWith("event-banners/")))
        {
            return BadRequest(new
            {
                message = "Invalid file location."
            });
        }

        try
        {
            var file = await _blobStorageService.DownloadAsync(blobName);

            if (file == null)
            {
                return NotFound(new
                {
                    message = "File not found."
                });
            }

            return File(
                file.Content,
                file.Details.ContentType ?? "application/octet-stream");
        }
        catch
        {
            return StatusCode(500, new
            {
                message = "Unable to retrieve the stored file."
            });
        }
    }
}

