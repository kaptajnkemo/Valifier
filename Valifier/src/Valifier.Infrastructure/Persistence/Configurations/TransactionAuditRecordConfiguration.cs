using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Compliance;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class TransactionAuditRecordConfiguration : IEntityTypeConfiguration<TransactionAuditRecord>
{
    public void Configure(EntityTypeBuilder<TransactionAuditRecord> builder)
    {
        builder.ToTable("TransactionAuditRecords");
        builder.HasKey(record => record.Id);

        builder.Property(record => record.OperationType)
            .HasMaxLength(32);

        builder.Property(record => record.RecordType)
            .HasMaxLength(64);

        builder.Property(record => record.RecordIdentifier)
            .HasMaxLength(64);

        builder.Property(record => record.ActorIdentifier)
            .HasMaxLength(128);

        builder.Property(record => record.TenantIdentifier)
            .HasMaxLength(64);
    }
}
