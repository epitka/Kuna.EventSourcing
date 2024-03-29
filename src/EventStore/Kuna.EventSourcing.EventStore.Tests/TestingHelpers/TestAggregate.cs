﻿using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.EventStore.Tests.TestingHelpers;

public class TestAggregate : Aggregate<Guid, TestAggregate.State>
{
    public static TestAggregate Create(Guid id, string name)
    {
        var toReturn = new TestAggregate();

        toReturn.RaiseEvent(new TestAggregateCreated(id, name));

        return toReturn;
    }

    public void ChangeName( string newName)
    {
        this.RaiseEvent(new TestAggregateNameChanged(this.Id.Value, newName));
    }

    #region 'State'

    public class State : AggregateState<Guid>
    {
        public string Name { get; set; } = default!;

        public void Apply(TestAggregateCreated @event)
        {
            this.SetId(@event.Id);
            this.Name = @event.Name;
        }

        public void Apply(TestAggregateNameChanged @event)
        {
            this.Name = @event.Name;
        }
    }

    #endregion

    #region 'Events'

    public record TestAggregateCreated (Guid Id, string Name)
    {
    }

    public record TestAggregateNameChanged (Guid Id, string Name)
    {
    }

    #endregion
}
