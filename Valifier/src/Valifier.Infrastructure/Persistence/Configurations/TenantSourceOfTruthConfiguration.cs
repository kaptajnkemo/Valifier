using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Tenancy;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class TenantSourceOfTruthConfiguration : IEntityTypeConfiguration<TenantSourceOfTruth>
{
    public void Configure(EntityTypeBuilder<TenantSourceOfTruth> builder)
    {
        builder.ToTable("TenantSourceOfTruths");
        builder.HasKey(sourceOfTruth => sourceOfTruth.Id);

        builder.Property(sourceOfTruth => sourceOfTruth.Id)
            .HasConversion(identifier => identifier.Value, value => new TenantSourceOfTruthId(value));

        builder.Property(sourceOfTruth => sourceOfTruth.TenantId)
            .HasConversion(identifier => identifier.Value, value => new TenantId(value));

        builder.Property(sourceOfTruth => sourceOfTruth.Topic)
            .HasMaxLength(120);

        builder.Property(sourceOfTruth => sourceOfTruth.Name)
            .HasMaxLength(200);

        builder.Property(sourceOfTruth => sourceOfTruth.SchemaVersion)
            .HasMaxLength(64);

        builder.HasIndex(sourceOfTruth => sourceOfTruth.TenantId);

        builder.HasMany(sourceOfTruth => sourceOfTruth.Entries)
            .WithOne()
            .HasForeignKey(entry => entry.SourceOfTruthId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
