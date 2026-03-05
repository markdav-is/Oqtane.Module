# Blog Post Notes: Building an Oqtane Module dotnet Template with CI/CD

## What We Built

A `dotnet new` item template — **MarkDav.Oqtane.Module.Template** — that scaffolds a complete, production-ready Oqtane module into an existing Oqtane.Application solution from the command line.

**The problem it solves:** Oqtane has a web UI wizard for creating modules, but it requires a running Oqtane instance and manual interaction. This template lets you scaffold a full module stack with one CLI command.

```bash
dotnet new install MarkDav.Oqtane.Module.Template
dotnet new oqtane-module -n MyModule --namespace MyCompany.MyModule
```

---

## What Gets Generated

15+ files across Oqtane's three-tier architecture:

**Client (Blazor):**
- `Index.razor` — list view component
- `Edit.razor` — add/edit form
- `Settings.razor` — module settings panel
- `ModuleInfo.cs` — IModule implementation
- `ModuleNameService.cs` — typed HTTP client

**Server (ASP.NET Core):**
- `ModuleNameController.cs` — full CRUD REST API with authorization
- `ModuleNameService.cs` — business logic (auto-registered via `ITransientService`)
- `ModuleNameManager.cs` — module lifecycle (`IInstallable`, `IPortable`, `ISearchable`)
- `ModuleNameContext.cs` — EF Core DbContext with multi-database support
- `ModuleNameRepository.cs` — data access layer

**Shared:**
- `ModuleName.cs` — entity model
- `IModuleNameService.cs` — service contract

---

## Interesting Technical Decisions

### Namespace Auto-Detection (v1.0.2)
The template tries to auto-detect the `RootNamespace` from the nearest `.csproj` file using MSBuild binding, so you don't have to pass `--namespace` manually when running from inside a solution:

```json
"namespace": {
  "type": "coalesce",
  "sourceVariableName": "namespaceFromMsBuild",
  "defaultVariableName": "namespaceParam",
  "replaces": "RootNamespace"
}
```

### No Manual DI Registration
Oqtane auto-registers services that implement `ITransientService`, `ISingletonService`, etc. The template leans into this — no `IServerStartup` class is generated, which is a common source of confusion for Oqtane newcomers.

### Multi-Database Support Built In
The generated `DbContext` inherits from `DBContextBase` and implements `IMultiDatabase`, so modules work across SQL Server, MySQL, PostgreSQL, and SQLite out of the box.

---

## CI/CD: Automated NuGet Publishing

Added a GitHub Actions workflow that publishes to NuGet.org automatically on version tags:

```yaml
on:
  push:
    tags:
      - 'v*.*.*'
```

**Steps:**
1. Checkout code
2. Setup .NET 10 SDK
3. `dotnet pack -c Release`
4. `dotnet nuget push` using `secrets.NUGET_API_KEY`
5. `--skip-duplicate` prevents errors if the same version is accidentally re-tagged

**To release a new version:**
```bash
git tag v1.0.2
git push --tags
```

The workflow picks it up and publishes automatically.

### Secret Setup
- Generate API key at nuget.org → Profile → API Keys
- Scope it to the specific package glob: `MarkDav.Oqtane.Module.Template`
- Add as `NUGET_API_KEY` in GitHub repo Settings → Secrets and variables → Actions

---

## Tech Stack

- **.NET 10** (template targets net10.0)
- **Oqtane Framework** — modular Blazor CMS/app framework
- **Entity Framework Core** — with EF migrations pre-generated
- **GitHub Actions** — CI/CD for NuGet publishing
- **dotnet new templating engine** — `PackageType: Template` in .csproj

---

## Links

- NuGet: [MarkDav.Oqtane.Module.Template](https://www.nuget.org/packages/MarkDav.Oqtane.Module.Template)
- GitHub: [markdav-is/Oqtane.Module](https://github.com/markdav-is/Oqtane.Module)
- Oqtane Framework: [oqtane/oqtane.framework](https://github.com/oqtane/oqtane.framework)

---

## Potential Blog Angles

- "Automate your Oqtane module scaffolding with a custom dotnet template"
- "From zero to NuGet: packaging and publishing a dotnet new template with GitHub Actions"
- "The dotnet templating engine's hidden superpower: MSBuild binding for namespace auto-detection"
- "Why I stopped writing Oqtane module boilerplate by hand"
