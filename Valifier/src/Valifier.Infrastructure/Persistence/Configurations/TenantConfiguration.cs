using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Identity;
using Valifier.Domain.Tenancy;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(tenant => tenant.Id);

        builder.Property(tenant => tenant.Id)
            .HasConversion(identifier => identifier.Value, value => new TenantId(value));

        builder.Property(tenant => tenant.Name)
            .HasMaxLength(200);

        builder.Property(tenant => tenant.InitialSuperuserDisplayName)
            .HasMaxLength(200);

        builder.Property(tenant => tenant.InitialSuperuserEmail)
            .HasMaxLength(256);

        builder.Property(tenant => tenant.InitialSuperuserUserId)
            .HasConversion(identifier => identifier.Value, value => new UserId(value));
    }
}
