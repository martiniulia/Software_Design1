using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;
    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<List<Category>> GetCategoriesAsync() => _context.Categories.OrderBy(c => c.Name).ToListAsync();
    public Task<Category?> GetCategoryWithFlowersAsync(int id) => _context.Categories.Include(c => c.Flowers).FirstOrDefaultAsync(c => c.Id == id);
    public void RemoveCategory(Category category) => _context.Categories.Remove(category);
    public Task<List<Flower>> GetFlowersAsync() => _context.Flowers.Include(f => f.Category).Include(f => f.Florist).OrderBy(f => f.Name).ToListAsync();
    public Task<Flower?> GetFlowerByIdAsync(int id) => _context.Flowers.FindAsync(id).AsTask();
    public void RemoveFlower(Flower flower) => _context.Flowers.Remove(flower);
    public Task<List<Bouquet>> GetBouquetsAsync() => _context.Bouquets.Include(b => b.BouquetFlowers).OrderBy(b => b.Name).ToListAsync();
    public Task<Bouquet?> GetBouquetWithFlowersAsync(int id) => _context.Bouquets.Include(b => b.BouquetFlowers).FirstOrDefaultAsync(b => b.Id == id);
    public void RemoveBouquet(Bouquet bouquet) => _context.Bouquets.Remove(bouquet);
    public void RemoveBouquetFlowers(IEnumerable<BouquetFlower> bouquetFlowers) => _context.BouquetFlowers.RemoveRange(bouquetFlowers);
    public Task<List<User>> GetUsersAsync() => _context.Users.OrderBy(u => u.Username).ToListAsync();
    public Task<User?> GetUserByIdAsync(int userId) => _context.Users.FindAsync(userId).AsTask();
    public Task<List<Order>> GetOrdersAsync() => _context.Orders.Include(o => o.User).Include(o => o.AssignedFlorist).Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();
    public Task<Order?> GetOrderByIdAsync(int orderId) => _context.Orders.FindAsync(orderId).AsTask();
    public Task<User?> GetFloristUserAsync(int userId) => _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRole.Florist);
    public Task<Florist?> GetFloristByUserIdAsync(int userId) => _context.Florists.FirstOrDefaultAsync(f => f.UserId == userId);
    public Task AddFloristAsync(Florist florist) => _context.Florists.AddAsync(florist).AsTask();
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
