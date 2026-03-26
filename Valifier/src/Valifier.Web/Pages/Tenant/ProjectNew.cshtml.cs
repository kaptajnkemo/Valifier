using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Valifier.Application.Features.Tenants.TenantProjects;
using Valifier.Domain.Identity;
using Valifier.Domain.Tenancy;
using Valifier.Infrastructure.Identity;
using Valifier.Infrastructure.Persistence;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.TenantWorkspaceRoles)]
public sealed class ProjectNewModel : PageModel
{
    private readonly CreateTenantProjectCommandHandler _handler;
    private readonly ValifierDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectNewModel(
        CreateTenantProjectCommandHandler handler,
        ValifierDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<SelectOption> OwnerOptions { get; private set; } = [];

    public IReadOnlyList<SelectOption> SourceOfTruthOptions { get; private set; } = [];

    public bool ShowOwnerSelection => User.IsInRole(RoleNames.Superuser);

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        await LoadOptionsAsync(user, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        var isSuperuser = await _userManager.IsInRoleAsync(user, RoleNames.Superuser);

        if (isSuperuser && !Input.OwnerUserId.HasValue)
        {
            ModelState.AddModelError(nameof(Input.OwnerUserId), "A Hiring Manager owner is required.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(user, cancellationToken);
            return Page();
        }

        await _handler.HandleAsync(
            new CreateTenantProjectCommand(
                user.Id,
                Input.JobTitle,
                Input.Department,
                Input.SourceOfTruthId!.Value,
                isSuperuser ? Input.OwnerUserId : null),
            cancellationToken);

        return Redirect("/tenant/projects");
    }

    private async Task LoadOptionsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        if (user.TenantId is null)
        {
            OwnerOptions = [];
            SourceOfTruthOptions = [];
            return;
        }

        var tenantId = new TenantId(user.TenantId.Value);

        OwnerOptions = await (
            from tenantUser in _dbContext.Users
            join userRole in _dbContext.UserRoles on tenantUser.Id equals userRole.UserId
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where tenantUser.TenantId == user.TenantId.Value && role.Name == RoleNames.HiringManager
            orderby tenantUser.DisplayName
            select new SelectOption(tenantUser.Id, $"{tenantUser.DisplayName} ({tenantUser.Email})"))
            .ToArrayAsync(cancellationToken);

        SourceOfTruthOptions = await _dbContext.TenantSourceOfTruths
            .Where(candidate => candidate.TenantId == tenantId)
            .OrderBy(candidate => candidate.Name)
            .Select(candidate => new SelectOption(candidate.Id.Value, candidate.Name))
            .ToArrayAsync(cancellationToken);
    }

    public sealed class InputModel
    {
        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public Guid? OwnerUserId { get; set; }

        [Required]
        public Guid? SourceOfTruthId { get; set; }
    }

    public sealed record SelectOption(Guid Value, string Label);
}
