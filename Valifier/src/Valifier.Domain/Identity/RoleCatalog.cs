namespace Valifier.Domain.Identity;

public static class RoleCatalog
{
    public static IReadOnlyList<Role> All { get; } =
    [
        new(new RoleId(Guid.Parse("1f138c43-4a7a-4e71-84ef-0e53a8bfa101")), RoleNames.Admin, "Global platform administration and support."),
        new(new RoleId(Guid.Parse("0c8d4854-7b08-4b7a-b4d7-dfd07d501102")), RoleNames.Superuser, "Tenant-level administration and project governance."),
        new(new RoleId(Guid.Parse("6513d0d3-b2df-4afb-bfd8-9af55f1f6103")), RoleNames.HiringManager, "Project ownership, candidate review, and decision support.")
    ];
}
