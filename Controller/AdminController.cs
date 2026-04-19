using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    public IActionResult Index() => View();
    public async Task<IActionResult> Categories()
    {
        return View(await _adminService.GetCategoriesAsync());
    }
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var error = await _adminService.DeleteCategoryAsync(id);
        if (!string.IsNullOrWhiteSpace(error))
        {
            TempData["Error"] = error;
        }
        return RedirectToAction(nameof(Categories));
    }
    public async Task<IActionResult> Flowers()
    {
        return View(await _adminService.GetFlowersAsync());
    }
    [HttpPost]
    public async Task<IActionResult> DeleteFlower(int id)
    {
        await _adminService.DeleteFlowerAsync(id);
        return RedirectToAction(nameof(Flowers));
    }
    public async Task<IActionResult> Bouquets()
    {
        return View(await _adminService.GetBouquetsAsync());
    }
    [HttpPost]
    public async Task<IActionResult> DeleteBouquet(int id)
    {
        await _adminService.DeleteBouquetAsync(id);
        return RedirectToAction(nameof(Bouquets));
    }
    public async Task<IActionResult> Users()
    {
        return View(await _adminService.GetUsersAsync());
    }
    public async Task<IActionResult> Orders()
    {
        var orders = await _adminService.GetOrdersAsync();
        ViewBag.FloristUsers = await _adminService.GetFloristUsersAsync();
        return View(orders);
    }
    [HttpPost]
    public async Task<IActionResult> AssignOrderFlorist(int orderId, int? floristUserId)
    {
        var error = await _adminService.AssignOrderFloristAsync(orderId, floristUserId);
        if (!string.IsNullOrWhiteSpace(error))
        {
            TempData["Error"] = error;
        }
        return RedirectToAction(nameof(Orders));
    }
    [HttpPost]
    public async Task<IActionResult> UpdateUserRole(int userId, UserRole role)
    {
        var updated = await _adminService.UpdateUserRoleAsync(userId, role);
        if (!updated) return NotFound();
        return RedirectToAction(nameof(Users));
    }
}
