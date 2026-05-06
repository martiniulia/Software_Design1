namespace FlowerShop.Events;

public interface IEventBus
{
    void Subscribe<T>(IEventHandler<T> handler) where T : IDomainEvent;
    void Publish<T>(T domainEvent) where T : IDomainEvent;
}
