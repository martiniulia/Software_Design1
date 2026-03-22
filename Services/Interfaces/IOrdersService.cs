using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IOrdersService
{
    Task<List<Order>> GetIndexAsync(bool canManageOrders, int? currentUserId, string? status, string? fromDate, string? toDate);
    Task<List<Order>> GetHistoryAsync(int userId);
    Task<Order?> GetDetailsAsync(int id);
    Task<OrderCreateViewData> GetCreateViewDataAsync();
    Task<OrderCreateResult> CreateAsync(Order order, List<int> flowerIds, List<int> quantities, int currentUserId);
    Task<bool> UpdateStatusAsync(int id, OrderStatus status);
    Task<Order?> GetEditAsync(int id);
    Task<bool> UpdateAsync(int id, Order order);
    Task DeleteAsync(int id);
}
