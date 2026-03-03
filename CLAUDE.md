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
- [ ] **Phase 5** — Client: `Index.razor`, `Edit.razor`, `Add.razor`, `Detail.razor`
- [ ] **Phase 6** — NuGet packaging (`MarkDav.Oqtane.Module.Template.csproj`)
- [ ] **Phase 7** — End-to-end test: scaffold into real Oqtane.Application solution, confirm build

Update this list as phases complete.

---

## Hard Rules

- Always use `IDbContextFactory<TenantDBContext>` — never inject `DbContext` directly
- Always inherit `ModuleBase` in Razor components
- Always use `@namespace RootNamespace.Modules.ModuleName` in Razor files
- Always register services via `IServerStartup` — never touch `Program.cs`
- Soft delete only — set `IsDeleted = true`, never hard delete
- Table naming convention: `RootNamespaceModuleName` — dots preserved from namespace, no separator between namespace and module name (e.g. `MyCompany.MyProjectTestModule`)
- Token `ModuleName` substitutes in both file contents and file/folder names
- Token `RootNamespace` substitutes in file contents only

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
