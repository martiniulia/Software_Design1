using System.Text.Json;

namespace FlowerShop.Export;

public class JsonExportStrategy : IExportStrategy
{
    public string Format => "json";
    public string ContentType => "application/json";
    public string FileExtension => "json";

    public byte[] Export<T>(IEnumerable<T> items)
    {
        return JsonSerializer.SerializeToUtf8Bytes(items, new JsonSerializerOptions { WriteIndented = true });
    }
}
