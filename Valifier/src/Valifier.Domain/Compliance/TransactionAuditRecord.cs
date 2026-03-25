namespace Valifier.Domain.Compliance;

public sealed class TransactionAuditRecord
{
    public TransactionAuditRecord(
        Guid id,
        Guid transactionIdentifier,
        DateTimeOffset utcCommitTimestamp,
        string operationType,
        string recordType,
        string recordIdentifier,
        string actorIdentifier,
        string tenantIdentifier)
    {
        if (string.IsNullOrWhiteSpace(operationType))
        {
            throw new ArgumentException("Operation type is required.", nameof(operationType));
        }

        if (string.IsNullOrWhiteSpace(recordType))
        {
            throw new ArgumentException("Record type is required.", nameof(recordType));
        }

        if (string.IsNullOrWhiteSpace(recordIdentifier))
        {
            throw new ArgumentException("Record identifier is required.", nameof(recordIdentifier));
        }

        if (string.IsNullOrWhiteSpace(actorIdentifier))
        {
            throw new ArgumentException("Actor identifier is required.", nameof(actorIdentifier));
        }

        if (string.IsNullOrWhiteSpace(tenantIdentifier))
        {
            throw new ArgumentException("Tenant identifier is required.", nameof(tenantIdentifier));
        }

        Id = id;
        TransactionIdentifier = transactionIdentifier;
        UtcCommitTimestamp = utcCommitTimestamp;
        OperationType = operationType.Trim();
        RecordType = recordType.Trim();
        RecordIdentifier = recordIdentifier.Trim();
        ActorIdentifier = actorIdentifier.Trim();
        TenantIdentifier = tenantIdentifier.Trim();
    }

    private TransactionAuditRecord()
    {
        Id = Guid.NewGuid();
        OperationType = string.Empty;
        RecordType = string.Empty;
        RecordIdentifier = string.Empty;
        ActorIdentifier = string.Empty;
        TenantIdentifier = string.Empty;
    }

    public Guid Id { get; private set; }

    public Guid TransactionIdentifier { get; private set; }

    public DateTimeOffset UtcCommitTimestamp { get; private set; }

    public string OperationType { get; private set; }

    public string RecordType { get; private set; }

    public string RecordIdentifier { get; private set; }

    public string ActorIdentifier { get; private set; }

    public string TenantIdentifier { get; private set; }
}
