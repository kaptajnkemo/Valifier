namespace Valifier.Domain.Catalog;

public static class SystemModuleCatalog
{
    public static IReadOnlyList<SystemModule> All { get; } =
    [
        new("Candidate Designer", "Define the ideal candidate profile with structured AI guidance.", "Design", "/workspace"),
        new("Interactive Job Post", "Expose an anonymous exploratory entry point for candidate self-selection.", "Attract", "/candidate-experience"),
        new("Interview Engine", "Generate structured interview transcripts and AI notes.", "Interview", "/workspace"),
        new("Psychometric Evaluation", "Collect structured behavioral and personality indicators.", "Assess", "/workspace"),
        new("Scoring Engine", "Produce deterministic, re-runnable fit scores.", "Evaluate", "/workspace"),
        new("Candidate Flow Manager", "Coordinate state changes, invitations, and feedback.", "Operate", "/workspace"),
        new("Leaderboard", "Present ranked candidates with sortable drill-down.", "Decide", "/workspace"),
        new("AI Decision Support", "Enable natural-language comparisons across the candidate pool.", "Decide", "/workspace")
    ];
}
