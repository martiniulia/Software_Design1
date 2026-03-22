using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class FloristOrdersService : IFloristOrdersService
{
    private readonly IFloristRepository _floristRepository;
    public FloristOrdersService(IFloristRepository floristRepository)
    {
        _floristRepository = floristRepository;
    }
    public async Task<FloristDashboardData> GetDashboardAsync(int? currentUserId)
    {
        if (!currentUserId.HasValue)
        {
            return new FloristDashboardData { AssignmentWarning = "No florist profile is linked to your account." };
        }
        var florist = await _floristRepository.GetByUserIdAsync(currentUserId.Value);
        if (florist is null)
        {
            return new FloristDashboardData { AssignmentWarning = "No florist profile is linked to your account." };
        }
        var pending = await _floristRepository.GetAssignedOrdersByStatusAsync(florist.Id, OrderStatus.Pending);
        var inPrep = await _floristRepository.GetAssignedOrdersByStatusAsync(florist.Id, OrderStatus.InPreparation);
        return new FloristDashboardData { Pending = pending, InPreparation = inPrep };
    }
    public async Task<Order?> GetOrderDetailsAsync(int orderId, int? currentUserId)
    {
        if (!currentUserId.HasValue) return null;
        var florist = await _floristRepository.GetByUserIdAsync(currentUserId.Value);
        if (florist is null) return null;
        var order = await _floristRepository.GetAssignedOrderDetailsAsync(orderId);
        if (order is null) return null;
        if (order.AssignedFloristId != florist.Id) return null;
        if (order.Status is not (OrderStatus.Pending or OrderStatus.InPreparation)) return null;
        return order;
    }
    public async Task<bool> SetInPreparationAsync(int orderId, int? currentUserId)
    {
        var order = await GetOrderDetailsAsync(orderId, currentUserId);
        if (order is null || order.Status != OrderStatus.Pending) return false;
        order.Status = OrderStatus.InPreparation;
        await _floristRepository.SaveChangesAsync();
        return true;
    }
    public async Task<bool> SetDeliveredAsync(int orderId, int? currentUserId)
    {
        var order = await GetOrderDetailsAsync(orderId, currentUserId);
        if (order is null || order.Status != OrderStatus.InPreparation) return false;
        order.Status = OrderStatus.Delivered;
        await _floristRepository.SaveChangesAsync();
        return true;
    }
}
