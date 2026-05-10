using FlowerShop.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace FlowerShop.Repositories.Interfaces;
public interface IOrderRepository
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task AddOrderAsync(Order order);
    Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems);
    Task<List<Order>> GetOrdersForIndexAsync(bool canManageOrders, int? currentUserId, OrderStatus? status, DateTime? fromDate, DateTime? toDate);
    Task<List<Order>> GetHistoryAsync(int userId);
    Task<Order?> GetDetailsAsync(int id);
    Task<Order?> GetForEditAsync(int id);
    Task<Order?> GetForDeleteAsync(int id);
    void Remove(Order order);
    Task SaveChangesAsync();
}
