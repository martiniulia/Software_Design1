using FlowerShop.Core.CQRS;
using FlowerShop.Models;

namespace FlowerShop.CatalogService.CQRS.Queries;

public class GetAllFlowersQuery : IQuery<List<Flower>>
{
}

public class GetFlowerByIdQuery : IQuery<Flower?>
{
    public int Id { get; set; }
    public GetFlowerByIdQuery(int id) { Id = id; }
}
