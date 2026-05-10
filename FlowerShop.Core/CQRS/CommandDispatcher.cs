namespace FlowerShop.Core.CQRS;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
}

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = (ICommandHandler<TCommand>)_serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
        if (handler == null) throw new InvalidOperationException($"Handler not found for command {typeof(TCommand).Name}");
        await handler.HandleAsync(command);
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        var handler = (ICommandHandler<TCommand, TResult>)_serviceProvider.GetService(typeof(ICommandHandler<TCommand, TResult>));
        if (handler == null) throw new InvalidOperationException($"Handler not found for command {typeof(TCommand).Name}");
        return await handler.HandleAsync(command);
    }
}
