using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senf.EventSourcing.Core.EventStore.Subscriptions;

namespace Senf.EventSourcing.Core.EventStore.Configuration;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Make sure to register each IHandleEvent<TEvent> manually, or use https://github.com/khellang/Scrutor to auto-wire it up
    /// Call only once
    /// </summary>
    public static void AddEventStoreSubscriptions(
        this IServiceCollection services,
        IConfiguration configuration,
        params StreamSubscriptionSettings[] settings)
    {
        services.AddTransient<IEventStreamListener, EventStreamListener>();
        services.AddTransient<IEventDispatcher, EventDispatcher>();

        services.AddSingleton(
            sp =>
            {
                var esSettings = EventStoreClientSettings
                    .Create(configuration.GetConnectionString("EventStore"));

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
                    async stoppingToken =>
                    {
                        await Task.WhenAll(
                            settings.Select(
                                s =>
                                {
                                    var listener = serviceProvider.GetRequiredService<IEventStreamListener>();
                                    return listener.Start(s, stoppingToken);
                                }));

                        // keep background worker alive
                        while (!stoppingToken.IsCancellationRequested)
                        {
                        }
                    });
            });
    }
}
