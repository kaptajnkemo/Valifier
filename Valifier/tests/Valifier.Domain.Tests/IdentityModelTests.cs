using System.Reflection;

namespace Valifier.Domain.Tests;

public sealed class IdentityModelTests
{
    [Fact]
    public void Domain_contains_domain_native_identity_types()
    {
        var assembly = Assembly.Load("Valifier.Domain");

        Assert.NotNull(assembly.GetType("Valifier.Domain.Identity.User"));
        Assert.NotNull(assembly.GetType("Valifier.Domain.Identity.Role"));
        Assert.NotNull(assembly.GetType("Valifier.Domain.Identity.UserId"));
        Assert.NotNull(assembly.GetType("Valifier.Domain.Identity.RoleId"));
    }

    [Fact]
    public void UserId_exposes_a_guid_value()
    {
        var assembly = Assembly.Load("Valifier.Domain");
        var userIdType = assembly.GetType("Valifier.Domain.Identity.UserId");

        Assert.NotNull(userIdType);

        var valueProperty = userIdType!.GetProperty("Value");

        Assert.NotNull(valueProperty);
        Assert.Equal(typeof(Guid), valueProperty!.PropertyType);
    }
}
