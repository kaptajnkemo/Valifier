namespace Valifier.Web.Pages.Shared.Navigation;

public sealed record WorkspaceNavigationModel(
    string? CurrentLocationLabel,
    IReadOnlyList<WorkspaceNavigationLink> PrimaryLinks,
    IReadOnlyList<WorkspaceNavigationLink> ReturnLinks);

public sealed record WorkspaceNavigationLink(string Label, string Href);
