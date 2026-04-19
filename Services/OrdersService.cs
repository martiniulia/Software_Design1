using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class OrdersService : IOrdersService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IFlowerRepository _flowerRepository;
    private readonly IBouquetRepository _bouquetRepository;
    public OrdersService(
        IOrderRepository orderRepository,
        IFlowerRepository flowerRepository,
        IBouquetRepository bouquetRepository)
    {
        _orderRepository = orderRepository;
        _flowerRepository = flowerRepository;
        _bouquetRepository = bouquetRepository;
    }
    public async Task<List<Order>> GetIndexAsync(bool canManageOrders, int? currentUserId, string? status, string? fromDate, string? toDate)
    {
        OrderStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, out var statusValue))
        {
            parsedStatus = statusValue;
        }
        DateTime? parsedFrom = DateTime.TryParse(fromDate, out var from) ? from : null;
        DateTime? parsedTo = DateTime.TryParse(toDate, out var to) ? to.AddDays(1) : null;
        return await _orderRepository.GetOrdersForIndexAsync(canManageOrders, currentUserId, parsedStatus, parsedFrom, parsedTo);
    }
    public Task<List<Order>> GetHistoryAsync(int userId)
    {
        return _orderRepository.GetHistoryAsync(userId);
    }
    public Task<Order?> GetDetailsAsync(int id)
    {
        return _orderRepository.GetDetailsAsync(id);
    }
    public async Task<OrderCreateViewData> GetCreateViewDataAsync()
    {
        return new OrderCreateViewData
        {
            Flowers = await _flowerRepository.GetInStockWithCategoryAsync(),
            Bouquets = await _bouquetRepository.GetAllWithFiltersAsync(null)
        };
    }
    public async Task<OrderCreateResult> CreateAsync(Order order, List<int> flowerIds, List<int> quantities, int currentUserId)
    {
        order.UserId = currentUserId;
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;
        decimal total = 0;
        var orderItems = new List<OrderItem>();
        for (var i = 0; i < flowerIds.Count && i < quantities.Count; i++)
        {
            var qty = quantities[i];
            if (qty <= 0)
            {
                continue;
            }
            var flower = await _flowerRepository.GetByIdAsync(flowerIds[i]);
            if (flower is null)
            {
                continue;
            }
            if (flower.StockQuantity < qty)
            {
                return new OrderCreateResult
                {
                    Success = false,
                    ErrorMessage = $"Insufficient stock for {flower.Name}. Available: {flower.StockQuantity}."
                };
            }
            total += flower.Price * qty;
            orderItems.Add(new OrderItem
            {
                FlowerId = flower.Id,
                Quantity = qty,
                UnitPrice = flower.Price
            });
            flower.StockQuantity -= qty;
        }
        order.TotalAmount = total;
        await _orderRepository.AddOrderAsync(order);
        await _orderRepository.SaveChangesAsync();
        foreach (var item in orderItems)
        {
            item.OrderId = order.Id;
        }
        await _orderRepository.AddOrderItemsAsync(orderItems);
        await _orderRepository.SaveChangesAsync();
        return new OrderCreateResult { Success = true, OrderId = order.Id };
    }
    public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _orderRepository.GetDetailsAsync(id);
        if (order is null)
        {
            return false;
        }
        order.Status = status;
        await _orderRepository.SaveChangesAsync();
        return true;
    }
    public Task<Order?> GetEditAsync(int id)
    {
        return _orderRepository.GetForEditAsync(id);
    }
    public async Task<bool> UpdateAsync(int id, Order order)
    {
        var existing = await _orderRepository.GetForEditAsync(id);
        if (existing is null)
        {
            return false;
        }
        existing.Status = order.Status;
        existing.DeliveryAddress = order.DeliveryAddress;
        existing.Notes = order.Notes;
        existing.TotalAmount = order.TotalAmount;
        existing.AssignedFloristId = order.AssignedFloristId;
        await _orderRepository.SaveChangesAsync();
        return true;
    }
    public async Task DeleteAsync(int id)
    {
        var order = await _orderRepository.GetForDeleteAsync(id);
        if (order is null)
        {
            return;
        }
        if (order.Status is OrderStatus.Pending or OrderStatus.Cancelled)
        {
            foreach (var item in order.OrderItems)
            {
                if (item.FlowerId.HasValue)
                {
                    var flower = await _flowerRepository.GetByIdAsync(item.FlowerId.Value);
                    if (flower is not null)
                    {
                        flower.StockQuantity += item.Quantity;
                    }
                }
            }
        }
        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync();
    }
}
