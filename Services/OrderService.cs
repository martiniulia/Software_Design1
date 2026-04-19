using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IFlowerRepository _flowerRepository;
    private readonly IBouquetRepository _bouquetRepository;
    public OrderService(
        IOrderRepository orderRepository,
        IFlowerRepository flowerRepository,
        IBouquetRepository bouquetRepository)
    {
        _orderRepository = orderRepository;
        _flowerRepository = flowerRepository;
        _bouquetRepository = bouquetRepository;
    }
    public async Task<PlaceOrderResult> PlaceOrderAsync(int userId, IReadOnlyCollection<CartItem> cartItems)
    {
        if (cartItems.Count == 0)
        {
            return new PlaceOrderResult { Success = false, ErrorMessage = "Cart is empty." };
        }
        await using var transaction = await _orderRepository.BeginTransactionAsync();
        try
        {
            foreach (var item in cartItems)
            {
                if (!item.IsBouquet)
                {
                    var flower = await _flowerRepository.GetByIdAsync(item.Id);
                    if (flower is null)
                    {
                        return new PlaceOrderResult { Success = false, ErrorMessage = "Flower was not found." };
                    }
                    if (flower.StockQuantity < item.Quantity)
                    {
                        return new PlaceOrderResult
                        {
                            Success = false,
                            ErrorMessage = $"Insufficient stock for {flower.Name}. Available: {flower.StockQuantity}."
                        };
                    }
                }
                else
                {
                    var bouquet = await _bouquetRepository.GetByIdWithFlowersAsync(item.Id);
                    if (bouquet is null)
                    {
                        return new PlaceOrderResult { Success = false, ErrorMessage = "Bouquet was not found." };
                    }
                    foreach (var bf in bouquet.BouquetFlowers)
                    {
                        var flower = await _flowerRepository.GetByIdAsync(bf.FlowerId);
                        if (flower is null)
                        {
                            continue;
                        }
                        var requiredQty = bf.Quantity * item.Quantity;
                        if (flower.StockQuantity < requiredQty)
                        {
                            return new PlaceOrderResult
                            {
                                Success = false,
                                ErrorMessage = $"Insufficient stock for {flower.Name}. Available: {flower.StockQuantity}."
                            };
                        }
                    }
                }
            }
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity)
            };
            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();
            var orderItems = new List<OrderItem>();
            foreach (var item in cartItems)
            {
                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    BouquetId = item.IsBouquet ? item.Id : null,
                    FlowerId = item.IsBouquet ? null : item.Id
                });
                if (!item.IsBouquet)
                {
                    var flower = await _flowerRepository.GetByIdAsync(item.Id);
                    if (flower is not null)
                    {
                        flower.StockQuantity -= item.Quantity;
                    }
                }
                else
                {
                    var bouquet = await _bouquetRepository.GetByIdWithFlowersAsync(item.Id);
                    if (bouquet is not null)
                    {
                        foreach (var bf in bouquet.BouquetFlowers)
                        {
                            var flower = await _flowerRepository.GetByIdAsync(bf.FlowerId);
                            if (flower is not null)
                            {
                                flower.StockQuantity -= bf.Quantity * item.Quantity;
                            }
                        }
                    }
                }
            }
            await _orderRepository.AddOrderItemsAsync(orderItems);
            await _orderRepository.SaveChangesAsync();
            await transaction.CommitAsync();
            return new PlaceOrderResult { Success = true };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
