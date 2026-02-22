namespace Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;

public class TestAggregateRepository : AggregateRepository<Guid, TestAggregate>
{
    public TestAggregateRepository(
        IStreamReader streamReader,
        IStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "test-";
}
