using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Knowledge;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class TenantSourceOfTruthEntryConfiguration : IEntityTypeConfiguration<TenantSourceOfTruthEntry>
{
    public void Configure(EntityTypeBuilder<TenantSourceOfTruthEntry> builder)
    {
        builder.ToTable("TenantSourceOfTruthEntries");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasConversion(identifier => identifier.Value, value => new TenantSourceOfTruthEntryId(value));

        builder.Property(entry => entry.SourceOfTruthId)
            .HasConversion(identifier => identifier.Value, value => new TenantSourceOfTruthId(value));

        builder.Property(entry => entry.Key)
            .HasMaxLength(100);

        builder.Property(entry => entry.Label)
            .HasMaxLength(120);

        builder.Property(entry => entry.ValueType)
            .HasMaxLength(40);

        builder.Property(entry => entry.Value)
            .HasMaxLength(500);

        builder.HasIndex(entry => entry.SourceOfTruthId);
    }
}
