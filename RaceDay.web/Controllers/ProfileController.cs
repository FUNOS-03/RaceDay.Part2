
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class ProfileController : Controller
{
    private readonly RaceDayApiService _apiService;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProfileController(
        RaceDayApiService apiService,
        IHttpClientFactory httpClientFactory)
    {
        _apiService = apiService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(
            HttpContext.Session.GetString("JWToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var model = await _apiService.GetAsync<ProfileViewModel>(
                "api/profile/me");

            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to load your profile.";

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to communicate with the RaceDay API.";

            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProfileViewModel model)
    {
        if (string.IsNullOrEmpty(
            HttpContext.Session.GetString("JWToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (model.ProfilePicture != null)
        {
            if (model.ProfilePicture.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(
                    "ProfilePicture",
                    "Profile pictures must be smaller than 5 MB.");
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(
                model.ProfilePicture.ContentType.ToLowerInvariant()))
            {
                ModelState.AddModelError(
                    "ProfilePicture",
                    "Only JPG, PNG and WEBP images are allowed.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            var client =
                _httpClientFactory.CreateClient("RaceDayAPI");

            var token =
                HttpContext.Session.GetString("JWToken");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();

            form.Add(
                new StringContent(model.FullName),
                "FullName");

            form.Add(
                new StringContent(model.Email),
                "Email");

            if (model.ProfilePicture != null)
            {
                var fileContent = new StreamContent(
                    model.ProfilePicture.OpenReadStream());

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(
                        model.ProfilePicture.ContentType);

                form.Add(
                    fileContent,
                    "ProfilePicture",
                    model.ProfilePicture.FileName);
            }

            var response = await client.PutAsync(
                "api/profile/me",
                form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Your profile has been updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            var responseText =
                await response.Content.ReadAsStringAsync();

            try
            {
                using var document =
                    JsonDocument.Parse(responseText);

                if (document.RootElement.TryGetProperty(
                    "message", out var message))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        message.GetString() ??
                        "Unable to update your profile.");
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to update your profile.");
                }
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update your profile.");
            }

            return View("Index", model);
        }
        catch
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to communicate with the RaceDay API.");

            return View("Index", model);
        }
    }
}

