using FlowerShop.Export;
using FlowerShop.Models;
using Xunit;
using System.Text;

namespace FlowerShop.Tests;

public class ExportTests
{
    [Fact]
    public void JsonExporter_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var exporter = new JsonExporter<FlowerExportDto>();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = exporter.Export(dtos);
        var jsonString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", jsonString);
        Assert.Equal("application/json", exporter.ContentType);
        Assert.Equal("json", exporter.FileExtension);
    }

    [Fact]
    public void XmlExporter_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var exporter = new XmlExporter<FlowerExportDto>();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = exporter.Export(dtos);
        var xmlString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", xmlString);
        Assert.Equal("application/xml", exporter.ContentType);
        Assert.Equal("xml", exporter.FileExtension);
    }

    [Fact]
    public void CsvExporter_ShouldReturnNonEmptyBytes_ForValidInput()
    {
        var exporter = new CsvExporter<FlowerExportDto>();
        var dtos = new List<FlowerExportDto>
        {
            new FlowerExportDto { Id = 1, Name = "Rose", Price = 10, Stock = 50, CategoryName = "Roses", FloristName = "Admin" }
        };

        var bytes = exporter.Export(dtos);
        var csvString = Encoding.UTF8.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Contains("Rose", csvString);
        Assert.Contains("10", csvString);
        Assert.Equal("text/csv", exporter.ContentType);
        Assert.Equal("csv", exporter.FileExtension);
    }
}
