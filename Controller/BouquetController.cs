using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class BouquetsController : Controller
{
    private readonly IBouquetService _bouquetService;
    public BouquetsController(IBouquetService bouquetService)
    {
        _bouquetService = bouquetService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
    private bool CanManage() => IsAdmin();
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        ViewBag.CanManage = CanManage();
        ViewBag.IsLoggedIn = IsLoggedIn();
        return View(await _bouquetService.GetIndexDataAsync(search));
    }
    public async Task<IActionResult> Details(int id)
    {
        var bouquet = await _bouquetService.GetDetailsAsync(id);
        if (bouquet == null) return NotFound();
        ViewBag.CanManage = CanManage();
        ViewBag.IsLoggedIn = IsLoggedIn();
        return View(bouquet);
    }
    public async Task<IActionResult> Create()
    {
        if (!CanManage()) return Forbid();
        ViewBag.Flowers = await _bouquetService.GetAvailableFlowersAsync();
        return View(new Bouquet());
    }
    [HttpPost]
    public async Task<IActionResult> Create(Bouquet bouquet, List<int> flowerIds, List<int> quantities)
    {
        if (!CanManage()) return Forbid();
        if (ModelState.IsValid)
        {
            var id = await _bouquetService.CreateAsync(bouquet, flowerIds ?? new List<int>(), quantities ?? new List<int>());
            return RedirectToAction(nameof(Details), new { id });
        }
        ViewBag.Flowers = await _bouquetService.GetAvailableFlowersAsync();
        return View(bouquet);
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!CanManage()) return Forbid();
        var bouquet = await _bouquetService.GetDetailsAsync(id);
        if (bouquet == null) return NotFound();
        ViewBag.Flowers = await _bouquetService.GetAvailableFlowersAsync();
        return View(bouquet);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Bouquet bouquet)
    {
        if (!CanManage()) return Forbid();
        if (id != bouquet.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var updated = await _bouquetService.UpdateAsync(id, bouquet);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Flowers = await _bouquetService.GetAvailableFlowersAsync();
        return View(bouquet);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return Forbid();
        await _bouquetService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
