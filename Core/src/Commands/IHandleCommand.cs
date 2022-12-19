namespace Senf.EventSourcing.Core.Commands;

/// <summary>
/// Marker interface used to identify handlers of events
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleCommand<in TCommand>
    where TCommand : class, ICommand
{
    Task Handle(TCommand command, CancellationToken ct);
}
