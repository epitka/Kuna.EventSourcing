using Microsoft.Extensions.DependencyInjection;

namespace Kuna.EventSourcing.Core.Events;

public interface IEventDispatcher
{
    Task Publish(object @event, Type eventType, CancellationToken ct);
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider serviceProvider;

    private static readonly ConcurrentDictionary<Type, MethodInfo> PublishMethods = new();

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task Publish(object @event, Type eventType, CancellationToken ct)
    {
        var methodInfo = GetGenericPublishFor(eventType);

        if (methodInfo is not null)
        {
            await (Task)methodInfo.Invoke(this, new[] { @event, ct })!;
        }
    }

    private async Task InternalPublish<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : class
    {
        var handlers = this.serviceProvider.GetServices<IHandleEvent<TEvent>>();

        if (!handlers.Any())
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
