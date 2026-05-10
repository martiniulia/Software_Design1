using FlowerShop.Core.CQRS;

namespace FlowerShop.CatalogService.CQRS.Commands;

public class CreateFlowerCommand : ICommand<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Color { get; set; }
    public int? CategoryId { get; set; }
    public int? FloristId { get; set; }
    public string? ImageUrl { get; set; }
}
