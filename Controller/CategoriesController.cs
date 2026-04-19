using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        ViewBag.IsAdmin = IsAdmin();
        return View(await _categoryService.GetIndexDataAsync(search));
    }
    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetDetailsAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        if (ModelState.IsValid)
        {
            await _categoryService.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        var category = await _categoryService.GetDetailsAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        if (id != category.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var updated = await _categoryService.UpdateAsync(id, category);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        var result = await _categoryService.DeleteAsync(id);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Unable to delete category.");
            ViewBag.IsAdmin = IsAdmin();
            return View("Details", result.Category);
        }
        return RedirectToAction(nameof(Index));
    }
}
