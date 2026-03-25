using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Valifier.Domain.Compliance;
using Valifier.Domain.Recruitment;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;

namespace Valifier.Infrastructure.Persistence;

public sealed class ValifierDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ValifierDbContext(DbContextOptions<ValifierDbContext> options)
        : base(options)
    {
    }

    public DbSet<RecruitmentProject> RecruitmentProjects => Set<RecruitmentProject>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<PrivacyRequest> PrivacyRequests => Set<PrivacyRequest>();

    public DbSet<TransactionAuditRecord> TransactionAuditRecords => Set<TransactionAuditRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ValifierDbContext).Assembly);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
    }
}
