using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IFloristOrdersService
{
    Task<FloristDashboardData> GetDashboardAsync(int? currentUserId);
    Task<Order?> GetOrderDetailsAsync(int orderId, int? currentUserId);
    Task<bool> SetInPreparationAsync(int orderId, int? currentUserId);
    Task<bool> SetDeliveredAsync(int orderId, int? currentUserId);
}
