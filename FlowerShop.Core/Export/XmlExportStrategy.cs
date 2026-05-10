using System.Xml.Serialization;

namespace FlowerShop.Export;

public class XmlExporter<T> : DataExporter<T>
{
    public XmlExporter()
    {
        Format = "xml";
        ContentType = "application/xml";
        FileExtension = "xml";
    }

    protected override string TransformData(IEnumerable<T> data)
    {
        var list = data.ToList();
        var serializer = new XmlSerializer(typeof(List<T>));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, list);
        return stringWriter.ToString();
    }
}
