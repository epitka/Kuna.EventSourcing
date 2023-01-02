namespace Kuna.EventSourcing.Core.Tests.AggregateStateTests;

public class SetId
{
    [Fact]
    public void When_Id_Is_Already_Set_SetId_Should_Throw_If_Id_Is_Different()
    {
        var state = new TestAggregate.State();

        var aggregateId = Id.Create();

        // normally this is not set directly but through application of event
        state.SetId(aggregateId);

        Assert.Throws<InvalidOperationException>(() => state.SetId(Id.Create()));
    }

    [Fact]
    public void Should_Be_Idempotent()
    {
        var state = new TestAggregate.State();

        var aggregateId = Id.Create();

        // normally this is not set directly but through application of event
        state.SetId(aggregateId);
        state.SetId(aggregateId);

        state.Id.Value.Should().Be(aggregateId);
    }
}
