using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Valifier.Infrastructure.Persistence;

public sealed class DesignTimeValifierDbContextFactory : IDesignTimeDbContextFactory<ValifierDbContext>
{
    public ValifierDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ValifierDbContext>();
        builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Valifier;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ValifierDbContext(builder.Options);
    }
}
