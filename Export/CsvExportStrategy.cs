using System.Text;

namespace FlowerShop.Export;

public class CsvExportStrategy : IExportStrategy
{
    public string Format => "csv";
    public string ContentType => "text/csv";
    public string FileExtension => "csv";

    public byte[] Export<T>(IEnumerable<T> items)
    {
        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", properties.Select(p => EscapeCsv(p.Name))));

        foreach (var item in items)
        {
            var values = properties.Select(p => 
            {
                var val = p.GetValue(item)?.ToString() ?? string.Empty;
                return EscapeCsv(val);
            });
            sb.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private string EscapeCsv(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
