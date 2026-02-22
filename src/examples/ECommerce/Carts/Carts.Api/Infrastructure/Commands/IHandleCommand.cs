namespace Carts.Infrastructure.Commands;

/// <summary>
/// Marker interface used to identify handlers of commands
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleCommand<in TCommand>
    where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken ct);
}

public interface IHandleCommand<in TCommand, TReturn>
    where TCommand : ICommand<TReturn>
{
    Task<TReturn> Handle(TCommand command, CancellationToken ct);
}
