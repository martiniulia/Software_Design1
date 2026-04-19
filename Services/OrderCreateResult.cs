namespace FlowerShop.Services;
public sealed class OrderCreateResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int? OrderId { get; init; }
}
