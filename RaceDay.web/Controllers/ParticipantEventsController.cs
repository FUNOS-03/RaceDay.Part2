
using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class ParticipantEventsController : Controller
{
    private readonly RaceDayApiService _apiService;

    public ParticipantEventsController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    // Browse available events
    public async Task<IActionResult> Index()
    {
        if (!IsParticipant())
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

    // View the participant's own enrolments
    public async Task<IActionResult> MyEnrolments()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var enrolments =
                await _apiService.GetAsync<List<EnrolmentViewModel>>(
                    "api/enrolments/my");

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load your enrolments.";
            return View(new List<EnrolmentViewModel>());
        }
    }

    // Enrol in an event
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventId)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var response = await _apiService.PostAsync(
                "api/enrolments",
                new { EventId = eventId });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "You have successfully enrolled.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Enrolment unsuccessful. You may already be enrolled or the event may be full.";
            }
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to connect to the API.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Cancel an enrolment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var response = await _apiService.DeleteAsync(
                $"api/enrolments/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Enrolment cancelled successfully.";
            else
                TempData["ErrorMessage"] = "Unable to cancel this enrolment.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to connect to the API.";
        }

        return RedirectToAction(nameof(MyEnrolments));
    }

    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("UserRole") == "Participant"
            && !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWToken"));
    }
}
