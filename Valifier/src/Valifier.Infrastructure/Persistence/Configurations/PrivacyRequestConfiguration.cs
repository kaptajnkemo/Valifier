using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valifier.Domain.Compliance;

namespace Valifier.Infrastructure.Persistence.Configurations;

public sealed class PrivacyRequestConfiguration : IEntityTypeConfiguration<PrivacyRequest>
{
    public void Configure(EntityTypeBuilder<PrivacyRequest> builder)
    {
        builder.ToTable("PrivacyRequests");
        builder.HasKey(request => request.Id);

        builder.Property(request => request.SubjectIdentifier)
            .HasMaxLength(200);

        builder.Property(request => request.RequestType)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(32);
    }
}
