using System.Xml.Serialization;

namespace FlowerShop.Export;

public class XmlExportStrategy : IExportStrategy
{
    public string Format => "xml";
    public string ContentType => "application/xml";
    public string FileExtension => "xml";

    public byte[] Export<T>(IEnumerable<T> items)
    {
        var serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute("ExportData"));
        using var memoryStream = new MemoryStream();
        serializer.Serialize(memoryStream, items.ToList());
        return memoryStream.ToArray();
    }
}
