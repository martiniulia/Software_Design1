using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.UserService.CQRS.Commands;
using FlowerShop.UserService.CQRS.Handlers;
using FlowerShop.UserService.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Software_Design1.Tests;

public class CQRSUserTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateUserCommand_ShouldAddUserToDatabase()
    {
        // Arrange
        var dbContext = GetDbContext();
        var handler = new UserCommandHandlers(dbContext);
        var command = new CreateUserCommand
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            Role = "Client"
        };

        // Act
        var userId = await handler.HandleAsync(command);

        // Assert
        var user = await dbContext.Users.FindAsync(userId);
        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
        Assert.Equal(UserRole.Client, user.Role);
    }

    [Fact]
    public async Task GetUserByIdQuery_ShouldReturnCorrectUser()
    {
        // Arrange
        var dbContext = GetDbContext();
        var user = new User { Username = "admin", Email = "admin@example.com", PasswordHash = "123", Role = UserRole.Admin };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var handler = new UserQueryHandlers(dbContext);
        var query = new GetUserByIdQuery(user.Id);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("admin", result.Username);
    }
}
