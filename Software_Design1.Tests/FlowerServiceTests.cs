using System.Security.Claims;
using FlowerShop.Events;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class FlowerServiceTests
{
    private readonly Mock<IFlowerRepository> _flowerRepositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly FlowerService _flowerService;

    public FlowerServiceTests()
    {
        _flowerRepositoryMock = new Mock<IFlowerRepository>();
        _eventBusMock = new Mock<IEventBus>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var httpContext = new DefaultHttpContext();
        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "test@test.com") });
        httpContext.User = new ClaimsPrincipal(claimsIdentity);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _flowerService = new FlowerService(_flowerRepositoryMock.Object, _eventBusMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldPublishFlowerCreatedEvent()
    {
        var flower = new Flower { Id = 1, Name = "Rose", Price = 10, StockQuantity = 50 };

        await _flowerService.CreateAsync(flower);

        _eventBusMock.Verify(bus => bus.Publish(It.Is<FlowerCreatedEvent>(e => 
            e.FlowerId == 1 &&
            e.FlowerName == "Rose" &&
            e.Price == 10 &&
            e.Stock == 50 &&
            e.TriggeredByEmail == "test@test.com"
        )), Times.Once);
        
        _flowerRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPublishFlowerUpdatedEvent_WithChanges()
    {
        var existingFlower = new Flower { Id = 1, Name = "Rose", Price = 10, StockQuantity = 50, Description = "Red" };
        var updatedFlower = new Flower { Id = 1, Name = "White Rose", Price = 15, StockQuantity = 40, Description = "White" };

        _flowerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingFlower);

        var result = await _flowerService.UpdateAsync(1, updatedFlower);

        Assert.True(result);
        _eventBusMock.Verify(bus => bus.Publish(It.Is<FlowerUpdatedEvent>(e => 
            e.FlowerId == 1 &&
            e.FlowerName == "White Rose" &&
            e.OldName == "White Rose" &&
            e.Price == 15 &&
            e.Changes.Contains("Name changed from 'Rose' to 'White Rose'") &&
            e.Changes.Contains("Price changed from 10 to 15") &&
            e.Changes.Contains("Stock changed from 50 to 40") &&
            e.Changes.Contains("Description changed from 'Red' to 'White'")
        )), Times.Once);

        _flowerRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldPublishFlowerDeletedEvent()
    {
        var existingFlower = new Flower { Id = 1, Name = "Rose" };
        _flowerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingFlower);

        await _flowerService.DeleteAsync(1);

        _eventBusMock.Verify(bus => bus.Publish(It.Is<FlowerDeletedEvent>(e => 
            e.FlowerId == 1 &&
            e.FlowerName == "Rose" &&
            e.TriggeredByEmail == "test@test.com"
        )), Times.Once);

        _flowerRepositoryMock.Verify(repo => repo.Remove(existingFlower), Times.Once);
        _flowerRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }
}
