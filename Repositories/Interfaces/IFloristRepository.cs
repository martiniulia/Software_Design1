using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface IFloristRepository
{
    Task<List<Florist>> GetAllAsync(string? search);
    Task<Florist?> GetByIdAsync(int id);
    Task<Florist?> GetByIdWithFlowersAsync(int id);
    Task<Florist?> GetByUserIdAsync(int userId);
    Task AddAsync(Florist florist);
    void Remove(Florist florist);
    Task<List<Order>> GetAssignedOrdersByStatusAsync(int floristId, OrderStatus status);
    Task<Order?> GetAssignedOrderDetailsAsync(int orderId);
    Task SaveChangesAsync();
}
