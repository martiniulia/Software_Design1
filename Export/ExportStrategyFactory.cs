namespace FlowerShop.Export;

public class ExportStrategyFactory
{
    private readonly IEnumerable<IExportStrategy> _strategies;

    public ExportStrategyFactory(IEnumerable<IExportStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IExportStrategy GetStrategy(string format)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        if (strategy == null)
            throw new NotSupportedException($"Export format '{format}' is not supported.");
            
        return strategy;
    }
}
