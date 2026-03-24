using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.Provisioning;
using Valifier.Domain.Identity;

namespace Valifier.Web.Pages.Admin.Tenants;

[Authorize(Roles = RoleNames.Admin)]
public sealed class NewModel : PageModel
{
    private readonly CreateTenantCommandHandler _handler;

    public NewModel(CreateTenantCommandHandler handler)
    {
        _handler = handler;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _handler.HandleAsync(
            new CreateTenantCommand(
                Input.TenantName,
                Input.InitialSuperuserDisplayName,
                Input.InitialSuperuserEmail,
                Input.InitialSuperuserPassword),
            cancellationToken);

        return Redirect("/admin/dashboard");
    }

    public sealed class InputModel
    {
        [Required]
        public string TenantName { get; set; } = string.Empty;

        [Required]
        public string InitialSuperuserDisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string InitialSuperuserEmail { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string InitialSuperuserPassword { get; set; } = string.Empty;
    }
}
