using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IOrderService
{
    Task<PlaceOrderResult> PlaceOrderAsync(int userId, IReadOnlyCollection<CartItem> cartItems);
}
