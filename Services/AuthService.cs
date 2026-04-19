using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
namespace FlowerShop.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        if (await _userRepository.EmailExistsAsync(email))
        {
            return new AuthResult { Success = false, ErrorMessage = "Email already exists." };
        }
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Role = UserRole.Client
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return new AuthResult { Success = true, User = user };
    }
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAndPasswordHashAsync(email, HashPassword(password));
        if (user is null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Invalid email or password." };
        }
        return new AuthResult { Success = true, User = user };
    }
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
