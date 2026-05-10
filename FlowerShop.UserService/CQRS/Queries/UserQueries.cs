using FlowerShop.Core.CQRS;
using FlowerShop.Models;

namespace FlowerShop.UserService.CQRS.Queries;

public class GetUserByIdQuery : IQuery<User?>
{
    public int Id { get; set; }
    public GetUserByIdQuery(int id) { Id = id; }
}
