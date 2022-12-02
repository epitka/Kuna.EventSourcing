namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public class TestAggregateRepository : AggregateRepository<TestAggregate>
{
    public TestAggregateRepository(IAggregateStreamReader streamReader, IAggregateStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    protected override string StreamPrefix => "test";
}
