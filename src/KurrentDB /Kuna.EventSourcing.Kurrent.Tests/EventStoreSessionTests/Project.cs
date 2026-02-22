using DeepEqual.Syntax;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;

namespace Kuna.EventSourcing.Kurrent.Tests.EventStoreSessionTests;

public class Project
{
    public class TestState
    {
        public TestState()
        {
            this.NameChanges = [];
            this.StateId = string.Empty;
        }

        public string StateId { get; set; }

        public List<string> NameChanges { get; set; }

        public void Apply(TestAggregate.TestAggregateCreated @event)
        {
            this.StateId = @event.Id.ToString();
            this.NameChanges.Add(@event.Name);
        }

        public void Apply(TestAggregate.TestAggregateNameChanged @event)
        {
            this.NameChanges.Add(@event.Name);
        }
    }

    [Fact]
    public async Task Should_Project_State()
    {
        var fakeReader = A.Fake<IStreamReader>(opt => opt.Strict());
        var fakeWriter = A.Fake<IStreamWriter>(opt => opt.Strict());

        var aggregateId = Guid.NewGuid();

        var events = new List<object>()
        {
            new TestAggregate.TestAggregateCreated(aggregateId, "Initial"),
            new TestAggregate.TestAggregateNameChanged(aggregateId, "Changed"),
        };

        var expectedState = new TestState()
        {
            StateId = aggregateId.ToString(),
            NameChanges = new List<string>() { "Initial", "Changed" },
        };


        A.CallTo(() => fakeReader.GetEvents(A<string>._, CancellationToken.None))
         .ReturnsLazily(() => events);

        var session = new EventStoreSession(fakeReader, fakeWriter);

       var projection = await session.Project<TestState>(aggregateId.ToString(), CancellationToken.None);

        projection.ShouldDeepEqual(expectedState);
    }
}
