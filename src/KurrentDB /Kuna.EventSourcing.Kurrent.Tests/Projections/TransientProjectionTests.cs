using DeepEqual.Syntax;
using Kuna.EventSourcing.Kurrent.Projections;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;

namespace Kuna.EventSourcing.Kurrent.Tests.Projections
{
    public class TransientProjectionTests
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
        public async Task Should_Mutate_State()
        {
            var fakeReader = A.Fake<IStreamReader>(opt => opt.Strict());
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

            var projection = new TransientProjection<TestState>(fakeReader);

            var result = await projection.GetFor(aggregateId.ToString(), CancellationToken.None);

            result.ShouldDeepEqual(expectedState);

        }

    }
}
