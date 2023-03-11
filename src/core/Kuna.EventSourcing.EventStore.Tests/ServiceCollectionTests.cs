using EventStore.Client;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.EventStore.Configuration;
using Kuna.EventSourcing.EventStore.Subscriptions;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Kuna.EventSourcing.EventStore.Tests;

public class ServiceCollectionTests
{
    private readonly ITestOutputHelper console;

    public ServiceCollectionTests(ITestOutputHelper console)
    {
        this.console = console;
    }

    [Fact]
    public void AddEventStoreTest()
    {
        // build container
        var sc = new ServiceCollection();

        sc.AddLogging();

        sc.AddLogging(
            opt =>
            {
                opt.ClearProviders();
                opt.AddConsole();
            });

        sc.AddTransient<ILogger>(
            sp =>
            {
              return  LoggerFactory.Create(
                                 b =>
                                 {
                                     b.ClearProviders();
                                     b.AddConsole();
                                 })
                             .CreateLogger("Test");
            });

        var cfgBuilder = new ConfigurationBuilder();
        cfgBuilder.AddJsonFile("appsettings.json");

        var cfg = cfgBuilder.Build();

        sc.AddEventStore(
            configuration: cfg,
            eventStoreConnectionStringName: "EventStore",
            assembliesWithAggregateEvents: new[] { typeof(TestAggregate).Assembly },
            subscriptionSettings: new[]
            {
                new StreamSubscriptionSettings(
                    "$ce-test",
                    StartFrom: StreamPosition.End),
            });

        var provider = sc.BuildServiceProvider();

        using var scope = provider.CreateScope();

        var toResolve = new[]
        {
            typeof(IEventStreamListener),
            typeof(IAggregateStreamWriter),
            typeof(IAggregateStreamReader),
            typeof(IEventDataFactory),
            typeof(IEventMetadataFactory),
            typeof(IEventStoreSerializer),
            typeof(IEventTypeMapper),
        };

        var failed = false;

        foreach (var type in toResolve)
        {
            try
            {
                var r = scope.ServiceProvider.GetRequiredService(type);
            }
            catch (Exception e)
            {
                this.console.WriteLine(e.Message);
                failed = true;
            }
        }

        failed.Should().Be(false, "Resolving types failed");

    }
}
