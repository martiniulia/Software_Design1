using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class CartServiceTests
{
    [Fact]
    public async Task BuildCartItemAsync_WhenQuantityBelowOne_NormalizesToOne()
    {
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        flowerRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Flower
        {
            Id = 5,
            Name = "Rose",
            Price = 10m,
            StockQuantity = 3,
            ImageUrl = "/r.jpg"
        });
        var sut = new CartService(flowerRepo.Object, bouquetRepo.Object);

        var item = await sut.BuildCartItemAsync(5, "flower", 0);

        Assert.NotNull(item);
        Assert.Equal(1, item!.Quantity);
    }

    [Fact]
    public async Task BuildCartItemAsync_Flower_ReturnsCartItemWithIsBouquetFalse()
    {
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        flowerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Flower
        {
            Id = 1,
            Name = "Tulip",
            Price = 4.5m,
            StockQuantity = 10
        });
        var sut = new CartService(flowerRepo.Object, bouquetRepo.Object);

        var item = await sut.BuildCartItemAsync(1, "flower", 2);

        Assert.NotNull(item);
        Assert.False(item!.IsBouquet);
        Assert.Equal("Tulip", item.Name);
        Assert.Equal(4.5m, item.Price);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public async Task BuildCartItemAsync_Bouquet_ReturnsCartItemWithIsBouquetTrue()
    {
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        bouquetRepo.Setup(r => r.GetByIdWithFlowersAsync(9)).ReturnsAsync(new Bouquet
        {
            Id = 9,
            Name = "Spring",
            Price = 50m,
            ImageUrl = "/b.png"
        });
        var sut = new CartService(flowerRepo.Object, bouquetRepo.Object);

        var item = await sut.BuildCartItemAsync(9, "bouquet", 1);

        Assert.NotNull(item);
        Assert.True(item!.IsBouquet);
        Assert.Equal(9, item.Id);
        Assert.Equal(50m, item.Price);
    }

    [Fact]
    public async Task BuildCartItemAsync_WhenProductMissing_ReturnsNull()
    {
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        flowerRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Flower?)null);
        var sut = new CartService(flowerRepo.Object, bouquetRepo.Object);

        var item = await sut.BuildCartItemAsync(99, "flower", 1);

        Assert.Null(item);
    }
}
