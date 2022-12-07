namespace Senf.EventSourcing.Core;

public interface IHandleMessage
{
    Task Handle(IMessage message, CancellationToken ct);
}
