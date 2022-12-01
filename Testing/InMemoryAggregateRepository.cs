﻿using Newtonsoft.Json;
using Senf.EventSourcing.Core;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Testing;

public class InMemoryAggregateRepository<TAggregate> : IAggregateRepository<TAggregate>
    where TAggregate : class, IAggregate, new()
{
    private readonly Dictionary<Guid, List<EventInfo>> eventsStream;

    public InMemoryAggregateRepository()
    {
        this.eventsStream = new Dictionary<Guid, List<EventInfo>>();
    }

    public Task<TAggregate> Get(Guid id, CancellationToken ct)
    {
        var aggregate = new TAggregate();

        if (this.eventsStream.ContainsKey(id) == false)
        {
            throw new AggregateNotFoundException(id, typeof(TAggregate));
        }

        var instanceStream = this.eventsStream[id];

        var events = instanceStream
                     .Select(eventInfo => (IEvent)JsonConvert.DeserializeObject(eventInfo.Data, eventInfo.Type)!)
                     .ToList();

        aggregate.InitWith(events);

        return Task.FromResult(aggregate);
    }

    public async Task Save(TAggregate aggregate, CancellationToken ct)
    {
        var events = aggregate.DequeuePendingEvents()
                             .Select(
                                      (@event) => new EventInfo(
                                          type: @event.GetType(),
                                          data: JsonConvert.SerializeObject(@event)))
                             .ToArray();

        await this.InternalSave(aggregate, events);
    }

    private Task InternalSave(
        IAggregate aggregate,
        EventInfo[] pendingEvents)
    {
        if (this.eventsStream.TryGetValue(aggregate.Id, out var instanceStream) == false)
        {
            instanceStream = new List<EventInfo>(pendingEvents.Length);
            this.eventsStream.Add(aggregate.Id, instanceStream);
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