namespace Kuna.EventSourcing.Core.Exceptions;

/// <summary>
/// Thrown when aggregate version in the data store does not match the expected version, when trying to persist
/// </summary>
/// <param name="streamId"></param>
/// <param name="message"></param>
/// <param name="ex"></param>
public class AggregateInvalidExpectedVersionException(string streamId, string? message=null, Exception? ex = null) : Exception (message,ex)
{
    public string StreamId { get; } = streamId;
}