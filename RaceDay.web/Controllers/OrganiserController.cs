using Microsoft.AspNetCore.Mvc;

namespace RaceDay.web.Controllers;

public class OrganiserController : Controller
{
    public IActionResult Index()
    {
        // Check whether the user is logged in
        if (string.IsNullOrEmpty(
            HttpContext.Session.GetString("JWToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        // Only Organisers may access this dashboard
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.FullName =
            HttpContext.Session.GetString("FullName");

        return View();
    }
}