using FlowerShop.Models;

namespace FlowerShop.Export;

public class ExportStrategyFactory
{
    private readonly IEnumerable<DataExporter<FlowerExportDto>> _strategies;

    public ExportStrategyFactory(IEnumerable<DataExporter<FlowerExportDto>> strategies)
    {
        _strategies = strategies;
    }

    public DataExporter<FlowerExportDto> GetStrategy(string format)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        if (strategy == null)
            throw new NotSupportedException($"Export format '{format}' is not supported.");
            
        return strategy;
    }
}
