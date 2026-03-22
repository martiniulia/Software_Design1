namespace FlowerShop.Services;
public sealed class PlaceOrderResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
