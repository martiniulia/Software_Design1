using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.UserService.CQRS.Commands;

namespace FlowerShop.UserService.CQRS.Handlers;

public class UserCommandHandlers : ICommandHandler<CreateUserCommand, int>
{
    private readonly AppDbContext _context;

    public UserCommandHandlers(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> HandleAsync(CreateUserCommand command)
    {
        var user = new User
        {
            Username = command.Username,
            Email = command.Email,
            PasswordHash = command.PasswordHash, // In a real app, hash this!
            Role = Enum.Parse<UserRole>(command.Role, true)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }
}
