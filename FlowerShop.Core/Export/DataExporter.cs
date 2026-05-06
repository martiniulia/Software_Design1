using System.Collections.Generic;
using System.Text;

namespace FlowerShop.Export;

// Template Pattern: Defines the skeleton of the algorithm in a method
public abstract class DataExporter<T>
{
    public string Format { get; protected set; } = string.Empty;
    public string ContentType { get; protected set; } = string.Empty;
    public string FileExtension { get; protected set; } = string.Empty;

    // Template Method
    public byte[] Export(IEnumerable<T> items)
    {
        var fetchedData = FetchData(items);
        var transformedData = TransformData(fetchedData);
        return WriteOutput(transformedData);
    }

    // Default implementation can just return the same items
    protected virtual IEnumerable<T> FetchData(IEnumerable<T> items)
    {
        return items; 
    }

    // Abstract methods to be implemented by subclasses
    protected abstract string TransformData(IEnumerable<T> data);

    protected virtual byte[] WriteOutput(string transformedData)
    {
        return Encoding.UTF8.GetBytes(transformedData);
    }
}
