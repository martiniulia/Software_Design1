using FlowerShop.CatalogService.CQRS.Commands;
using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;

namespace FlowerShop.CatalogService.CQRS.Handlers;

public class CreateFlowerCommandHandler : ICommandHandler<CreateFlowerCommand, int>
{
    private readonly AppDbContext _context;

    public CreateFlowerCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> HandleAsync(CreateFlowerCommand command)
    {
        var flower = new Flower
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            StockQuantity = command.StockQuantity,
            Color = command.Color,
            CategoryId = command.CategoryId,
            FloristId = command.FloristId,
            ImageUrl = command.ImageUrl
        };

        _context.Flowers.Add(flower);
        await _context.SaveChangesAsync();
        
        return flower.Id;
    }
}
