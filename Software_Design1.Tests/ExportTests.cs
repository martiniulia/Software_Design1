using FlowerShop.Export;
using FlowerShop.Models;
using Xunit;
using System.Text;

namespace FlowerShop.Tests;

public class ExportTests
{
    [Fact]
    public void ExportStrategyFactory_ShouldReturnCorrectStrategies()
    {
        var strategies = new List<IExportStrategy>
        {
            new JsonExportStrategy(),
            new XmlExportStrategy(),
            new CsvExportStrategy()
        };
        var factory = new ExportStrategyFactory(strategies);

        Assert.IsType<JsonExportStrategy>(factory.GetStrategy("json"));
        Assert.IsType<XmlExportStrategy>(factory.GetStrategy("xml"));
        Assert.IsType<CsvExportStrategy>(factory.GetStrategy("csv"));
        Assert.Throws<NotSupportedException>(() => factory.GetStrategy("invalid_format"));
    }

    [Fact]
    public void JsonExportStrategy_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var strategy = new JsonExportStrategy();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = strategy.Export(dtos);
        var jsonString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", jsonString);
        Assert.Equal("application/json", strategy.ContentType);
        Assert.Equal("json", strategy.FileExtension);
    }

    [Fact]
    public void XmlExportStrategy_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var strategy = new XmlExportStrategy();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = strategy.Export(dtos);
        var xmlString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", xmlString);
        Assert.Equal("application/xml", strategy.ContentType);
        Assert.Equal("xml", strategy.FileExtension);
    }

    [Fact]
    public void CsvExportStrategy_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var strategy = new CsvExportStrategy();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = strategy.Export(dtos);
        var csvString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", csvString);
        Assert.Contains("10", csvString);
        Assert.Equal("text/csv", strategy.ContentType);
        Assert.Equal("csv", strategy.FileExtension);
    }
}
