using Microsoft.Extensions.DependencyInjection;

namespace Senf.EventSourcing.Core.Commands;

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
        var handler = this.serviceProvider.GetRequiredService<IHandleCommand<TCommand>>();

        await handler.Handle(command, ct)
                     .ConfigureAwait(false);
    }
}
