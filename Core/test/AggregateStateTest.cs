using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests
{
    public class AggregateStateTest
    {
        [Fact]
        public void Should_Change_Internal_State_and_Version()
        {
            var state = new TestAggregate.State();

            var aggregateId = Id.Create();
            var created = new TestAggregateCreated(Id: aggregateId, Name: "Name1")
            {
                Version = 0,
            };

            var changed = new TestAggregateChanged(Id: aggregateId, ChangedValue: "changed1")
            {
                Version = 1,
            };

            state.ApplyEvent(created);
            state.ApplyEvent(changed);

            state.Id.Should().Be(aggregateId);
            state.Name.Should().Be("Name1");
            state.ChangedValue.Should().Be("changed1");
        }

        [Fact]
        public void Should_Not_Allow_Event_Out_of_Order()
        {
            var aggregateId = Id.Create();

            var state = new TestAggregate.State()
            {
                //// Aggregate = new TestAggregate(),
            };

            var created = new TestAggregateCreated(Id: aggregateId, Name: "Name1")
            {
                Version= 2,
            };

            // Act
            Assert.Throws<InvalidOperationException>(
                () => state.ApplyEvent(created));

            // Assert
            state.Version.Should().Be(-1);
        }

        [Fact]
        public void When_Id_Is_Already_Set_SetId_Should_Throw_If_Id_Is_Different()
        {
            var state = new TestAggregate.State();

            var aggregateId = Id.Create();

            // normally this is not set directly but through application of event
            state.SetId(aggregateId);

            state.SetId(aggregateId); // <--- this should be idempotent for same id

            Assert.Throws<InvalidOperationException>(() => state.SetId(Guid.NewGuid()));
        }
    }
}
