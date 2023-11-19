using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.EventStore.Tests.TestingHelpers;

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
