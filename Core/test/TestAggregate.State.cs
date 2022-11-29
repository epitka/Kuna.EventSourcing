#nullable disable
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests;

public partial class TestAggregate
{
    public class State
        : AggregateState
    {
        public string Name { get; set; }

        public string ChangedValue { get; set; }

        public void Apply(TestAggregateCreated @event)
        {
            this.SetId(@event.Id);
            this.Name = @event.Name;
        }

        public void Apply(TestAggregateChanged @event)
        {
            this.ChangedValue = @event.ChangedValue;
        }
    }
}