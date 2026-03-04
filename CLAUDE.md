# CLAUDE.md — Oqtane.Module.Template

## Project

`dotnet new` item template that scaffolds a full Oqtane module into an existing `Oqtane.Application` solution.

Full implementation spec is in `COPILOT_PROMPT.md`.

---

## Stack

- .NET 10
- Blazor (Oqtane.Application pattern)
- Oqtane via NuGet — never reference framework source directly

---

## Implementation Phases

- [x] **Phase 1** — `template.json` + token substitution verification
- [x] **Phase 2** — Shared: `ModuleName.cs`, `IModuleNameService.cs`
- [x] **Phase 3** — Server: `ModuleNameRepository.cs`, `ModuleNameManager.cs`, `ModuleNameController.cs`
- [x] **Phase 4** — Server: `ModuleNameRegistration.cs`
- [x] **Phase 5** — Client: `Index.razor`, `Edit.razor`, `Add.razor`, `Detail.razor`
- [x] **Phase 6** — NuGet packaging (`MarkDav.Oqtane.Module.Template.csproj`)
- [x] **Phase 7** — End-to-end test: scaffold into real Oqtane.Application solution, confirm build

Update this list as phases complete.

---

## Hard Rules

- Always use `IDbContextFactory<ModuleNameContext>` (dedicated per-module context) — never `TenantDBContext`, never inject `DbContext` directly
- `ModuleNameContext` inherits `DBContextBase` and implements `IMultiDatabase` — auto-registered by Oqtane, not manually in `IServerStartup`
- Always inherit `ModuleBase` in Razor components
- Always use `@namespace RootNamespace.Modules.ModuleName` in Razor files
- Register `IModuleNameRepository` via `IServerStartup` — never touch `Program.cs`
- Hard delete (row removal) matches official Oqtane pattern; `ModelBase` provides `IsDeleted` for audit purposes only
- Table naming convention: `RootNamespaceModuleName` — dots preserved from namespace, no separator (e.g. `MyCompany.MyProjectTestModule`)
- Token `ModuleName` substitutes in both file contents and file/folder names
- Token `RootNamespace` substitutes in file contents only
- Manager inherits `MigratableModuleBase`, implements `IInstallable`, `IPortable`, `ISearchable` — runs EF migrations on install
- Controller inherits `ModuleControllerBase`; use `IsAuthorizedEntityId()` — no `[Authorize]` policy attributes
- Add and Edit share a single `Edit.razor` using `PageState.Action` — no separate `Add.razor`
- `ModuleInfo.cs` implements `IModule` — required for Oqtane module discovery
- Client HTTP service (`ModuleNameService`) inherits `ServiceBase`, implements `ITransientService` — no manual client DI registration needed

---

## Template Tokens

| Token | CLI Parameter | Example Value |
|---|---|---|
| `ModuleName` | `-n` | `WeatherArbitrage` |
| `RootNamespace` | `--namespace` | `MarkDav.WeatherArbitrage` |

---

## Repo Structure

```
templates/
  oqtane-module/
    .template.config/
      template.json
    Client/Modules/ModuleName/
    Shared/Models/
    Shared/Interfaces/
    Server/Controllers/
    Server/Managers/
    Server/Repository/
    Server/Registration/
MarkDav.Oqtane.Module.Template.csproj
CLAUDE.md
COPILOT_PROMPT.md
README.md
```

---

## Build & Test Commands

```bash
# Pack the template
dotnet pack

# Install locally
dotnet new install ./bin/Release/MarkDav.Oqtane.Module.Template.1.0.0.nupkg

# Uninstall (before reinstalling after changes)
dotnet new uninstall MarkDav.Oqtane.Module.Template

# Scaffold test (run from root of an Oqtane.Application solution)
dotnet new oqtane-module -n WeatherArbitrage --namespace MarkDav.WeatherArbitrage

# Verify build of host solution after scaffolding
dotnet build
```

---

## Session Start Instructions

1. Read this file and `COPILOT_PROMPT.md`
2. Check the phase checklist above for current status
3. Implement the next incomplete phase
4. Update the phase checklist when complete
5. Do not proceed past a phase if the build is broken
