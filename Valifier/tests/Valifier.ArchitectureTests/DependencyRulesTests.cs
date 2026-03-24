using System.Xml.Linq;

namespace Valifier.ArchitectureTests;

public sealed class DependencyRulesTests
{
    [Fact]
    public void Production_project_references_match_the_required_graph()
    {
        var solutionRoot = GetSolutionRoot();

        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Domain", "Valifier.Domain.csproj"));
        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Application", "Valifier.Application.csproj"),
            "..\\Valifier.Domain\\Valifier.Domain.csproj");
        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Infrastructure", "Valifier.Infrastructure.csproj"),
            "..\\Valifier.Application\\Valifier.Application.csproj",
            "..\\Valifier.Domain\\Valifier.Domain.csproj");
        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Contracts", "Valifier.Contracts.csproj"));
        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Web.Client", "Valifier.Web.Client.csproj"),
            "..\\Valifier.Contracts\\Valifier.Contracts.csproj");
        AssertProjectReferences(
            Path.Combine(solutionRoot, "src", "Valifier.Web", "Valifier.Web.csproj"),
            "..\\Valifier.Application\\Valifier.Application.csproj",
            "..\\Valifier.Contracts\\Valifier.Contracts.csproj",
            "..\\Valifier.Infrastructure\\Valifier.Infrastructure.csproj",
            "..\\Valifier.Web.Client\\Valifier.Web.Client.csproj");
    }

    [Fact]
    public void Web_project_does_not_reference_persistence_or_identity_packages()
    {
        var solutionRoot = GetSolutionRoot();
        var document = XDocument.Load(Path.Combine(solutionRoot, "src", "Valifier.Web", "Valifier.Web.csproj"));
        var packageReferences = document
            .Descendants("PackageReference")
            .Select(node => (string?)node.Attribute("Include"))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        Assert.DoesNotContain("Microsoft.AspNetCore.Identity.EntityFrameworkCore", packageReferences);
        Assert.DoesNotContain("Microsoft.EntityFrameworkCore.SqlServer", packageReferences);
        Assert.DoesNotContain("Microsoft.EntityFrameworkCore.Tools", packageReferences);
    }

    [Fact]
    public void Web_project_does_not_own_the_dbcontext_or_migrations()
    {
        var solutionRoot = GetSolutionRoot();

        Assert.False(File.Exists(Path.Combine(solutionRoot, "src", "Valifier.Web", "Data", "ApplicationDbContext.cs")));
        Assert.False(Directory.Exists(Path.Combine(solutionRoot, "src", "Valifier.Web", "Data", "Migrations")));
    }

    private static void AssertProjectReferences(string projectPath, params string[] expectedReferences)
    {
        var document = XDocument.Load(projectPath);
        var actualReferences = document
            .Descendants("ProjectReference")
            .Select(node => (string?)node.Attribute("Include"))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var expected = expectedReferences
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(expected, actualReferences);
    }

    private static string GetSolutionRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }
}
