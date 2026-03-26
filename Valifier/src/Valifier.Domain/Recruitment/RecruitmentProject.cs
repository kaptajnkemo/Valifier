using Valifier.Domain.Identity;
using Valifier.Domain.Knowledge;
using Valifier.Domain.Tenancy;

namespace Valifier.Domain.Recruitment;

public sealed class RecruitmentProject
{
    public RecruitmentProject(
        RecruitmentProjectId id,
        TenantId? tenantId,
        string title,
        string department,
        RecruitmentProjectStatus status,
        DateOnly createdOn)
        : this(
            id,
            tenantId,
            new UserId(Guid.Empty),
            new TenantSourceOfTruthId(Guid.Empty),
            title,
            department,
            status,
            createdOn)
    {
    }

    public RecruitmentProject(
        RecruitmentProjectId id,
        TenantId? tenantId,
        UserId ownerUserId,
        TenantSourceOfTruthId sourceOfTruthId,
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
        TenantId = tenantId;
        OwnerUserId = ownerUserId;
        SourceOfTruthId = sourceOfTruthId;
        Title = title.Trim();
        Department = department.Trim();
        Status = status;
        CreatedOn = createdOn;
    }

    private RecruitmentProject()
    {
        Id = RecruitmentProjectId.New();
        OwnerUserId = new UserId(Guid.Empty);
        SourceOfTruthId = new TenantSourceOfTruthId(Guid.Empty);
        Title = string.Empty;
        Department = string.Empty;
    }

    public RecruitmentProjectId Id { get; private set; }

    public TenantId? TenantId { get; private set; }

    public UserId OwnerUserId { get; private set; }

    public TenantSourceOfTruthId SourceOfTruthId { get; private set; }

    public string Title { get; private set; }

    public string Department { get; private set; }

    public RecruitmentProjectStatus Status { get; private set; }

    public DateOnly CreatedOn { get; private set; }
}
