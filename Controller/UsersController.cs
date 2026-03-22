using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
    private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
    public async Task<IActionResult> Index(string? search, string? role)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        ViewBag.Search = search;
        ViewBag.Role = role;
        ViewBag.Roles = Enum.GetValues<UserRole>();
        return View(await _userService.GetIndexAsync(search, role));
    }
    public async Task<IActionResult> Details(int id)
    {
        if (!IsAdmin() && (!IsLoggedIn() || id != CurrentUserId))
            return RedirectToAction("Login", "Auth");
        var user = await _userService.GetDetailsAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin() && (!IsLoggedIn() || id != CurrentUserId))
            return RedirectToAction("Login", "Auth");
        var user = await _userService.GetDetailsAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, User user, string? newPassword)
    {
        if (!IsAdmin() && (!IsLoggedIn() || id != CurrentUserId))
            return RedirectToAction("Login", "Auth");
        if (id != user.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var result = await _userService.UpdateAsync(id, user, newPassword, IsAdmin());
            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Unable to update user.");
                return View(user);
            }
            var updatedUser = await _userService.GetDetailsAsync(id);
            if (updatedUser is null) return NotFound();
            if (id == CurrentUserId)
            {
                HttpContext.Session.SetString("Username", updatedUser.Username);
                HttpContext.Session.SetString("UserRole", updatedUser.Role.ToString());
            }
            return IsAdmin() ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Details), new { id });
        }
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        var result = await _userService.DeleteAsync(id, CurrentUserId);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Unable to delete user.");
            return await Index(null, null);
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Profile()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        var user = await _userService.GetDetailsAsync(CurrentUserId!.Value);
        if (user == null) return NotFound();
        return View(nameof(Details), user);
    }
}
