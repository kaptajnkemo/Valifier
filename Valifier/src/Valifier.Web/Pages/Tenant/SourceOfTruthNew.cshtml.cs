using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valifier.Application.Features.Tenants.TenantSourceOfTruths;
using Valifier.Domain.Identity;
using Valifier.Infrastructure.Identity;

namespace Valifier.Web.Pages.Tenant;

[Authorize(Roles = RoleNames.Superuser)]
public sealed class SourceOfTruthNewModel : PageModel
{
    private readonly CreateTenantSourceOfTruthCommandHandler _handler;
    private readonly UserManager<ApplicationUser> _userManager;

    public SourceOfTruthNewModel(
        CreateTenantSourceOfTruthCommandHandler handler,
        UserManager<ApplicationUser> userManager)
    {
        _handler = handler;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<EntryInputModel> Entries => DeserializeEntries(Input.EntriesJson);

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPostAddEntry()
    {
        if (string.IsNullOrWhiteSpace(Input.EntryKey) ||
            string.IsNullOrWhiteSpace(Input.EntryLabel) ||
            string.IsNullOrWhiteSpace(Input.EntryValueType) ||
            string.IsNullOrWhiteSpace(Input.EntryValue))
        {
            ModelState.AddModelError(string.Empty, "Entry values are required.");
            return Page();
        }

        var entries = DeserializeEntries(Input.EntriesJson).ToList();
        entries.Add(new EntryInputModel(Input.EntryKey, Input.EntryLabel, Input.EntryValueType, Input.EntryValue));
        Input.EntriesJson = JsonSerializer.Serialize(entries);
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        var entries = DeserializeEntries(Input.EntriesJson);

        if (entries.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "At least one entry is required.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Redirect("/sign-in");
        }

        await _handler.HandleAsync(
            new CreateTenantSourceOfTruthCommand(
                user.Id,
                Input.Topic,
                Input.Name,
                Input.SchemaVersion,
                entries
                    .Select(entry => new CreateTenantSourceOfTruthEntryInput(entry.Key, entry.Label, entry.ValueType, entry.Value))
                    .ToArray()),
            cancellationToken);

        return Redirect("/tenant/sots");
    }

    private static IReadOnlyList<EntryInputModel> DeserializeEntries(string? entriesJson)
    {
        return string.IsNullOrWhiteSpace(entriesJson)
            ? []
            : JsonSerializer.Deserialize<List<EntryInputModel>>(entriesJson) ?? [];
    }

    public sealed class InputModel
    {
        [Required]
        public string Topic { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SchemaVersion { get; set; } = string.Empty;

        public string EntryKey { get; set; } = string.Empty;

        public string EntryLabel { get; set; } = string.Empty;

        public string EntryValueType { get; set; } = string.Empty;

        public string EntryValue { get; set; } = string.Empty;

        public string EntriesJson { get; set; } = string.Empty;
    }

    public sealed record EntryInputModel(
        string Key,
        string Label,
        string ValueType,
        string Value);
}
