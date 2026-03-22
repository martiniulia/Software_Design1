using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task DeleteAsync_WhenDeletingSelf_ReturnsFailure()
    {
        var repo = new Mock<IUserRepository>();
        var sut = new UserService(repo.Object);

        var result = await sut.DeleteAsync(5, 5);

        Assert.False(result.Success);
        Assert.Equal("You cannot delete your own account.", result.ErrorMessage);
        repo.Verify(r => r.GetByIdWithOrdersAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserHasOrders_ReturnsFailure()
    {
        var repo = new Mock<IUserRepository>();
        var user = new User
        {
            Id = 2,
            Orders = new List<Order> { new Order { Id = 1 } }
        };
        repo.Setup(r => r.GetByIdWithOrdersAsync(2)).ReturnsAsync(user);
        var sut = new UserService(repo.Object);

        var result = await sut.DeleteAsync(2, 99);

        Assert.False(result.Success);
        Assert.Contains("existing orders", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        repo.Verify(r => r.Remove(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserHasNoOrders_RemovesAndSaves()
    {
        var repo = new Mock<IUserRepository>();
        var user = new User { Id = 3, Orders = new List<Order>() };
        repo.Setup(r => r.GetByIdWithOrdersAsync(3)).ReturnsAsync(user);
        repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var sut = new UserService(repo.Object);

        var result = await sut.DeleteAsync(3, 1);

        Assert.True(result.Success);
        repo.Verify(r => r.Remove(user), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
