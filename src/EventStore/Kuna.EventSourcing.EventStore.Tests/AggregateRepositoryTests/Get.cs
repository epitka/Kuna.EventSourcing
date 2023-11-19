using DeepEqual.Syntax;
using Kuna.EventSourcing.Core.Exceptions;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers;

namespace Kuna.EventSourcing.EventStore.Tests.AggregateRepositoryTests;

public class Get
{
    [Fact]
    public void When_Aggregate_Does_Not_Exist_Should_Throw()
    {
        var fakeReader = A.Fake<IStreamReader>(opt => opt.Strict());

        A.CallTo(() => fakeReader.GetEvents(A<string>._, default))
         .ReturnsLazily(Enumerable.Empty<object>);

        var repository = new TestAggregateRepository(fakeReader, A.Fake<IStreamWriter>());

        Assert.ThrowsAsync<AggregateNotFoundException<object>>(async () => await repository.Get(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task When_Aggregate_Has_Events_Should_Return_Instance_Of_Aggregate()
    {
        var fakeReader = A.Fake<IStreamReader>(opt => opt.Strict());
        var aggregateId = Guid.NewGuid();

        var events = new List<object>()
        {
            new TestAggregate.TestAggregateCreated(aggregateId, "Initial"),
            new TestAggregate.TestAggregateNameChanged(aggregateId, "Changed"),
        };

        A.CallTo(() => fakeReader.GetEvents(A<string>._, default))
         .ReturnsLazily(() => events);

        var repository = new TestAggregateRepository(fakeReader, A.Fake<IStreamWriter>());

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