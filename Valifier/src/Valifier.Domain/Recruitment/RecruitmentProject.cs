namespace Valifier.Domain.Recruitment;

public sealed class RecruitmentProject
{
    public RecruitmentProject(
        RecruitmentProjectId id,
        string title,
        string department,
        RecruitmentProjectStatus status,
        DateOnly createdOn)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Project title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(department))
        {
            throw new ArgumentException("Department is required.", nameof(department));
        }

        Id = id;
        Title = title.Trim();
        Department = department.Trim();
        Status = status;
        CreatedOn = createdOn;
    }

    private RecruitmentProject()
    {
        Id = RecruitmentProjectId.New();
        Title = string.Empty;
        Department = string.Empty;
    }

    public RecruitmentProjectId Id { get; private set; }

    public string Title { get; private set; }

    public string Department { get; private set; }

    public RecruitmentProjectStatus Status { get; private set; }

    public DateOnly CreatedOn { get; private set; }
}
