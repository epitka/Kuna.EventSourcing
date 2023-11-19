using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Kuna.EventSourcing.Core.Projections;

namespace Kuna.EventSourcing.Core.Tests.Projections
{
    public class StateMutatorTests
    {
        public class TestState
        {
            public TestState()
            {
                this.Changes = [];
                this.StateId = string.Empty;
            }

            public string StateId { get; set; }

            public List<string> Changes { get; set; }

            public void Apply(TestAggregateCreated @event)
            {
                this.StateId = @event.Id.ToString();
                this.Changes.Add(@event.Name);
            }

            public void Apply(TestAggregateChanged @event)
            {
                this.Changes.Add(@event.ChangedValue);
            }
        }

        [Fact]
        public void Can_Apply_Mutations()
        {
            var id = Guid.NewGuid();

            var events = new List<object>()
            {
                new TestAggregateCreated(id, "Initial"),
                new TestAggregateChanged(id, "Changed"),
            };

            var expectedState = new TestState()
            {
               StateId = id.ToString(),
               Changes = ["Initial", "Changed"],
            };

            var state1 = new TestState();
            var version1 = -1;

            StateMutator.Mutate(state1, ref version1, events[0]);
            StateMutator.Mutate(state1, ref version1, events[1]);

            var state2 = new TestState();
            var version2 = -1;

            StateMutator.Mutate(state2, ref version2, events);

           state1.ShouldDeepEqual(expectedState);
           state2.ShouldDeepEqual(expectedState);

        }
    }
}
