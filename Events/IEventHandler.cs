namespace FlowerShop.Events;

public interface IEventHandler<in T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}
