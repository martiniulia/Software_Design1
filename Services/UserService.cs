using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
namespace FlowerShop.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(IUserRepository userRepository, IHttpClientFactory httpClientFactory)
    {
        _userRepository = userRepository;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<List<User>> GetIndexAsync(string? search, string? role)
    {
        UserRole? parsedRole = null;
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, out var roleValue))
        {
            parsedRole = roleValue;
        }
        return await _userRepository.GetAllAsync(search, parsedRole);
    }
    public async Task<User?> GetDetailsAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("UserService");
        var response = await client.GetAsync($"api/users/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }
        return await _userRepository.GetByIdWithOrdersAsync(id);
    }
    public async Task<UserOperationResult> UpdateAsync(int id, User user, string? newPassword, bool canChangeRole)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return new UserOperationResult { Success = false, ErrorMessage = "User not found." };
        }
        existing.Username = user.Username;
        existing.Email = user.Email;
        if (canChangeRole)
        {
            existing.Role = user.Role;
        }
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            existing.PasswordHash = HashPassword(newPassword);
        }
        await _userRepository.SaveChangesAsync();
        return new UserOperationResult { Success = true };
    }
    public async Task<UserOperationResult> DeleteAsync(int id, int? currentUserId)
    {
        if (id == currentUserId)
        {
            return new UserOperationResult { Success = false, ErrorMessage = "You cannot delete your own account." };
        }
        var user = await _userRepository.GetByIdWithOrdersAsync(id);
        if (user is null)
        {
            return new UserOperationResult { Success = true };
        }
        if (user.Orders.Any())
        {
            return new UserOperationResult { Success = false, ErrorMessage = "Cannot delete user with existing orders." };
        }
        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        return new UserOperationResult { Success = true };
    }
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
