
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiService _apiService;

    public AccountController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync(
                "api/auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            var result = await response.Content
                .ReadFromJsonAsync<JsonElement>();

            var token = result.GetProperty("token").GetString();

            var user = result.GetProperty("user");

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API did not return a valid login token.");

                return View(model);
            }

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString(
                "UserId", user.GetProperty("id").ToString());
            HttpContext.Session.SetString(
                "FullName", user.GetProperty("fullName").GetString() ?? "");
            HttpContext.Session.SetString(
                "Email", user.GetProperty("email").GetString() ?? "");
            HttpContext.Session.SetString(
                "UserRole", user.GetProperty("role").GetString() ?? "");

            if (HttpContext.Session.GetString("UserRole") == "Organiser")
            {
                return RedirectToAction("Index", "Organiser");
            }

            return RedirectToAction("Index", "Participant");
        }
        catch (Exception)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to RaceDay API. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync(
                "api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. The email may already be registered.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Your account has been created. Please log in.";

            return RedirectToAction(nameof(Login));
        }
        catch (Exception)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to RaceDay API. Please try again.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}

