using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Exceptions;

namespace Kuna.EventSourcing.Core.TestKit;

public class InMemoryAggregateRepository<TAggregate> : IAggregateRepository<Guid, TAggregate>
    where TAggregate : class, IAggregate<Guid>, new()
{
    private readonly Dictionary<Guid, List<EventInfo>> eventsStream = new();

    public Task<TAggregate?> Get(Guid id, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty, id);

        var aggregate = new TAggregate();

        this.eventsStream.TryGetValue(id, out var value);

        if (value == null)
        {
            return Task.FromResult<TAggregate?>(null);
        }

        var instanceStream = value;

        var events = instanceStream
                     .Select(eventInfo => JsonConvert.DeserializeObject(eventInfo.Data, eventInfo.Type)!)
                     .ToList();

        aggregate.InitWith(events);

        return Task.FromResult(aggregate)!;
    }

    public async Task Save(TAggregate aggregate, CancellationToken ct)
    {
        var events = aggregate.DequeuePendingEvents()
                              .Select(
                                  (@event) => new EventInfo(
                                      type: @event.GetType(),
                                      data: JsonConvert.SerializeObject(@event)))
                              .ToArray();

        await this.InternalSave(aggregate, events)
                  .ConfigureAwait(false);
    }

    private Task InternalSave(
        IAggregate<Guid> aggregate,
        EventInfo[] pendingEvents)
    {
        if (!this.eventsStream.TryGetValue(aggregate.Id.Value, out var instanceStream))
        {
            instanceStream = new List<EventInfo>(pendingEvents.Length);
            this.eventsStream.Add(aggregate.Id.Value, instanceStream);
        }

        instanceStream.AddRange(pendingEvents);

        return Task.CompletedTask;
    }

    private class EventInfo
    {
        public EventInfo(Type type, string data)
        {
            this.Type = type;
            this.Data = data;
        }

        public Type Type { get; }

        public string Data { get; }
    }
}
