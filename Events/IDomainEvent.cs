namespace FlowerShop.Events;

public interface IDomainEvent
{
    DateTime Timestamp { get; }
    string TriggeredByEmail { get; }
}
