namespace Valifier.Contracts;

public sealed class TransactionAuditContract
{
    public required Guid TransactionIdentifier { get; init; }

    public required DateTimeOffset UtcCommitTimestamp { get; init; }

    public required string OperationType { get; init; }

    public required string RecordType { get; init; }

    public required string RecordIdentifier { get; init; }

    public required string ActorIdentifier { get; init; }

    public required string TenantIdentifier { get; init; }
}
