using Microsoft.AspNetCore.Mvc;
using RaceDay.web.Models;
using RaceDay.web.Services;

namespace RaceDay.web.Controllers;

public class CategoriesController : Controller
{
    private readonly RaceDayApiService _apiService;

    public CategoriesController(RaceDayApiService apiService)
    {
        _apiService = apiService;
    }

    private bool IsOrganiser()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken"))
            && HttpContext.Session.GetString("UserRole") == "Organiser";
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                "api/categories");

            return View(categories ?? new List<CategoryViewModel>());
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to load event categories.";
            return View(new List<CategoryViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _apiService.PostAsync("api/categories", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Unable to create the category.");
        }
        catch
        {
            ModelState.AddModelError("", "Unable to communicate with the API.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var category = await _apiService.GetAsync<CategoryViewModel>(
                $"api/categories/{id}");

            if (category == null)
                return NotFound();

            return View(category);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _apiService.PutAsync(
                $"api/categories/{model.Id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Unable to update the category.");
        }
        catch
        {
            ModelState.AddModelError("", "Unable to communicate with the API.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var response = await _apiService.DeleteAsync($"api/categories/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Category deleted successfully.";
            else
                TempData["ErrorMessage"] =
                    "Category could not be deleted. It may already be assigned to an event.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to communicate with the API.";
        }

        return RedirectToAction(nameof(Index));
    }
}
