namespace Valifier.Domain.Recruitment;

public readonly record struct RecruitmentProjectId(Guid Value)
{
    public static RecruitmentProjectId New() => new(Guid.NewGuid());
}
