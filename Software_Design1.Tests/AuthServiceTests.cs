using System.Security.Cryptography;
using System.Text;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class AuthServiceTests
{
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsFailure()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.EmailExistsAsync("taken@x.com")).ReturnsAsync(true);
        var sut = new AuthService(repo.Object);

        var result = await sut.RegisterAsync("u", "taken@x.com", "secret");

        Assert.False(result.Success);
        Assert.Equal("Email already exists.", result.ErrorMessage);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenNewEmail_CreatesClientAndSucceeds()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.EmailExistsAsync("new@x.com")).ReturnsAsync(false);
        var sut = new AuthService(repo.Object);

        var result = await sut.RegisterAsync("alice", "new@x.com", "p@ss");

        Assert.True(result.Success);
        Assert.NotNull(result.User);
        Assert.Equal(UserRole.Client, result.User!.Role);
        repo.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Username == "alice" &&
            u.Email == "new@x.com" &&
            u.PasswordHash == HashPassword("p@ss"))), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsInvalid_ReturnsFailure()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAndPasswordHashAsync("a@b.com", HashPassword("wrong")))
            .ReturnsAsync((User?)null);
        var sut = new AuthService(repo.Object);

        var result = await sut.LoginAsync("a@b.com", "wrong");

        Assert.False(result.Success);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ReturnsUser()
    {
        var user = new User { Id = 3, Email = "ok@x.com", Username = "bob", Role = UserRole.Client };
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAndPasswordHashAsync("ok@x.com", HashPassword("good")))
            .ReturnsAsync(user);
        var sut = new AuthService(repo.Object);

        var result = await sut.LoginAsync("ok@x.com", "good");

        Assert.True(result.Success);
        Assert.Same(user, result.User);
    }
}
