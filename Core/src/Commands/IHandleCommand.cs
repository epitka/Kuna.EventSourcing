namespace Kuna.EventSourcing.Core.Commands;

/// <summary>
/// Marker interface used to identify handlers of commands
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleCommand<in TCommand>
    where TCommand : class, ICommand
{
    Task Handle(TCommand command, CancellationToken ct);
}
