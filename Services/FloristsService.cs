using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class FloristsService : IFloristsService
{
    private readonly IFloristRepository _floristRepository;
    public FloristsService(IFloristRepository floristRepository)
    {
        _floristRepository = floristRepository;
    }
    public Task<List<Florist>> GetIndexAsync(string? search)
    {
        return _floristRepository.GetAllAsync(search);
    }
    public Task<Florist?> GetDetailsAsync(int id)
    {
        return _floristRepository.GetByIdWithFlowersAsync(id);
    }
    public async Task CreateAsync(Florist florist)
    {
        await _floristRepository.AddAsync(florist);
        await _floristRepository.SaveChangesAsync();
    }
    public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, Florist florist)
    {
        var existing = await _floristRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return (false, "Florist not found.");
        }
        existing.Name = florist.Name;
        existing.Specialization = florist.Specialization;
        existing.Bio = florist.Bio;
        existing.ContactEmail = florist.ContactEmail;
        existing.Phone = florist.Phone;
        await _floristRepository.SaveChangesAsync();
        return (true, null);
    }
    public async Task<(bool Success, string? ErrorMessage, Florist? Florist)> DeleteAsync(int id)
    {
        var florist = await _floristRepository.GetByIdWithFlowersAsync(id);
        if (florist is null)
        {
            return (true, null, null);
        }
        if (florist.Flowers.Any())
        {
            return (false, "You cannot delete a florist that still has associated flowers.", florist);
        }
        _floristRepository.Remove(florist);
        await _floristRepository.SaveChangesAsync();
        return (true, null, null);
    }
}
