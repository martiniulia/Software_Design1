namespace FlowerShop.Events;

public class FlowerUpdatedEvent : IDomainEvent
{
    public int FlowerId { get; set; }
    public string FlowerName { get; set; } = string.Empty;
    public string OldName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TriggeredByEmail { get; set; } = string.Empty;
    public List<string> Changes { get; set; } = new();
}
