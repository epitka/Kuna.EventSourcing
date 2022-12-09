using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Senf.EventSourcing.Core.EventStore.Subscriptions;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class EventDispatcherTest
{
    public record Created(Guid Id);

    public record Updated(Guid Id);

    public record Deleted(Guid Id);

    public class CreatedHandler : IHandleEvent<Created>
    {
        public async Task Handle(Created message, CancellationToken ct)
        {
            await Task.Delay(5000, ct);
        }
    }

    public class UpdatedHandler : IHandleEvent<Updated>
    {
        public Task Handle(Updated message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }

    public class DeletedHandler : IHandleEvent<Deleted>
    {
        public Task Handle(Deleted message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }

    public class SagaHandler
        : IHandleEvent<Created>,
          IHandleEvent<Updated>,
          IHandleEvent<Deleted>
    {
        public Task Handle(Created message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task Handle(Updated message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task Handle(Deleted message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Should_Invoke_Correct_Handler()
    {
        var @event = new Created(Guid.NewGuid());

        var fakeCreatedHandler = A.Fake<IHandleEvent<Created>>(opt => opt.Strict());
        var createdCallConfig = A.CallTo(() => fakeCreatedHandler.Handle(@event, A<CancellationToken>._));

        createdCallConfig.Returns(Task.CompletedTask);

        var fakeUpdatedHandler = A.Fake<IHandleEvent<Updated>>(opt => opt.Strict());

        var sc = new ServiceCollection();
        sc.AddScoped<IHandleEvent<Created>>(sp => fakeCreatedHandler);
        sc.AddScoped<IHandleEvent<Updated>>(sp => fakeUpdatedHandler);
        sc.AddScoped<IEventDispatcher, EventDispatcher>();

        var sp = sc.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        await dispatcher.Publish(@event, CancellationToken.None);

        createdCallConfig.MustHaveHappenedOnceExactly();

        A.CallTo(fakeUpdatedHandler)
         .MustNotHaveHappened();
    }

    [Fact]
    public async Task When_Multiple_Handlers_Should_Invoke_Each_One()
    {
        var @event = new Created(Guid.NewGuid());

        var fakeCreatedHandlers = A.CollectionOfFake<IHandleEvent<Created>>(3);

        var createdCallConfigs = fakeCreatedHandlers.Select(fakeCreatedHandler => A.CallTo(() => fakeCreatedHandler.Handle(@event, A<CancellationToken>._)));

        foreach (var callConfig in createdCallConfigs)
        {
            callConfig.Returns(Task.CompletedTask);
        }

        var fakeUpdatedHandler = A.Fake<IHandleEvent<Updated>>(opt => opt.Strict());

        var sc = new ServiceCollection();

        foreach (var fakeCreatedHandler in fakeCreatedHandlers)
        {
            sc.AddScoped<IHandleEvent<Created>>(sp => fakeCreatedHandler);
        }

        sc.AddScoped<IHandleEvent<Updated>>(sp => fakeUpdatedHandler);
        sc.AddScoped<IEventDispatcher, EventDispatcher>();

        var sp = sc.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        await dispatcher.Publish(@event, CancellationToken.None);

        foreach (var createdCallConfig in createdCallConfigs)
        {
            createdCallConfig.MustHaveHappenedOnceExactly();
        }

        A.CallTo(fakeUpdatedHandler)
         .MustNotHaveHappened();
    }

    [Fact]
    public async Task When_Multiple_Handlers_And_Some_Throw_Should_Invoke_Each_One_And_Throw_AggregateException()
    {
        var @event = new Created(Guid.NewGuid());

        var fakeCreatedHandlers = A.CollectionOfFake<IHandleEvent<Created>>(4);

        var createdCallConfigs = fakeCreatedHandlers.Select(fakeCreatedHandler => A.CallTo(() => fakeCreatedHandler.Handle(@event, A<CancellationToken>._)))
                                                    .ToArray();

        // let's make some throw;

        createdCallConfigs[0].Returns(Task.FromException<InvalidOperationException>(new InvalidOperationException("operation")));
        createdCallConfigs[1].Returns(Task.FromException<ArgumentException>(new ArgumentException("argument")));

        for (var i = 2; i < createdCallConfigs.Count(); i++)
        {
            createdCallConfigs[i].Returns(Task.Delay(1000));
        }

        var sc = new ServiceCollection();

        foreach (var fakeCreatedHandler in fakeCreatedHandlers)
        {
            sc.AddScoped<IHandleEvent<Created>>(sp => fakeCreatedHandler);
        }

        sc.AddScoped<IEventDispatcher, EventDispatcher>();

        var sp = sc.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        var result = await Assert.ThrowsAsync<AggregateException>(async () => await dispatcher.Publish(@event, CancellationToken.None));

        result.InnerExceptions.Count().Should().Be(2);
        result.InnerExceptions.First().Should().BeOfType<InvalidOperationException>();
        result.InnerExceptions.Last().Should().BeOfType<ArgumentException>();

        foreach (var t in createdCallConfigs)
        {
            t.MustHaveHappenedOnceExactly();
        }
    }
}
