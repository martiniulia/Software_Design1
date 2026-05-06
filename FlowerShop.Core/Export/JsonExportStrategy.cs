using System.Text.Json;

namespace FlowerShop.Export;

public class JsonExporter<T> : DataExporter<T>
{
    public JsonExporter()
    {
        Format = "json";
        ContentType = "application/json";
        FileExtension = "json";
    }

    protected override string TransformData(IEnumerable<T> data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    }
}
