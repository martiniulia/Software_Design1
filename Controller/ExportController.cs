using FlowerShop.Export;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Controllers;

[AllowAnonymous]
public class ExportController : Controller
{
    private readonly IFlowerService _flowerService;
    private readonly ExportStrategyFactory _exportFactory;

    public ExportController(IFlowerService flowerService, ExportStrategyFactory exportFactory)
    {
        _flowerService = flowerService;
        _exportFactory = exportFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Flowers(string format = "json", string? sortBy = "Name", bool sortDir = true, string? filterField = null, string? filterValue = null)
    {
        var role = HttpContext.Session.GetString("UserRole");
        var isVisitor = string.IsNullOrEmpty(role);

        if (isVisitor)
        {
            filterField = null;
            filterValue = null;
        }

        var search = filterValue; 
        if (!string.IsNullOrEmpty(filterField)) 
        {
            search = filterValue;
        }

        var flowers = await _flowerService.GetIndexDataAsync(search, sortBy, sortDir);

        var dtos = flowers.Select(f => new FlowerExportDto
        {
            Id = f.Id,
            Name = f.Name,
            Price = f.Price,
            Stock = f.StockQuantity,
            CategoryName = f.Category?.Name ?? string.Empty,
            FloristName = f.Florist?.User?.Username ?? string.Empty
        }).ToList();

        var strategy = _exportFactory.GetStrategy(format);
        var bytes = strategy.Export(dtos);

        return File(bytes, strategy.ContentType, $"flowers_{DateTime.Now:yyyy-MM-dd}.{strategy.FileExtension}");
    }
}
