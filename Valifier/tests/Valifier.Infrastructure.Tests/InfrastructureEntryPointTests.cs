using System.Reflection;

namespace Valifier.Infrastructure.Tests;

public sealed class InfrastructureEntryPointTests
{
    [Fact]
    public void Infrastructure_contains_persistence_and_identity_entrypoints()
    {
        var assembly = Assembly.Load("Valifier.Infrastructure");

        Assert.NotNull(assembly.GetType("Valifier.Infrastructure.Persistence.ValifierDbContext"));
        Assert.NotNull(assembly.GetType("Valifier.Infrastructure.Identity.ApplicationUser"));
        Assert.NotNull(assembly.GetType("Valifier.Infrastructure.Identity.ApplicationRole"));
        Assert.NotNull(assembly.GetType("Valifier.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions"));
    }
}
