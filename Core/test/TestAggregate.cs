using System.Dynamic;
using System.Runtime.CompilerServices;
using AutoFixture;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests
{
    public partial class TestAggregate : Aggregate<TestAggregate.State>
    {
        public static TestAggregate Create(Guid id, string name)
        {
            var @event = new TestAggregateCreated(id, name);

            var toReturn = new TestAggregate();

            toReturn.RaiseEvent(@event);

            return toReturn;
        }

        public void Raise(Event @event)
        {
            this.RaiseEvent(@event);
        }
    }
}
