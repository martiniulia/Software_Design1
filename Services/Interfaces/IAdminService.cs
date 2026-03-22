using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IAdminService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<string?> DeleteCategoryAsync(int id);
    Task<List<Flower>> GetFlowersAsync();
    Task DeleteFlowerAsync(int id);
    Task<List<Bouquet>> GetBouquetsAsync();
    Task DeleteBouquetAsync(int id);
    Task<List<User>> GetUsersAsync();
    Task<List<Order>> GetOrdersAsync();
    Task<List<User>> GetFloristUsersAsync();
    Task<string?> AssignOrderFloristAsync(int orderId, int? floristUserId);
    Task<bool> UpdateUserRoleAsync(int userId, UserRole role);
}
