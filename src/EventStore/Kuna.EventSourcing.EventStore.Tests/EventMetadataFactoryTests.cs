using System.Diagnostics;

namespace Kuna.EventSourcing.EventStore.Tests;

public class EventMetadataFactoryTests
{
    [Fact]
    public void When_Activity_Exists_Should_Set_Correlation_Id()
    {
        var activity = new Activity("test");

        activity.Start();

        var factory = new EventMetadataFactory();

        var result = factory.Get();

        result["CorrelationId"].Should().Be(activity.Id);

        activity.Stop();
    }

    [Fact]
    public void When_No_Activity_Exists_Should_Not_Have_Entry_For_CorrelationId()
    {
        IEventMetadataFactory factory = new EventMetadataFactory();

        var result = factory.Get();

        result.ContainsKey("Correlationid").Should().BeFalse();
    }
}
