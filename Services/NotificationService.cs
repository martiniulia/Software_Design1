using FlowerShop.Events;

namespace FlowerShop.Services;

public class NotificationService : 
    IEventHandler<FlowerCreatedEvent>,
    IEventHandler<FlowerUpdatedEvent>,
    IEventHandler<FlowerDeletedEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;

    public NotificationService(IEmailSender emailSender, ILogger<NotificationService> logger, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task HandleAsync(FlowerCreatedEvent domainEvent)
    {
        var message = $"Hello, a flower '{domainEvent.FlowerName}' was created on {domainEvent.Timestamp:yyyy-MM-dd}.";
        _logger.LogInformation(message);
        
        var toAddress = !string.IsNullOrEmpty(domainEvent.TriggeredByEmail) && domainEvent.TriggeredByEmail != "Unknown" 
            ? domainEvent.TriggeredByEmail 
            : _configuration["Smtp:AdminEmail"] ?? "admin@flowershop.com";
            
        await _emailSender.SendAsync(toAddress, "Flower Created", message);
    }

    public async Task HandleAsync(FlowerUpdatedEvent domainEvent)
    {
        var changesInfo = domainEvent.Changes.Any() ? $" The following modifications were made: {string.Join("; ", domainEvent.Changes)}." : "";
        var message = $"Hello, a flower '{domainEvent.FlowerName}' was updated on {domainEvent.Timestamp:yyyy-MM-dd}.{changesInfo}";
        _logger.LogInformation(message);

        var toAddress = !string.IsNullOrEmpty(domainEvent.TriggeredByEmail) && domainEvent.TriggeredByEmail != "Unknown" 
            ? domainEvent.TriggeredByEmail 
            : _configuration["Smtp:AdminEmail"] ?? "admin@flowershop.com";

        await _emailSender.SendAsync(toAddress, "Flower Updated", message);
    }

    public async Task HandleAsync(FlowerDeletedEvent domainEvent)
    {
        var message = $"Hello, a flower '{domainEvent.FlowerName}' was deleted on {domainEvent.Timestamp:yyyy-MM-dd}.";
        _logger.LogInformation(message);

        var toAddress = !string.IsNullOrEmpty(domainEvent.TriggeredByEmail) && domainEvent.TriggeredByEmail != "Unknown" 
            ? domainEvent.TriggeredByEmail 
            : _configuration["Smtp:AdminEmail"] ?? "admin@flowershop.com";

        await _emailSender.SendAsync(toAddress, "Flower Deleted", message);
    }
}
