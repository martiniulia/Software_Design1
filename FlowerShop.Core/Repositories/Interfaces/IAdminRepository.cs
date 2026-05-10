using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface IAdminRepository
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryWithFlowersAsync(int id);
    void RemoveCategory(Category category);
    Task<List<Flower>> GetFlowersAsync();
    Task<Flower?> GetFlowerByIdAsync(int id);
    void RemoveFlower(Flower flower);
    Task<List<Bouquet>> GetBouquetsAsync();
    Task<Bouquet?> GetBouquetWithFlowersAsync(int id);
    void RemoveBouquet(Bouquet bouquet);
    void RemoveBouquetFlowers(IEnumerable<BouquetFlower> bouquetFlowers);
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<List<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<User?> GetFloristUserAsync(int userId);
    Task<Florist?> GetFloristByUserIdAsync(int userId);
    Task AddFloristAsync(Florist florist);
    Task SaveChangesAsync();
}
