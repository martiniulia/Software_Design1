using FlowerShop.CatalogService.CQRS.Commands;
using FlowerShop.CatalogService.CQRS.Handlers;
using FlowerShop.CatalogService.CQRS.Queries;
using FlowerShop.Data;
using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Software_Design1.Tests;

public class CQRSFlowerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateFlowerCommand_ShouldAddFlowerToDatabase()
    {
        // Arrange
        var dbContext = GetDbContext();
        var handler = new CreateFlowerCommandHandler(dbContext);
        var command = new CreateFlowerCommand
        {
            Name = "Red Rose",
            Price = 10m,
            StockQuantity = 50,
            Color = "Red"
        };

        // Act
        var flowerId = await handler.HandleAsync(command);

        // Assert
        var flower = await dbContext.Flowers.FindAsync(flowerId);
        Assert.NotNull(flower);
        Assert.Equal("Red Rose", flower.Name);
        Assert.Equal(10m, flower.Price);
    }

    [Fact]
    public async Task GetAllFlowersQuery_ShouldReturnAllFlowers()
    {
        // Arrange
        var dbContext = GetDbContext();
        dbContext.Flowers.Add(new Flower { Name = "Rose", Price = 10 });
        dbContext.Flowers.Add(new Flower { Name = "Tulip", Price = 8 });
        await dbContext.SaveChangesAsync();

        var handler = new FlowerQueryHandlers(dbContext);
        var query = new GetAllFlowersQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetFlowerByIdQuery_ShouldReturnCorrectFlower()
    {
        // Arrange
        var dbContext = GetDbContext();
        var rose = new Flower { Name = "Rose", Price = 10 };
        dbContext.Flowers.Add(rose);
        await dbContext.SaveChangesAsync();

        var handler = new FlowerQueryHandlers(dbContext);
        var query = new GetFlowerByIdQuery(rose.Id);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rose", result.Name);
    }
}
