namespace Kuna.Utilities.Commands;

/// <summary>
/// Marker interface used to identify handlers of commands
/// </summary>
/// <typeparam name="TCommand"></typeparam>
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
