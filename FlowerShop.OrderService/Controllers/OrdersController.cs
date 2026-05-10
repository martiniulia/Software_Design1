using FlowerShop.Core.CQRS;
using FlowerShop.Models;
using FlowerShop.OrderService.CQRS.Commands;
using FlowerShop.OrderService.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public OrdersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _queryDispatcher.DispatchAsync<GetOrderByIdQuery, Order?>(new GetOrderByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var result = await _queryDispatcher.DispatchAsync<GetOrdersByUserIdQuery, List<Order>>(new GetOrdersByUserIdQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var id = await _commandDispatcher.DispatchAsync<CreateOrderCommand, int>(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }
}
