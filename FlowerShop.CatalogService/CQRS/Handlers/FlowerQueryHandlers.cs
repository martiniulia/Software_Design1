using FlowerShop.CatalogService.CQRS.Queries;
using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.CatalogService.CQRS.Handlers;

public class FlowerQueryHandlers : 
    IQueryHandler<GetAllFlowersQuery, List<Flower>>,
    IQueryHandler<GetFlowerByIdQuery, Flower?>
{
    private readonly AppDbContext _context;

    public FlowerQueryHandlers(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Flower>> HandleAsync(GetAllFlowersQuery query)
    {
        return await _context.Flowers
            .Include(f => f.Category)
            .Include(f => f.Florist)
            .ThenInclude(f => f!.User)
            .ToListAsync();
    }

    public async Task<Flower?> HandleAsync(GetFlowerByIdQuery query)
    {
        return await _context.Flowers
            .Include(f => f.Category)
            .Include(f => f.Florist)
            .FirstOrDefaultAsync(f => f.Id == query.Id);
    }
}
