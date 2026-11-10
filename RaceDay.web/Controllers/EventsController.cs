
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace RaceDay.web.Controllers;

public class EventsController : Controller
{
    private readonly RaceDayApiService _apiService;
    private readonly IHttpClientFactory _httpClientFactory;

    public EventsController(
        RaceDayApiService apiService,
        IHttpClientFactory httpClientFactory)
    {
        _apiService = apiService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/events");

            return View(events ?? new List<EventViewModel>());
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load events.";
            return View(new List<EventViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsOrganiser())
            return RedirectToAction("Index", "Home");

        var model = new EventViewModel();
        await LoadCategories(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View(model);
        }

        try
        {
            using var content = CreateEventContent(model);

            var client = CreateAuthenticatedClient();

            var response = await client.PostAsync(
                "api/events", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Event created successfully.";

                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                "",
                "Unable to create event. " + error);
        }
        catch
        {
            ModelState.AddModelError(
                "",
                "Unable to connect to the RaceDay API.");
        }

        await LoadCategories(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Index", "Home");

        try
        {
            var model = await _apiService.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (model == null)
                return NotFound();

            await LoadCategories(model);
            return View(model);
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load the event.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View(model);
        }

        try
        {
            using var content = CreateEventContent(model);

            var client = CreateAuthenticatedClient();

            var response = await client.PutAsync(
                $"api/events/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Event updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                "",
                "Unable to update event. " + error);
        }
        catch
        {
            ModelState.AddModelError(
                "",
                "Unable to connect to the RaceDay API.");
        }

        await LoadCategories(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Index", "Home");

        try
        {
            var response = await _apiService.DeleteAsync(
                $"api/events/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Event deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Unable to delete the event.";
            }
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to connect to the API.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Banner(string blobName)
    {
        if (!IsLoggedIn())
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(blobName) ||
            !blobName.StartsWith(
                "event-banners/",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        try
        {
            var client = CreateAuthenticatedClient();

            var response = await client.GetAsync(
                "api/files/" + blobName);

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            var contentType =
                response.Content.Headers.ContentType?.MediaType
                ?? "application/octet-stream";

            return File(fileBytes, contentType);
        }
        catch
        {
            return NotFound();
        }
    }

    private MultipartFormDataContent CreateEventContent(
        EventViewModel model)
    {
        var content = new MultipartFormDataContent();

        content.Add(new StringContent(model.Name ?? ""), "Name");
        content.Add(new StringContent(model.Description ?? ""), "Description");
        content.Add(new StringContent(model.Location ?? ""), "Location");

        content.Add(
            new StringContent(
                model.EventDate.ToString(
                    "O",
                    CultureInfo.InvariantCulture)),
            "EventDate");

        content.Add(
            new StringContent(
                model.MaximumParticipants.ToString(
                    CultureInfo.InvariantCulture)),
            "MaximumParticipants");

        content.Add(
            new StringContent(
                model.EventCategoryId.ToString(
                    CultureInfo.InvariantCulture)),
            "EventCategoryId");

        if (model.BannerImage != null &&
            model.BannerImage.Length > 0)
        {
            var fileContent = new StreamContent(
                model.BannerImage.OpenReadStream());

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(
                    model.BannerImage.ContentType);

            content.Add(
                fileContent,
                "bannerImage",
                model.BannerImage.FileName);
        }

        return content;
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient("RaceDayAPI");

        var token = HttpContext.Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrWhiteSpace(
            HttpContext.Session.GetString("JWToken"));
    }

    private bool IsOrganiser()
    {
        return IsLoggedIn() &&
               HttpContext.Session.GetString("UserRole") == "Organiser";
    }

    private async Task LoadCategories(EventViewModel model)
    {
        try
        {
            model.Categories =
                await _apiService.GetAsync<List<EventCategoryOption>>(
                    "api/categories") ?? new List<EventCategoryOption>();
        }
        catch
        {
            model.Categories = new List<EventCategoryOption>();
        }
    }
}


