using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Valifier.Domain.Compliance;

namespace Valifier.Infrastructure.Persistence;

public sealed class TransactionAuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentAuditContextAccessor _currentAuditContextAccessor;

    public TransactionAuditSaveChangesInterceptor(ICurrentAuditContextAccessor currentAuditContextAccessor)
    {
        _currentAuditContextAccessor = currentAuditContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditRecords(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditRecords(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void ApplyAuditRecords(DbContext? context)
    {
        if (context is not ValifierDbContext dbContext)
        {
            return;
        }

        var attemptedAuditUpdates = dbContext.ChangeTracker
            .Entries<TransactionAuditRecord>()
            .Where(entry => entry.State is EntityState.Modified or EntityState.Deleted)
            .ToArray();

        if (attemptedAuditUpdates.Length > 0)
        {
            throw new InvalidOperationException("Transaction audit records are immutable.");
        }

        var governedEntries = dbContext.ChangeTracker
            .Entries()
            .Where(entry =>
                entry.Entity is IGovernedRecord &&
                entry.Entity is not TransactionAuditRecord &&
                entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        if (governedEntries.Length == 0)
        {
            return;
        }

        var transactionIdentifier = Guid.NewGuid();
        var commitTimestamp = DateTimeOffset.UtcNow;
        var actorIdentifier = _currentAuditContextAccessor.GetActorIdentifier();
        var tenantIdentifier = _currentAuditContextAccessor.GetTenantIdentifier();

        var auditRecords = governedEntries
            .Select(entry =>
            {
                var governedRecord = (IGovernedRecord)entry.Entity;
                return new TransactionAuditRecord(
                    Guid.NewGuid(),
                    transactionIdentifier,
                    commitTimestamp,
                    MapOperationType(entry.State),
                    governedRecord.GetRecordType(),
                    governedRecord.GetRecordIdentifier(),
                    actorIdentifier,
                    tenantIdentifier);
            })
            .ToArray();

        dbContext.TransactionAuditRecords.AddRange(auditRecords);
    }

    private static string MapOperationType(EntityState state)
    {
        return state switch
        {
            EntityState.Added => "Create",
            EntityState.Modified => "Update",
            EntityState.Deleted => "Delete",
            _ => throw new InvalidOperationException($"Unsupported state '{state}'.")
        };
    }
}
