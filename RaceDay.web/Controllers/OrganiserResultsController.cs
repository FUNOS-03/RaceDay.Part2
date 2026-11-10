using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class OrganiserResultsController : Controller
{
    private readonly RaceDayApiService _apiService;

    public OrganiserResultsController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            return RedirectToAction("Login", "Account");

        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Index", "Home");

        return await LoadResultsPage();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Record(OrganiserResultsViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            return RedirectToAction("Login", "Account");

        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Index", "Home");

        if (!TimeSpan.TryParse(model.FinishTime, out TimeSpan finishTime))
        {
            ModelState.AddModelError(
                "FinishTime",
                "Enter a valid finish time, for example 01:25:30.");
        }

        if (!ModelState.IsValid)
        {
            await LoadPageData(model);
            return View("Index", model);
        }

        try
        {
            var response = await _apiService.PostAsync(
                "api/results",
                new
                {
                    model.EnrolmentId,
                    model.FinishingPosition,
                    FinishTime = finishTime
                });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Race result recorded successfully.";

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] =
                "The result could not be recorded. Check the enrolment and try again.";
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to communicate with the RaceDay API.";
        }

        return await LoadResultsPage();
    }

    private async Task<IActionResult> LoadResultsPage()
    {
        var model = new OrganiserResultsViewModel();
        await LoadPageData(model);
        return View("Index", model);
    }

    private async Task LoadPageData(OrganiserResultsViewModel model)
    {
        try
        {
            model.Enrolments =
                await _apiService.GetAsync<List<OrganiserEnrolmentViewModel>>(
                    "api/enrolments") ?? new();

            model.Results =
                await _apiService.GetAsync<List<RecordedRaceResultViewModel>>(
                    "api/results") ?? new();
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to load enrolments or race results.";
        }
    }
}