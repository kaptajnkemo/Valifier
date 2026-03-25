using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Recruitment;
using Valifier.Domain.Tenancy;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class RecruitmentProjectConfiguration : IEntityTypeConfiguration<RecruitmentProject>
{
    public void Configure(EntityTypeBuilder<RecruitmentProject> builder)
    {
        builder.ToTable("RecruitmentProjects");
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasConversion(identifier => identifier.Value, value => new RecruitmentProjectId(value));

        builder.Property(project => project.TenantId)
            .HasConversion(
                identifier => identifier == null ? (Guid?)null : identifier.Value.Value,
                value => value.HasValue ? new TenantId(value.Value) : null);

        builder.Property(project => project.OwnerUserId)
            .HasConversion(identifier => identifier.Value, value => new UserId(value));

        builder.Property(project => project.SourceOfTruthId)
            .HasConversion(identifier => identifier.Value, value => new TenantSourceOfTruthId(value));

        builder.Property(project => project.Title)
            .HasMaxLength(200);

        builder.Property(project => project.Department)
            .HasMaxLength(120);

        builder.Property(project => project.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(project => project.TenantId);
        builder.HasIndex(project => project.OwnerUserId);
        builder.HasIndex(project => project.SourceOfTruthId);
    }
}
