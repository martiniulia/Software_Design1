using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.UserService.CQRS.Queries;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UserService.CQRS.Handlers;

public class UserQueryHandlers : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly AppDbContext _context;

    public UserQueryHandlers(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> HandleAsync(GetUserByIdQuery query)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == query.Id);
    }
}
