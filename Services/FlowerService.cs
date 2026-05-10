using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
using FlowerShop.Events;
namespace FlowerShop.Services;
public class FlowerService : IFlowerService
{
    private readonly IFlowerRepository _flowerRepository;
    private readonly IEventBus _eventBus;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public FlowerService(IFlowerRepository flowerRepository, IEventBus eventBus, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
        _flowerRepository = flowerRepository;
        _eventBus = eventBus;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<List<Flower>> GetIndexDataAsync(string? search, string? sortBy, bool ascending)
    {
        var client = _httpClientFactory.CreateClient("CatalogService");
        var response = await client.GetAsync("api/flowers");
        if (response.IsSuccessStatusCode)
        {
            var flowers = await response.Content.ReadFromJsonAsync<List<Flower>>() ?? new List<Flower>();
            
            if (!string.IsNullOrWhiteSpace(search))
                flowers = flowers.Where(f => f.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || f.Category?.Name.Contains(search, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (sortBy == "price")
                flowers = ascending ? flowers.OrderBy(f => f.Price).ToList() : flowers.OrderByDescending(f => f.Price).ToList();
            else if (sortBy == "name")
                flowers = ascending ? flowers.OrderBy(f => f.Name).ToList() : flowers.OrderByDescending(f => f.Name).ToList();

            return flowers;
        }
        return await _flowerRepository.GetAllWithFiltersAsync(search, sortBy, ascending);
    }
    public async Task<Flower?> GetDetailsAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("CatalogService");
        var response = await client.GetAsync($"api/flowers/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Flower>();
        }
        return await _flowerRepository.GetByIdWithRelationsAsync(id);
    }
    public Task<List<Category>> GetCategoriesAsync()
    {
        return _flowerRepository.GetAllCategoriesAsync();
    }
    public Task<List<Florist>> GetFloristsAsync()
    {
        return _flowerRepository.GetAllFloristsAsync();
    }
    public async Task CreateAsync(Flower flower)
    {
        var client = _httpClientFactory.CreateClient("CatalogService");
        var command = new {
            flower.Name,
            flower.Description,
            flower.Price,
            flower.StockQuantity,
            flower.Color,
            flower.ImageUrl,
            flower.CategoryId,
            flower.FloristId
        };
        var response = await client.PostAsJsonAsync("api/flowers", command);

        if (!response.IsSuccessStatusCode)
        {
            await _flowerRepository.AddAsync(flower);
            await _flowerRepository.SaveChangesAsync();
        }

        var email = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("UserEmail") 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("Username") 
                    ?? "Unknown";
        _eventBus.Publish(new FlowerShop.Events.FlowerCreatedEvent
        {
            FlowerId = flower.Id,
            FlowerName = flower.Name,
            Price = flower.Price,
            Stock = flower.StockQuantity,
            TriggeredByEmail = email
        });
    }
    public async Task<bool> UpdateAsync(int id, Flower flower)
    {
        var existing = await _flowerRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        var changes = new List<string>();

        if (existing.Name != flower.Name)
            changes.Add($"Name changed from '{existing.Name}' to '{flower.Name}'");
        if (existing.Description != flower.Description)
            changes.Add($"Description changed from '{existing.Description}' to '{flower.Description}'");
        if (existing.Price != flower.Price)
            changes.Add($"Price changed from {existing.Price} to {flower.Price}");
        if (existing.StockQuantity != flower.StockQuantity)
            changes.Add($"Stock changed from {existing.StockQuantity} to {flower.StockQuantity}");
        if (existing.Color != flower.Color)
            changes.Add($"Color changed from '{existing.Color}' to '{flower.Color}'");
        if (existing.CategoryId != flower.CategoryId)
            changes.Add($"CategoryId changed from {existing.CategoryId} to {flower.CategoryId}");
        if (existing.FloristId != flower.FloristId)
            changes.Add($"FloristId changed from {existing.FloristId} to {flower.FloristId}");

        if (string.IsNullOrWhiteSpace(flower.ImageUrl))
        {
            flower.ImageUrl = existing.ImageUrl;
        }
        else if (existing.ImageUrl != flower.ImageUrl)
        {
            changes.Add($"ImageUrl changed from '{existing.ImageUrl}' to '{flower.ImageUrl}'");
        }

        existing.Name = flower.Name;
        existing.Description = flower.Description;
        existing.Price = flower.Price;
        existing.StockQuantity = flower.StockQuantity;
        existing.Color = flower.Color;
        existing.CategoryId = flower.CategoryId;
        existing.FloristId = flower.FloristId;
        existing.ImageUrl = flower.ImageUrl;

        await _flowerRepository.SaveChangesAsync();

        var email = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("UserEmail") 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("Username") 
                    ?? "Unknown";
        
        _eventBus.Publish(new FlowerShop.Events.FlowerUpdatedEvent
        {
            FlowerId = existing.Id,
            FlowerName = existing.Name,
            OldName = existing.Name,
            Price = existing.Price,
            Stock = existing.StockQuantity,
            TriggeredByEmail = email,
            Changes = changes
        });

        return true;
    }
    public async Task DeleteAsync(int id)
    {
        var flower = await _flowerRepository.GetByIdAsync(id);
        if (flower is null)
        {
            return;
        }

        var flowerName = flower.Name;

        _flowerRepository.Remove(flower);
        await _flowerRepository.SaveChangesAsync();

        var email = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("UserEmail") 
                    ?? _httpContextAccessor.HttpContext?.Session?.GetString("Username") 
                    ?? "Unknown";
        _eventBus.Publish(new FlowerShop.Events.FlowerDeletedEvent
        {
            FlowerId = id,
            FlowerName = flowerName,
            TriggeredByEmail = email
        });
    }
}
