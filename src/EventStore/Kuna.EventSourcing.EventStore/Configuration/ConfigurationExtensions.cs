using System.Reflection;
using EventStore.Client;
using Kuna.EventSourcing.EventStore.Subscriptions;
using Kuna.Utilities.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kuna.EventSourcing.EventStore.Configuration;

public static class ConfigurationExtensions
{
    public static void AddEventStore(
        this IServiceCollection services,
        IConfiguration configuration,
        string eventStoreConnectionStringName,
        Assembly[] assembliesWithAggregateEvents,
        Func<Assembly[], Type[]> aggregateEventsDiscoverFunc,
        StreamSubscriptionSettings[]? subscriptionSettings = null)
    {
        services.AddSingleton<IEventTypeMapper>(sp => new EventTypeMapper(assembliesWithAggregateEvents, aggregateEventsDiscoverFunc))
                .AddSingleton<IEventStoreSerializer, JsonEventStoreSerializer>()
                .AddSingleton<IEventMetadataFactory, EventMetadataFactory>()
                .AddSingleton<IEventDataFactory, EventDataFactory>()
                .AddSingleton<IStreamWriter, AggregateStreamWriter>()
                .AddSingleton<IStreamReader, AggregateStreamReader>();

        services.AddSingleton<EventStoreClient>(
            sp =>
            {
                var settings = EventStoreClientSettings
                    .Create(configuration.GetConnectionString(eventStoreConnectionStringName)!);

                settings.ConnectionName = "test-" + Guid.NewGuid();

                return new EventStoreClient(settings);
            });

        if (subscriptionSettings != null)
        {
            services.AddEventStoreSubscriptions(
                configuration,
                eventStoreConnectionStringName,
                subscriptionSettings);
        }
    }

    /// <summary>
    /// Make sure to register each IHandleEvent<TEvent> manually, or use https://github.com/khellang/Scrutor to auto-wire it up
    /// Call only once
    /// </summary>
    private static void AddEventStoreSubscriptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string eventStoreConnectionStringName,
        StreamSubscriptionSettings[] settings)
    {
        services.AddTransient<IEventStreamListener, EventStreamListener>();
        services.AddTransient<IEventDispatcher, EventDispatcher>();

        services.AddSingleton(
            sp =>
            {
                var esSettings = EventStoreClientSettings
                    .Create(configuration.GetConnectionString(eventStoreConnectionStringName)!);

                esSettings.ConnectionName = "persistentSubscriptions-" + Assembly.GetEntryAssembly()!.GetName()!.Name!;

                return new EventStorePersistentSubscriptionsClient(esSettings);
            });

        services.AddHostedService(
            serviceProvider =>
            {
                const string backgroundWorkerName = "PersistentSubscriptions";

                var exists = serviceProvider.GetServices<BackgroundWorker>()
                                            .Any(x => x.Name == backgroundWorkerName);

                if (exists)
                {
                    throw new InvalidOperationException("Stream subscriptions have been already added. Cannot call this extension multiple times");
                }

                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                return new BackgroundWorker(
                    backgroundWorkerName,
                    logger,
                    stoppingToken =>
                    {
                        foreach (var s in settings)
                        {
                            var listener = serviceProvider.GetRequiredService<IEventStreamListener>();
                            listener.Start(s, stoppingToken);
                        }

                        while (!stoppingToken.IsCancellationRequested)
                        {
                            // keep background worker alive
                        }

                        return Task.CompletedTask;
                    });
            });
    }
}
