using Senf.EventSourcing.Core.Aggregates;

namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public class TestAggregateRepository : AggregateRepository<Guid, TestAggregate>
{
    public TestAggregateRepository(
        IAggregateStreamReader streamReader,
        IAggregateStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "test-";
}
