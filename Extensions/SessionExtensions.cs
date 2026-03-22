using System.Text.Json;
namespace FlowerShop.Extensions;
public static class SessionExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public static void SetJson<T>(this ISession session, string key, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        session.SetString(key, json);
    }
    public static T? GetJson<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(data, JsonOptions);
        }
        catch (JsonException)
        {
            session.Remove(key);
            return default;
        }
    }
}
