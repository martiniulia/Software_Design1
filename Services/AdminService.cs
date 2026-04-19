using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public Task<List<Category>> GetCategoriesAsync() => _adminRepository.GetCategoriesAsync();
    public async Task<string?> DeleteCategoryAsync(int id)
    {
        var category = await _adminRepository.GetCategoryWithFlowersAsync(id);
        if (category is null) return null;
        if (category.Flowers.Any()) return "You cannot delete a category that still has associated flowers.";
        _adminRepository.RemoveCategory(category);
        await _adminRepository.SaveChangesAsync();
        return null;
    }
    public Task<List<Flower>> GetFlowersAsync() => _adminRepository.GetFlowersAsync();
    public async Task DeleteFlowerAsync(int id)
    {
        var flower = await _adminRepository.GetFlowerByIdAsync(id);
        if (flower is null) return;
        _adminRepository.RemoveFlower(flower);
        await _adminRepository.SaveChangesAsync();
    }
    public Task<List<Bouquet>> GetBouquetsAsync() => _adminRepository.GetBouquetsAsync();
    public async Task DeleteBouquetAsync(int id)
    {
        var bouquet = await _adminRepository.GetBouquetWithFlowersAsync(id);
        if (bouquet is null) return;
        _adminRepository.RemoveBouquetFlowers(bouquet.BouquetFlowers);
        _adminRepository.RemoveBouquet(bouquet);
        await _adminRepository.SaveChangesAsync();
    }
    public Task<List<User>> GetUsersAsync() => _adminRepository.GetUsersAsync();
    public Task<List<Order>> GetOrdersAsync() => _adminRepository.GetOrdersAsync();
    public async Task<List<User>> GetFloristUsersAsync()
        => (await _adminRepository.GetUsersAsync()).Where(u => u.Role == UserRole.Florist).OrderBy(u => u.Username).ToList();
    public async Task<string?> AssignOrderFloristAsync(int orderId, int? floristUserId)
    {
        var order = await _adminRepository.GetOrderByIdAsync(orderId);
        if (order is null) return "Order not found.";
        if (!floristUserId.HasValue)
        {
            order.AssignedFloristId = null;
            await _adminRepository.SaveChangesAsync();
            return null;
        }
        var floristUser = await _adminRepository.GetFloristUserAsync(floristUserId.Value);
        if (floristUser is null) return "Selected florist user does not exist.";
        var floristProfile = await _adminRepository.GetFloristByUserIdAsync(floristUser.Id);
        if (floristProfile is null)
        {
            floristProfile = new Florist
            {
                UserId = floristUser.Id,
                Name = floristUser.Username,
                ContactEmail = floristUser.Email
            };
            await _adminRepository.AddFloristAsync(floristProfile);
            await _adminRepository.SaveChangesAsync();
        }
        order.AssignedFloristId = floristProfile.Id;
        await _adminRepository.SaveChangesAsync();
        return null;
    }
    public async Task<bool> UpdateUserRoleAsync(int userId, UserRole role)
    {
        var user = await _adminRepository.GetUserByIdAsync(userId);
        if (user is null) return false;
        user.Role = role;
        if (role == UserRole.Florist)
        {
            var floristProfile = await _adminRepository.GetFloristByUserIdAsync(user.Id);
            if (floristProfile is null)
            {
                await _adminRepository.AddFloristAsync(new Florist
                {
                    UserId = user.Id,
                    Name = user.Username,
                    ContactEmail = user.Email
                });
            }
        }
        await _adminRepository.SaveChangesAsync();
        return true;
    }
}
