using Microsoft.AspNetCore.Mvc;

namespace RaceDay.web.Controllers;

public class ParticipantController : Controller
{
    public IActionResult Index()
    {
        if (!string.IsNullOrEmpty(
            HttpContext.Session.GetString("JWToken")))
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.FullName =
                HttpContext.Session.GetString("FullName");

            return View();
        }
        return RedirectToAction("Login", "Account");
    }
}