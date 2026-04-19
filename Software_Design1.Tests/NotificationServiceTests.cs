using FlowerShop.Events;
using FlowerShop.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class NotificationServiceTests
{
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _loggerMock = new Mock<ILogger<NotificationService>>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Smtp:AdminEmail"]).Returns("admin@test.com");

        _notificationService = new NotificationService(_emailSenderMock.Object, _loggerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task HandleAsync_FlowerCreatedEvent_ShouldSendEmail()
    {
        var @event = new FlowerCreatedEvent { FlowerId = 1, FlowerName = "Rose", TriggeredByEmail = "user@test.com" };

        await _notificationService.HandleAsync(@event);

        _emailSenderMock.Verify(es => es.SendAsync(
            "user@test.com", 
            "Flower Created", 
            It.Is<string>(body => body.Contains("was created on") && body.Contains("Rose"))
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_FlowerUpdatedEvent_ShouldSendEmail_WithChanges()
    {
        var @event = new FlowerUpdatedEvent 
        { 
            FlowerId = 1, 
            FlowerName = "White Rose", 
            TriggeredByEmail = "user@test.com",
            Changes = new List<string> { "Price changed from 10 to 15" }
        };

        await _notificationService.HandleAsync(@event);

        _emailSenderMock.Verify(es => es.SendAsync(
            "user@test.com", 
            "Flower Updated", 
            It.Is<string>(body => body.Contains("was updated on") && body.Contains("Price changed from 10 to 15"))
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_FlowerDeletedEvent_ShouldSendEmail()
    {
        var @event = new FlowerDeletedEvent { FlowerId = 1, FlowerName = "Rose", TriggeredByEmail = "user@test.com" };

        await _notificationService.HandleAsync(@event);

        _emailSenderMock.Verify(es => es.SendAsync(
            "user@test.com", 
            "Flower Deleted", 
            It.Is<string>(body => body.Contains("was deleted on") && body.Contains("Rose"))
        ), Times.Once);
    }
}
