using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class OrdersServiceTests
{
    [Fact]
    public async Task GetIndexAsync_ParsesStatusAndDateFilters()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.GetOrdersForIndexAsync(false, 5, OrderStatus.Delivered,
                It.Is<DateTime?>(d => d.HasValue && d.Value.Date == new DateTime(2025, 1, 10).Date),
                It.Is<DateTime?>(d => d.HasValue && d.Value.Date == new DateTime(2025, 1, 20).Date)))
            .ReturnsAsync(new List<Order>());
        var sut = new OrdersService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);

        await sut.GetIndexAsync(false, 5, "Delivered", "2025-01-10", "2025-01-19");

        orderRepo.Verify(r => r.GetOrdersForIndexAsync(false, 5, OrderStatus.Delivered,
            It.Is<DateTime?>(d => d.HasValue && d.Value.Date == new DateTime(2025, 1, 10).Date),
            It.Is<DateTime?>(d => d.HasValue && d.Value.Date == new DateTime(2025, 1, 20).Date)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenInsufficientStock_ReturnsFailureWithoutCompletingHappyPath()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        var low = new Flower { Id = 1, Name = "Orchid", StockQuantity = 1, Price = 12m };
        flowerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(low);
        var sut = new OrdersService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);

        var result = await sut.CreateAsync(new Order(), new List<int> { 1 }, new List<int> { 3 }, 9);

        Assert.False(result.Success);
        Assert.Contains("Insufficient stock", result.ErrorMessage, StringComparison.Ordinal);
        orderRepo.Verify(r => r.AddOrderAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenStockOk_PersistsOrderAndItems()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        var f = new Flower { Id = 2, Name = "Daisy", StockQuantity = 5, Price = 2m };
        flowerRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(f);
        orderRepo.Setup(r => r.AddOrderAsync(It.IsAny<Order>()))
            .Callback<Order>(o => o.Id = 77)
            .Returns(Task.CompletedTask);
        orderRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        orderRepo.Setup(r => r.AddOrderItemsAsync(It.IsAny<IEnumerable<OrderItem>>())).Returns(Task.CompletedTask);

        var sut = new OrdersService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);
        var newOrder = new Order();

        var result = await sut.CreateAsync(newOrder, new List<int> { 2 }, new List<int> { 4 }, 3);

        Assert.True(result.Success);
        Assert.Equal(77, result.OrderId);
        Assert.Equal(3, newOrder.UserId);
        Assert.Equal(OrderStatus.Pending, newOrder.Status);
        Assert.Equal(8m, newOrder.TotalAmount);
        Assert.Equal(1, f.StockQuantity);
        orderRepo.Verify(r => r.AddOrderItemsAsync(It.IsAny<IEnumerable<OrderItem>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenOrderMissing_ReturnsFalse()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.GetDetailsAsync(99)).ReturnsAsync((Order?)null);
        var sut = new OrdersService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);

        var ok = await sut.UpdateStatusAsync(99, OrderStatus.Confirmed);

        Assert.False(ok);
        orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenOrderExists_UpdatesAndSaves()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        var existing = new Order { Id = 1, Status = OrderStatus.Pending };
        orderRepo.Setup(r => r.GetDetailsAsync(1)).ReturnsAsync(existing);
        orderRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var sut = new OrdersService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);

        var ok = await sut.UpdateStatusAsync(1, OrderStatus.InPreparation);

        Assert.True(ok);
        Assert.Equal(OrderStatus.InPreparation, existing.Status);
        orderRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
