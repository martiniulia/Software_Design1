using FlowerShop.CatalogService.CQRS.Commands;
using FlowerShop.CatalogService.CQRS.Queries;
using FlowerShop.Core.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlowersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public FlowersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _queryDispatcher.DispatchAsync<GetAllFlowersQuery, List<FlowerShop.Models.Flower>>(new GetAllFlowersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _queryDispatcher.DispatchAsync<GetFlowerByIdQuery, FlowerShop.Models.Flower?>(new GetFlowerByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlowerCommand command)
    {
        var id = await _commandDispatcher.DispatchAsync<CreateFlowerCommand, int>(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }
}
