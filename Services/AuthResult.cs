using FlowerShop.Models;
namespace FlowerShop.Services;
public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User { get; init; }
}
