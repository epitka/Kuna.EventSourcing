using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Subscriptions;

public interface IEventDispatcher
{
    Task Publish(IAggregateEvent aggregateEvent, CancellationToken ct);
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider serviceProvider;

    private static readonly ConcurrentDictionary<Type, MethodInfo> PublishMethods = new();

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Publish(IAggregateEvent aggregateEvent, CancellationToken ct)
    {
        // TODO: EventTypeMapper already looks up eventType, need to reduce overhead of looking up event type
        var eventType = aggregateEvent.GetType();

        var methodInfo = GetGenericPublishFor(eventType);

       await (Task)methodInfo.Invoke(this, new object[] { aggregateEvent, ct })!;
    }

    private async Task InternalPublish<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : class
    {
        var handlers = this.serviceProvider.GetServices<IHandleEvent<TEvent>>();

        if (handlers.Any() == false)
        {
            return;
        }

        if (handlers!.Count() == 1)
        {
            await handlers.First()
                          .Handle(@event, ct)
                          .ConfigureAwait(false);
        }
        else
        {
            var aggregateTask = Task.WhenAll(handlers.Select(handler => handler.Handle(@event, ct)));

            try
            {
                await aggregateTask.ConfigureAwait(false);
            }
            catch (Exception)
            {
                if (aggregateTask.Exception != null)
                {
                    throw aggregateTask.Exception;
                }
            }
        }
    }

    private static MethodInfo GetGenericPublishFor(Type eventType) =>
        PublishMethods.GetOrAdd(
            eventType,
            evtType =>
                typeof(EventDispatcher)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Single(m => m.Name == nameof(InternalPublish) && m.GetGenericArguments().Any())
                    .MakeGenericMethod(evtType)
        );
}
