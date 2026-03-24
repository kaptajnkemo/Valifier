---
name: Create .NET Solution
description: Create a new .NET solution from scratch when the user asks for a fresh solution scaffold, especially with prompts like "Create a .NET solution called <name>". Use for opinionated solution initialization with strict Clean Architecture, Blazor Web App mixed SSR/CSR frontend, MudBlazor UI, Microsoft Identity authentication, EF Core with LocalDB and migrations for development, and architecture tests that detect dependency drift.
---

# Precedence

- This skill is operational and lower-priority than constitution/skill-1-immutable-authority.md, constitution/skill-2-execution-evidence.md, constitution/skill-3-domain-law.md, and constitution/skill-4-local-work-protection-case-law.md.
- If any instruction in this skill conflicts with any instruction in Constitution layers 1-4, execution is blocked and the conflicting instruction is not applicable.

# Create .NET Solution

Create exactly one new solution rooted at `<solution-name>`. Treat the request as greenfield scaffolding work unless the user explicitly says to update an existing solution.

## Required inputs

- Extract `<solution-name>` from the user request.
- If the solution name is missing, stop and ask for it.
- Default the target path to `<cwd>\<solution-name>` unless the user explicitly provides a different target path.

## Hard-stop safety gate

Before creating any file or directory, inspect the target path and all descendant paths.

Execution is blocked if any of the following is true:

- the target path already contains any `*.sln` file
- the target path already contains any `*.csproj` file
- any intended output project directory already exists
- any intended output file already exists

If any safety gate fails:

- abort immediately
- do not overwrite, merge, rename, or reuse the existing solution or project
- report the exact conflicting path or paths

## Default platform and app model

- Use the latest stable .NET SDK installed on the machine unless the user explicitly pins an older version.
- Verify available templates and SDK state before generation with `dotnet --info` and `dotnet new list`.
- Use a Blazor Web App frontend that mixes SSR and CSR through render modes.
- Treat "hybrid" in this skill as mixed server-side rendering and client-side rendering inside a Blazor Web App.
- Do not create a .NET MAUI, WPF, WinForms, or Blazor Hybrid native host unless the user explicitly asks for that host model.

## Mandatory solution shape

Create this solution structure by default:

```text
<solution-name>.sln

src/
  <solution-name>.Domain/
  <solution-name>.Application/
  <solution-name>.Infrastructure/
  <solution-name>.Contracts/
  <solution-name>.Web/
  <solution-name>.Web.Client/

tests/
  <solution-name>.Domain.Tests/
  <solution-name>.Application.Tests/
  <solution-name>.Infrastructure.Tests/
  <solution-name>.Web.IntegrationTests/
  <solution-name>.ArchitectureTests/
```

If the user explicitly says no CSR pages are needed, `<solution-name>.Web.Client` MAY be omitted. Otherwise include it.

## Mandatory dependency graph

Project references MUST satisfy this graph and no other graph:

- `Domain` -> no solution-project dependencies
- `Application` -> `Domain`
- `Infrastructure` -> `Application`, `Domain`
- `Contracts` -> no solution-project dependencies
- `Web.Client` -> `Contracts`
- `Web` -> `Application`, `Infrastructure`, `Contracts`, and `Web.Client` when `Web.Client` exists
- test projects -> only the production projects they test plus test-only libraries

The following are forbidden:

- `Domain` referencing any other solution project
- `Application` referencing `Infrastructure`, `Web`, or `Web.Client`
- `Infrastructure` referencing `Web` or `Web.Client`
- `Web.Client` referencing `Application`, `Infrastructure`, or `Domain`
- any circular project reference

## Layer responsibilities

### Domain

`<solution-name>.Domain` MUST contain:

- entities
- value objects
- domain events
- enums
- business rules
- domain-native identity model

`Domain` MUST NOT contain:

- EF Core types
- ASP.NET Core types
- Microsoft Identity types
- MudBlazor types
- HTTP abstractions
- persistence attributes or web framework attributes

### Application

`<solution-name>.Application` MUST contain:

- use-case-oriented feature slices
- commands, queries, handlers, validators
- DTOs or models needed by application workflows
- interfaces for infrastructure-dependent behavior

`Application` MUST depend only on `Domain` and BCL packages.

`Application` MUST NOT reference:

- `DbContext`
- `IdentityUser`
- `IdentityRole`
- `UserManager<>`
- `RoleManager<>`
- `ClaimsPrincipal`
- `HttpContext`
- MudBlazor

### Infrastructure

`<solution-name>.Infrastructure` MUST contain:

- EF Core persistence
- SQL Server provider configuration
- development migrations
- Microsoft Identity implementation details
- implementations of application interfaces
- seeders for required development roles and baseline users when the user asks for them

### Contracts

`<solution-name>.Contracts` MUST contain transport contracts only.

Use it for:

- API request models
- API response models
- cross-client DTOs needed by `Web.Client` and future mobile clients

Do not place business logic in `Contracts`.

### Web

`<solution-name>.Web` MUST be the Blazor Web App host and MUST contain:

- DI composition root
- middleware and endpoint configuration
- authentication and authorization pipeline
- MudBlazor registration
- Razor components, layouts, pages, and routeable UI
- server-side enforcement boundaries

`Web` MUST NOT contain domain or persistence logic.

### Web.Client

`<solution-name>.Web.Client` MUST contain:

- CSR-interactive components
- WebAssembly-specific bootstrapping
- client-side services required for browser execution

`Web.Client` MUST NOT contain:

- EF Core
- Microsoft Identity server implementation
- domain business logic

## Blazor frontend constraints

- Use a Blazor Web App, not a native Blazor Hybrid host.
- Mix SSR and CSR intentionally by render mode.
- Default to SSR for most routes.
- Use interactive server rendering for authenticated forms, dashboards, and admin workflows unless browser-only execution is required.
- Use interactive WebAssembly only for pages or components that require CSR.
- Keep the UI in reusable components and layouts.
- Use MudBlazor as the component library.
- Do not place business rules in Razor components.

## Identity model constraints

The domain owns the product identity model.

Create domain-native identity types such as:

- `User`
- `Role`
- `UserId`
- `RoleId`
- `UserRole` or equivalent membership concept

The canonical internal business model MUST be these domain types, not Microsoft Identity types.

Microsoft Identity is an infrastructure adapter only.

Enforce these rules:

- `Domain` and `Application` MUST use domain-native user and role concepts
- `Infrastructure` MUST map Microsoft Identity users and roles to domain-facing abstractions
- `Web` MUST use Microsoft Identity only for authentication and authorization wiring
- `Web`, `Application`, and `Domain` MUST NOT expose `IdentityUser` or `IdentityRole` as internal business entities
- password hashes, MFA secrets, lockout counters, external login details, and token mechanics MUST remain in the infrastructure identity adapter

## Authentication and authorization constraints

- Use Microsoft Identity for authentication and role management.
- Use EF Core stores for Identity persistence.
- Enforce authorization on the server.
- UI authorization checks MAY control visibility, but MUST NOT be the real security boundary.
- Prefer policies and claims where appropriate, but roles MUST exist because the user explicitly requested roles.

## Persistence constraints

- Use EF Core.
- Use SQL Server LocalDB for development.
- Keep connection strings and secrets out of source-controlled production configuration.
- Keep migrations in `Infrastructure`.
- Keep EF entity configuration in `Infrastructure`.
- Do not expose EF entities directly to UI contracts.

Use a development connection string pattern equivalent to:

```text
Server=(localdb)\MSSQLLocalDB;Database=<solution-name>;Trusted_Connection=True;TrustServerCertificate=True;
```

## Architecture test layer

Create `<solution-name>.ArchitectureTests` and make it mandatory.

Use architecture tests to detect Clean Architecture drift. The initial rule set MUST include:

- project-reference graph enforcement
- no circular project references
- `Domain` does not depend on `Application`, `Infrastructure`, `Contracts`, `Web`, or `Web.Client`
- `Domain` does not depend on `Microsoft.AspNetCore`, `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore.Identity`, or `MudBlazor`
- `Application` does not depend on `Infrastructure`, `Web`, `Web.Client`, `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore.Http`, `Microsoft.AspNetCore.Identity`, or `MudBlazor`
- `Infrastructure` does not depend on `Web` or `Web.Client`
- `Web.Client` does not depend on `Application`, `Infrastructure`, or `Domain`
- EF `DbContext` types live only in `Infrastructure`
- EF configuration types live only in `Infrastructure`
- Identity adapter types live only in `Infrastructure`
- Razor components live only in `Web` or `Web.Client`
- domain-native user and role types live only in `Domain`
- only `Infrastructure` may reference Microsoft Identity implementation types

Prefer `ArchUnitNET` for compiled-assembly rules. If project-reference graph checks are easier to express by reading `.csproj` files directly, add a plain test for that instead of weakening the rule.

## Package and tooling expectations

Verify exact package versions against the installed SDK and available stable packages before adding them.

The default stack MUST include stable versions of:

- EF Core SQL Server provider
- EF Core design-time tooling when needed
- Microsoft ASP.NET Core Identity EF Core integration
- MudBlazor
- a test framework for unit and integration tests
- an architecture test package such as `ArchUnitNET`

## Generation workflow

Follow this sequence:

1. Resolve the solution name and target path.
2. Execute the hard-stop safety gate.
3. Verify installed SDK and templates.
4. Create the solution root and solution file.
5. Create all required production and test projects.
6. Add projects to the solution.
7. Add only the approved project references.
8. Add required NuGet packages.
9. Implement the production project structure and baseline composition.
10. Implement architecture tests that enforce the dependency rules.
11. Implement application, infrastructure, and web wiring for Identity, EF Core, MudBlazor, and render modes.
12. Create the initial migration only after the data model is in place.
13. Build and run the relevant tests according to the active governing rules.

## Output expectations

When executing this skill, report:

- the resolved solution name
- the resolved target path
- whether the safety gate passed or the exact reason it blocked execution
- the exact project list created
- the enforced dependency graph
- the verification commands executed and their verbatim output when execution evidence is required

Do not claim completion without the required execution evidence.
