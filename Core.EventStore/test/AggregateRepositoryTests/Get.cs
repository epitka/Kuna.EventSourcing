using DeepEqual.Syntax;
using FakeItEasy;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.EventStore.Tests.AggregateRepositoryTests;

public class Get
{
    [Fact]
    public void When_Aggregate_Does_Not_Exist_Should_Throw()
    {
        var fakeReader = A.Fake<IAggregateStreamReader>(opt => opt.Strict());

        A.CallTo(() => fakeReader.GetEvents(A<string>._, default))
         .ReturnsLazily(Enumerable.Empty<IEvent>);

        var repository = new TestAggregateRepository(fakeReader, A.Fake<IAggregateStreamWriter>());

        Assert.ThrowsAsync<AggregateNotFoundException>(async () => await repository.Get(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task When_Aggregate_Has_Events_Should_Return_Instance_Of_Aggregate()
    {
        var fakeReader = A.Fake<IAggregateStreamReader>(opt => opt.Strict());
        var aggregateId = Guid.NewGuid();

        var events = new List<IEvent>()
        {
            new TestAggregate.TestAggregateCreated(aggregateId, "Initial"),
            new TestAggregate.TestAggregateNameChanged(aggregateId, "Changed"),
        };

        A.CallTo(() => fakeReader.GetEvents(A<string>._, default))
         .ReturnsLazily(() => events);

        var repository = new TestAggregateRepository(fakeReader, A.Fake<IAggregateStreamWriter>());

        var instance = await repository.Get(aggregateId, default);

        var instanceState = instance.GetState();

        var expectedState = new TestAggregate.State()
        {
            Name = "Changed",
        };

        expectedState.SetId(aggregateId);
        expectedState.Version = 1;
        expectedState.OriginalVersion = 1;

        instanceState.ShouldDeepEqual(expectedState);
    }
}