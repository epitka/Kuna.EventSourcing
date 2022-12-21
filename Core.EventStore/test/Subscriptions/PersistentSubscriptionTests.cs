﻿using EventStore.Client;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.EventStore.Configuration;
using Senf.EventSourcing.Core.EventStore.Subscriptions;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;
using static Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.HelperFunctions;


namespace Senf.EventSourcing.Core.EventStore.Tests;

[Collection("EventStore collection")]
public class PersistentSubscriptionTests
{
    private readonly EventStoreContainerFixture eventStoreDatabaseFixture;

    private IConfigurationBuilder Configuration { get; set; }

    private ServiceCollection Services { get; set; }

    private ServiceProvider ServiceProvider { get; set; } = default!;

    public PersistentSubscriptionTests(EventStoreContainerFixture eventStoreDatabaseFixture)
    {
        this.eventStoreDatabaseFixture = eventStoreDatabaseFixture;

        this.Configuration =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false);

        this.Services = new ServiceCollection();

        this.Services.AddLogging();
        this.Services.AddSingleton<ILogger>(sp => NullLogger.Instance);
    }

    [Fact]
    public async Task Can_Subscribe_To_Stream()
    {
        this.Services.AddEventStore(
            this.Configuration.Build(),
            "EventStore",
            new[] { this.GetType().Assembly },
            new[]
            {
                new StreamSubscriptionSettings(
                    "$ce-test",
                    StreamPosition.Start),
            });

        var numberOfEvents = 100;
        var control = new List<string>(100);
        // with fake handler we will collect all values of the events into a list
        var fakeEventHandler = A.Fake<IHandleEvent<TestAggregateEvent>>(opt => opt.Strict());
        A.CallTo(() => fakeEventHandler.Handle(A<TestAggregateEvent>._, A<CancellationToken>._))
         .Invokes(
             call =>
             {
                 var @event = call.Arguments[0] as TestAggregateEvent;
                 control.Add(@event!.Value);
             })
         .Returns(Task.CompletedTask);

        this.Services.AddTransient<IHandleEvent<TestAggregateEvent>>(sp => fakeEventHandler);

        this.ServiceProvider = this.Services.BuildServiceProvider();

        var serviceProvider = this.ServiceProvider;

        using var scope = serviceProvider.CreateScope();

        var hostedService = serviceProvider.GetRequiredService<IHostedService>();

        var ct = new CancellationToken();

        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();

        var aggregateId = Guid.NewGuid();

        await WriteEvents(writer, "test-", aggregateId, numberOfEvents);

        await Task.Run(
            () => { hostedService.StartAsync(ct).ConfigureAwait(false); },
            ct);

        // let's give handlers some time to process
        await Task.Delay(5_000, ct);

        // since there is only one consumer, this process
        // events should be applied in the order they were added to ES

        var reader = scope.ServiceProvider.GetRequiredService<IAggregateStreamReader>();

        var events = (await reader.GetEvents("test-" + aggregateId, ct))
            .ToArray();

        control.Count.Should().Be(100);

        for (var i = 0; i < events.Length; i++)
        {
            var value = ((TestAggregateEvent)events[i]).Value;

            control[i].Should().Be(value);
        }

        await hostedService.StopAsync(ct);
    }

    private static async Task WriteEvents(
        IAggregateStreamWriter writer,
        string streamPrefix,
        Guid aggregateId,
        int numberOfEvents)
    {
        var streamId = GetStreamId(streamPrefix, aggregateId);
        var events = GetEvents(aggregateId, numberOfEvents);
        await writer.Write(streamId, (-1).ToStreamRevision(), events, default);
    }
}
