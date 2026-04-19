using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class FlowersController : Controller
{
    private readonly IFlowerService _flowerService;
    public FlowersController(IFlowerService flowerService)
    {
        _flowerService = flowerService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool CanManageFlowers() => IsAdmin();
    public async Task<IActionResult> Index(string? search, string? sortBy, bool ascending = true)
    {
        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;
        ViewBag.Ascending = ascending;
        ViewBag.IsAdmin = IsAdmin();
        ViewBag.CanManage = CanManageFlowers();
        return View(await _flowerService.GetIndexDataAsync(search, sortBy, ascending));
    }
    public async Task<IActionResult> Details(int id)
    {
        var flower = await _flowerService.GetDetailsAsync(id);
        if (flower == null) return NotFound();
        ViewBag.CanManage = CanManageFlowers();
        return View(flower);
    }
    public async Task<IActionResult> Create()
    {
        if (!CanManageFlowers()) return Forbid();
        ViewBag.Categories = await _flowerService.GetCategoriesAsync();
        ViewBag.Florists = await _flowerService.GetFloristsAsync();
        return View(new Flower());
    }
    [HttpPost]
    public async Task<IActionResult> Create(Flower flower)
    {
        if (!CanManageFlowers()) return Forbid();
        if (ModelState.IsValid)
        {
            await _flowerService.CreateAsync(flower);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = await _flowerService.GetCategoriesAsync();
        ViewBag.Florists = await _flowerService.GetFloristsAsync();
        return View(flower);
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!CanManageFlowers()) return Forbid();
        var flower = await _flowerService.GetDetailsAsync(id);
        if (flower == null) return NotFound();
        ViewBag.Categories = await _flowerService.GetCategoriesAsync();
        ViewBag.Florists = await _flowerService.GetFloristsAsync();
        return View(flower);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Flower flower)
    {
        if (!CanManageFlowers()) return Forbid();
        if (id != flower.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var updated = await _flowerService.UpdateAsync(id, flower);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = await _flowerService.GetCategoriesAsync();
        ViewBag.Florists = await _flowerService.GetFloristsAsync();
        return View(flower);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return Forbid();
        await _flowerService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
