using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Recruitment;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class RecruitmentProjectConfiguration : IEntityTypeConfiguration<RecruitmentProject>
{
    public void Configure(EntityTypeBuilder<RecruitmentProject> builder)
    {
        builder.ToTable("RecruitmentProjects");
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasConversion(identifier => identifier.Value, value => new RecruitmentProjectId(value));

        builder.Property(project => project.Title)
            .HasMaxLength(200);

        builder.Property(project => project.Department)
            .HasMaxLength(120);

        builder.Property(project => project.Status)
            .HasConversion<string>()
            .HasMaxLength(32);
    }
}
