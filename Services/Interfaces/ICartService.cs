using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface ICartService
{
    Task<CartItem?> BuildCartItemAsync(int id, string productType, int quantity);
}
