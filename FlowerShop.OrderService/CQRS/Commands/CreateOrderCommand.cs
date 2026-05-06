using FlowerShop.Core.CQRS;

namespace FlowerShop.OrderService.CQRS.Commands;

public class CreateOrderCommand : ICommand<int>
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? DeliveryAddress { get; set; }
}
