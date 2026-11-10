
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class OrganiserEnrolmentsController : Controller
{
    private readonly RaceDayApiService _apiService;

    public OrganiserEnrolmentsController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            return RedirectToAction("Login", "Account");

        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Index", "Home");

        try
        {
            var enrolments =
                await _apiService.GetAsync<List<OrganiserEnrolmentViewModel>>(
                    "api/enrolments");

            return View(enrolments ?? new List<OrganiserEnrolmentViewModel>());
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load participant enrolments.";
            return View(new List<OrganiserEnrolmentViewModel>());
        }
    }
}
