using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
[Authorize(Roles = "Florist")]
public class FloristController : Controller
{
    private readonly IFloristOrdersService _floristOrdersService;
    public FloristController(IFloristOrdersService floristOrdersService)
    {
        _floristOrdersService = floristOrdersService;
    }
    public IActionResult OrdersToProcess() => RedirectToAction(nameof(Dashboard));
    public async Task<IActionResult> Dashboard()
    {
        var data = await _floristOrdersService.GetDashboardAsync(HttpContext.Session.GetInt32("UserId"));
        ViewBag.Pending = data.Pending;
        ViewBag.InPreparation = data.InPreparation;
        ViewBag.AssignmentWarning = data.AssignmentWarning;
        return View();
    }
    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _floristOrdersService.GetOrderDetailsAsync(id, HttpContext.Session.GetInt32("UserId"));
        if (order is null) return Forbid();
        return View(order);
    }
    [HttpPost]
    public async Task<IActionResult> SetInPreparation(int id)
    {
        var updated = await _floristOrdersService.SetInPreparationAsync(id, HttpContext.Session.GetInt32("UserId"));
        if (!updated) return BadRequest();
        return RedirectToAction(nameof(OrderDetails), new { id });
    }
    [HttpPost]
    public async Task<IActionResult> SetDelivered(int id)
    {
        var updated = await _floristOrdersService.SetDeliveredAsync(id, HttpContext.Session.GetInt32("UserId"));
        if (!updated) return BadRequest();
        return RedirectToAction(nameof(Dashboard));
    }
}
