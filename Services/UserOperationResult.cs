namespace FlowerShop.Services;
public sealed class UserOperationResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
