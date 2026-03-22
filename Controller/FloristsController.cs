using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class FloristsController : Controller
{
    private readonly IFloristsService _floristsService;
    public FloristsController(IFloristsService floristsService)
    {
        _floristsService = floristsService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool IsFlorist() => HttpContext.Session.GetString("UserRole") == "Florist";
    private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
    public async Task<IActionResult> Index(string? search)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        ViewBag.Search = search;
        ViewBag.IsAdmin = IsAdmin();
        ViewBag.IsFlorist = IsFlorist();
        return View(await _floristsService.GetIndexAsync(search));
    }
    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        var florist = await _floristsService.GetDetailsAsync(id);
        if (florist == null) return NotFound();
        ViewBag.IsAdmin = IsAdmin();
        return View(florist);
    }
    public IActionResult Create()
    {
        if (!IsAdmin()) return Forbid();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Florist florist)
    {
        if (!IsAdmin()) return Forbid();
        if (ModelState.IsValid)
        {
            await _floristsService.CreateAsync(florist);
            return RedirectToAction(nameof(Index));
        }
        return View(florist);
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        var florist = await _floristsService.GetDetailsAsync(id);
        if (florist == null) return NotFound();
        if (IsFlorist())
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (florist.UserId != currentUserId) return Forbid();
        }
        else if (!IsAdmin())
        {
            return Forbid();
        }
        return View(florist);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Florist florist)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        if (id != florist.Id) return NotFound();
        var existing = await _floristsService.GetDetailsAsync(id);
        if (existing == null) return NotFound();
        if (IsFlorist())
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (existing.UserId != currentUserId) return Forbid();
        }
        else if (!IsAdmin())
        {
            return Forbid();
        }
        if (ModelState.IsValid)
        {
            var result = await _floristsService.UpdateAsync(id, florist);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Unable to update florist.");
                return View(florist);
            }
            return RedirectToAction(nameof(Index));
        }
        return View(florist);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return Forbid();
        var result = await _floristsService.DeleteAsync(id);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Unable to delete florist.");
            ViewBag.IsAdmin = true;
            return View("Details", result.Florist);
        }
        return RedirectToAction(nameof(Index));
    }
}
