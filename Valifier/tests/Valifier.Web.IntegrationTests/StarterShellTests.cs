namespace Valifier.Web.IntegrationTests;

public sealed class StarterShellTests
{
    [Fact]
    public void Home_page_is_branded_for_valifier()
    {
        var solutionRoot = GetSolutionRoot();
        var homePagePath = Path.Combine(solutionRoot, "src", "Valifier.Web", "Components", "Pages", "Home.razor");
        var homePage = File.ReadAllText(homePagePath);

        Assert.Contains("Valifier", homePage, StringComparison.Ordinal);
        Assert.DoesNotContain("Hello, world!", homePage, StringComparison.Ordinal);
    }

    [Fact]
    public void Client_contains_a_candidate_experience_page()
    {
        var solutionRoot = GetSolutionRoot();
        var candidatePagePath = Path.Combine(solutionRoot, "src", "Valifier.Web.Client", "Pages", "CandidateExperience.razor");

        Assert.True(File.Exists(candidatePagePath), $"Expected candidate experience page at '{candidatePagePath}'.");
    }

    private static string GetSolutionRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }
}
