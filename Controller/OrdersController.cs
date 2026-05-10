using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class OrdersController : Controller
{
    private readonly IOrdersService _ordersService;
    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool IsFlorist() => HttpContext.Session.GetString("UserRole") == "Florist";
    private bool IsClient() => HttpContext.Session.GetString("UserRole") == "Client";
    private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
    private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
    private bool CanManageOrders() => IsAdmin() || IsFlorist();

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string? status, string? fromDate, string? toDate)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        ViewBag.Status = status;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.IsAdmin = IsAdmin();
        ViewBag.IsFlorist = IsFlorist();
        ViewBag.CanManage = CanManageOrders();
        ViewBag.Statuses = Enum.GetValues<OrderStatus>();
        var orders = await _ordersService.GetIndexAsync(CanManageOrders(), CurrentUserId, status, fromDate, toDate);
        return View(orders);
    }

    [Authorize(Roles = "Client")]
    public async Task<IActionResult> History()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("History", "Orders") });
        if (!IsClient()) return Forbid();
        var orders = await _ordersService.GetHistoryAsync(CurrentUserId!.Value);
        return View(orders);
    }
    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        var order = await _ordersService.GetDetailsAsync(id);
        if (order == null) return NotFound();
        if (!CanManageOrders() && order.UserId != CurrentUserId)
            return Forbid();
        ViewBag.IsAdmin = IsAdmin();
        ViewBag.CanManage = CanManageOrders();
        ViewBag.Statuses = Enum.GetValues<OrderStatus>();
        return View(order);
    }
    public async Task<IActionResult> Create()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        var data = await _ordersService.GetCreateViewDataAsync();
        ViewBag.Flowers = data.Flowers;
        ViewBag.Bouquets = data.Bouquets;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Order order, List<int> flowerIds, List<int> quantities)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        if (ModelState.IsValid)
        {
            var result = await _ordersService.CreateAsync(order, flowerIds ?? new List<int>(), quantities ?? new List<int>(), CurrentUserId!.Value);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Unable to create order.");
                await SetupCreateView();
                return View(order);
            }
            return RedirectToAction(nameof(Details), new { id = result.OrderId });
        }
        await SetupCreateView();
        return View(order);
    }
    private async Task SetupCreateView()
    {
        var data = await _ordersService.GetCreateViewDataAsync();
        ViewBag.Flowers = data.Flowers;
        ViewBag.Bouquets = data.Bouquets;
    }
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        if (!IsFlorist()) return Forbid();
        var updated = await _ordersService.UpdateStatusAsync(id, status);
        if (!updated) return NotFound();
        return RedirectToAction(nameof(Details), new { id });
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin()) return Forbid();
        var order = await _ordersService.GetEditAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (!IsAdmin()) return Forbid();
        if (id != order.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var updated = await _ordersService.UpdateAsync(id, order);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return Forbid();
        await _ordersService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
