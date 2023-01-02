using Microsoft.Extensions.DependencyInjection;

namespace Kuna.EventSourcing.Core.Commands;

public interface ICommandDispatcher
{
    Task Send<TCommand>(TCommand command, CancellationToken ct)
        where TCommand : class, ICommand;
}

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Send<TCommand>(TCommand command, CancellationToken ct)
        where TCommand : class, ICommand
    {
        using var scope = this.serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IHandleCommand<TCommand>>();

        await handler.Handle(command, ct)
                     .ConfigureAwait(false);
    }
}
