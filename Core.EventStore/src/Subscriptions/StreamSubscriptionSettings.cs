using EventStore.Client;

namespace Kuna.EventSourcing.Core.EventStore.Subscriptions;

// TODO: add retry delay timespan
public record StreamSubscriptionSettings(
    string StreamName,
    StreamPosition StartFrom,
    string ConsumerStrategy = nameof(SystemConsumerStrategies.Pinned),
    int MaxResubscribeAttempts = 10,
    int LiveBufferSize = 500,
    int ReadBatchSize = 20,
    int HistoryBufferSize = 500,
    int CheckPointLowerBound = 10,
    int CheckPointUpperBound = 1000,
    TimeSpan? CheckpointAfter = null,
    TimeSpan? MessageTimeout = null,
    bool ExtraStatistics = false);

