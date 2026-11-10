
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class ParticipantResultsController : Controller
{
    private readonly RaceDayApiService _apiService;

    public ParticipantResultsController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            return RedirectToAction("Login", "Account");

        if (HttpContext.Session.GetString("UserRole") != "Participant")
            return RedirectToAction("Index", "Home");

        try
        {
            var results = await _apiService.GetAsync<List<ParticipantResultViewModel>>(
                "api/results/my");

            return View(results ?? new List<ParticipantResultViewModel>());
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load your race results.";
            return View(new List<ParticipantResultViewModel>());
        }
    }
}
