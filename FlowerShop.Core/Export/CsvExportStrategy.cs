using System.Text;

namespace FlowerShop.Export;

public class CsvExporter<T> : DataExporter<T>
{
    public CsvExporter()
    {
        Format = "csv";
        ContentType = "text/csv";
        FileExtension = "csv";
    }

    protected override string TransformData(IEnumerable<T> data)
    {
        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", properties.Select(p => EscapeCsv(p.Name))));

        foreach (var item in data)
        {
            var values = properties.Select(p => 
            {
                var val = p.GetValue(item)?.ToString() ?? string.Empty;
                return EscapeCsv(val);
            });
            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
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
