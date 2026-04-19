using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class CartService : ICartService
{
    private readonly IFlowerRepository _flowerRepository;
    private readonly IBouquetRepository _bouquetRepository;
    public CartService(IFlowerRepository flowerRepository, IBouquetRepository bouquetRepository)
    {
        _flowerRepository = flowerRepository;
        _bouquetRepository = bouquetRepository;
    }
    public async Task<CartItem?> BuildCartItemAsync(int id, string productType, int quantity)
    {
        if (quantity < 1)
        {
            quantity = 1;
        }
        var isBouquet = string.Equals(productType, "bouquet", StringComparison.OrdinalIgnoreCase);
        if (isBouquet)
        {
            var bouquet = await _bouquetRepository.GetByIdWithFlowersAsync(id);
            if (bouquet is null) return null;
            return new CartItem
            {
                Id = bouquet.Id,
                Name = bouquet.Name,
                Price = bouquet.Price,
                Quantity = quantity,
                ImageUrl = bouquet.ImageUrl,
                IsBouquet = true
            };
        }
        var flower = await _flowerRepository.GetByIdAsync(id);
        if (flower is null) return null;
        return new CartItem
        {
            Id = flower.Id,
            Name = flower.Name,
            Price = flower.Price,
            Quantity = quantity,
            ImageUrl = flower.ImageUrl,
            IsBouquet = false
        };
    }
}
