using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IUserService
{
    Task<List<User>> GetIndexAsync(string? search, string? role);
    Task<User?> GetDetailsAsync(int id);
    Task<UserOperationResult> UpdateAsync(int id, User user, string? newPassword, bool canChangeRole);
    Task<UserOperationResult> DeleteAsync(int id, int? currentUserId);
}
