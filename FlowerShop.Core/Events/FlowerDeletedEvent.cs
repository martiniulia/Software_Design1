namespace FlowerShop.Events;

public class FlowerDeletedEvent : IDomainEvent
{
    public int FlowerId { get; set; }
    public string FlowerName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TriggeredByEmail { get; set; } = string.Empty;
}
