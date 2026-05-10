using FlowerShop.Events;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class InMemoryEventBusTests
{
    [Fact]
    public void Publish_ShouldInvokeAllSubscribedHandlers()
    {
        var eventBus = new InMemoryEventBus();
        var handlerMock1 = new Mock<IEventHandler<FlowerCreatedEvent>>();
        var handlerMock2 = new Mock<IEventHandler<FlowerCreatedEvent>>();

        eventBus.Subscribe(handlerMock1.Object);
        eventBus.Subscribe(handlerMock2.Object);

        var myEvent = new FlowerCreatedEvent { FlowerId = 1, FlowerName = "Test" };

        eventBus.Publish(myEvent);

        handlerMock1.Verify(h => h.HandleAsync(myEvent), Times.Once);
        handlerMock2.Verify(h => h.HandleAsync(myEvent), Times.Once);
    }
}
