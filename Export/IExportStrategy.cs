namespace FlowerShop.Export;

public interface IExportStrategy
{
    string Format { get; }
    string ContentType { get; }
    string FileExtension { get; }
    byte[] Export<T>(IEnumerable<T> items);
}
