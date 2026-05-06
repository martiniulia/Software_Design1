using FlowerShop.Core.CQRS;
using FlowerShop.Models;
using FlowerShop.UserService.CQRS.Commands;
using FlowerShop.UserService.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UsersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _queryDispatcher.DispatchAsync<GetUserByIdQuery, User?>(new GetUserByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var id = await _commandDispatcher.DispatchAsync<CreateUserCommand, int>(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }
}
