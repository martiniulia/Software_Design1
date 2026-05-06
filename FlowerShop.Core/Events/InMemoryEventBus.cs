using System.Collections.Concurrent;

namespace FlowerShop.Events;

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();

    public void Subscribe<T>(IEventHandler<T> handler) where T : IDomainEvent
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
        {
            _handlers[type] = new List<object>();
        }
        _handlers[type].Add(handler);
    }

    public void Publish<T>(T domainEvent) where T : IDomainEvent
    {
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (handler is IEventHandler<T> typedHandler)
                {
                    typedHandler.HandleAsync(domainEvent).GetAwaiter().GetResult();
                }
            }
        }
    }
}
