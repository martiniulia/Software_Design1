using FlowerShop.Core.CQRS;
using FlowerShop.Models;

namespace FlowerShop.OrderService.CQRS.Queries;

public class GetOrderByIdQuery : IQuery<Order?>
{
    public int Id { get; set; }
    public GetOrderByIdQuery(int id) { Id = id; }
}

public class GetOrdersByUserIdQuery : IQuery<List<Order>>
{
    public int UserId { get; set; }
    public GetOrdersByUserIdQuery(int userId) { UserId = userId; }
}
