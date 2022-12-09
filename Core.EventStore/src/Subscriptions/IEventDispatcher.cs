using System.Collections.Concurrent;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Senf.EventSourcing.Core.EventStore.Subscriptions;

public interface IEventDispatcher
{
    Task Publish<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : class;
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : class
    {
        var handlers = this.serviceProvider.GetServices<IHandleEvent<TEvent>>();

        if (handlers!.Count() == 1)
        {
            await handlers.First()
                          .Handle(@event, ct)
                          .ConfigureAwait(false);
        }
        else
        {
            var aggregateTask = Task.WhenAll(handlers.Select(handler => handler.Handle(@event, ct)));
            aggregateTask.ConfigureAwait(false);

            try
            {
                await aggregateTask;
            }
            catch (Exception e)
            {
                if (aggregateTask.Exception != null)
                {
                    throw aggregateTask.Exception;
                }
            }
        }
    }
}
