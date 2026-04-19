using FlowerShop.Extensions;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FlowerShop.Controllers;
[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class CartController : Controller
{
    private const string CartSessionKey = "CartItems";
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    public CartController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        await HttpContext.Session.LoadAsync();
        var items = GetCartItems();
        ViewBag.Total = items.Sum(i => i.Price * i.Quantity);
        return View(items);
    }
    [HttpPost]
    public async Task<IActionResult> AddToCart(int id, string productType, int quantity = 1, string? returnUrl = null)
    {
        await HttpContext.Session.LoadAsync();
        if (quantity < 1)
        {
            quantity = 1;
        }
        var isBouquet = string.Equals(productType, "bouquet", StringComparison.OrdinalIgnoreCase);
        var cart = GetCartItems();
        var existing = cart.FirstOrDefault(i => i.Id == id && i.IsBouquet == isBouquet);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            SaveCartItems(cart);
            return RedirectToLocalOrDefault(returnUrl);
        }
        var cartItem = await _cartService.BuildCartItemAsync(id, productType, quantity);
        if (cartItem is null) return NotFound();
        cart.Add(cartItem);
        SaveCartItems(cart);
        return RedirectToLocalOrDefault(returnUrl);
    }
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int id, string productType)
    {
        await HttpContext.Session.LoadAsync();
        var isBouquet = string.Equals(productType, "bouquet", StringComparison.OrdinalIgnoreCase);
        var cart = GetCartItems();
        var item = cart.FirstOrDefault(i => i.Id == id && i.IsBouquet == isBouquet);
        if (item is not null)
        {
            cart.Remove(item);
            SaveCartItems(cart);
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        await HttpContext.Session.LoadAsync();
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
        {
            var returnUrl = Url.Action(nameof(Index), "Cart") ?? "/Cart";
            return RedirectToAction("Login", "Auth", new { returnUrl });
        }
        var cart = GetCartItems();
        if (cart.Count == 0)
        {
            return RedirectToAction(nameof(Index));
        }
        var result = await _orderService.PlaceOrderAsync(userId.Value, cart);
        if (!result.Success)
        {
            TempData["CartError"] = result.ErrorMessage ?? "Unable to place order.";
            return RedirectToAction(nameof(Index));
        }
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction("History", "Orders");
    }
    private List<CartItem> GetCartItems()
    {
        return HttpContext.Session.GetJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
    }
    private void SaveCartItems(List<CartItem> cartItems)
    {
        HttpContext.Session.SetJson(CartSessionKey, cartItems);
    }
    private IActionResult RedirectToLocalOrDefault(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Index));
    }
}
