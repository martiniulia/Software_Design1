using FlowerShop.Models;
namespace FlowerShop.Services;
public sealed class OrderCreateViewData
{
    public List<Flower> Flowers { get; init; } = new();
    public List<Bouquet> Bouquets { get; init; } = new();
}
