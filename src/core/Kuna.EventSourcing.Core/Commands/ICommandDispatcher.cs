using Microsoft.Extensions.DependencyInjection;

namespace Kuna.EventSourcing.Core.Commands;

public interface ICommandDispatcher
{
    Task Send<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand;

    Task<TReturn> Send<TReturn>(ICommand<TReturn> command, CancellationToken ct = default);
}

/// <summary>
/// Command dispatcher takes advantage of container to resolve handlers for command.
/// It must be registered as a Singleton
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    // map used for resolving handlers for commands that implement ICommand<TReturn> interface
    private ConcurrentDictionary<Type, (Type, MethodInfo)> cmdToHandlerMap = new();

    private readonly IServiceProvider serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Send<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        using var scope = this.serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IHandleCommand<TCommand>>();

        await handler.Handle(command, ct)
                     .ConfigureAwait(false);
    }

    public async Task<TReturn> Send<TReturn>(ICommand<TReturn> command, CancellationToken ct = default)
    {
        using var scope = this.serviceProvider.CreateScope();

        var (handlerType, methodInfo) = GetHandlerTypeFor(command, ref this.cmdToHandlerMap);

        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

#nullable disable
        var result = await (Task<TReturn>)methodInfo.Invoke(
            handler,
            new object[]
            {
                command,
                ct,
            });
#nullable restore
        return result;
    }

    private static (Type handlerType, MethodInfo methodInfo) GetHandlerTypeFor<TReturn>(
        ICommand<TReturn> command,
        ref ConcurrentDictionary<Type, (Type, MethodInfo)> map)
    {
        var t = command.GetType();

        var handlerType = map.GetOrAdd(
            t,
            static cmdType =>
            {
                var ht = typeof(IHandleCommand<,>).MakeGenericType(cmdType, typeof(TReturn));

                var mi = ht.GetMethod("Handle");

                return (ht, mi!);
            });

        return handlerType;
    }
}
