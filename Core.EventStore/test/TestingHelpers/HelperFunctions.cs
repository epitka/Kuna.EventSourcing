using EventStore.Client;

namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public static class HelperFunctions
{
    public static TestAggregateEvent[] GetEvents(Guid aggregateId, int count)
    {
        var toReturn = new TestAggregateEvent[count];
        for (var i = 0; i < count; i++)
        {
            toReturn[i] = new TestAggregateEvent(aggregateId, i.ToString());
        }
        return toReturn;
    }

    public static string GetStreamId(string streamPrefix, Guid aggregateId) => string.Concat(streamPrefix, aggregateId.ToString());

    public static async  Task<int> GetExpectedVersion(EventStoreClient client, string streamId)
    {
        var existing = client.ReadStreamAsync(Direction.Backwards, streamId, StreamPosition.End, 1);
        var enumerator = existing.GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        var lastEvent = enumerator.Current;

        return Convert.ToInt32(lastEvent.Event.EventNumber);
    }
}
