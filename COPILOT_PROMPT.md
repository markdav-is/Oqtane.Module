# Copilot Implementation Prompt — Oqtane.Module.Template

## Project Goal

Build a `dotnet new` **item template** that scaffolds a full Oqtane module file stack into an existing `Oqtane.Application` solution. This is CLI-driven scaffolding only — no interaction with a running Oqtane instance.

## Target Usage

The developer runs this from the root of an existing Oqtane.Application solution:

```bash
dotnet new oqtane-module -n WeatherArbitrage --namespace MarkDav.WeatherArbitrage
```

This generates the complete module file structure in-place within the existing Client/Server/Shared projects.

---

## Template Mechanics

### `dotnet new` Item Template Structure

```
templates/
  oqtane-module/
    .template.config/
      template.json          ← dotnet new manifest
    Client/
      Modules/
        ModuleName/
          Index.razor
          Edit.razor
          Add.razor
          Detail.razor
    Shared/
      Models/
        ModuleName.cs
      Interfaces/
        IModuleNameService.cs
    Server/
      Controllers/
        ModuleNameController.cs
      Managers/
        ModuleNameManager.cs
      Repository/
        ModuleNameRepository.cs
      Registration/
        ModuleNameRegistration.cs
MarkDav.Oqtane.Module.Template.csproj
nuget.config
README.md
```

### `template.json` Requirements

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "MarkDav",
  "classifications": ["Oqtane", "Blazor", "Module"],
  "identity": "MarkDav.Oqtane.Module.Template",
  "name": "Oqtane Module",
  "shortName": "oqtane-module",
  "tags": {
    "language": "C#",
    "type": "item"
  },
  "symbols": {
    "ModuleName": {
      "type": "parameter",
      "datatype": "string",
      "replaces": "ModuleName",
      "fileRename": "ModuleName",
      "isRequired": true
    },
    "RootNamespace": {
      "type": "parameter",
      "datatype": "string",
      "replaces": "RootNamespace",
      "defaultValue": "MyCompany.MyApp"
    }
  }
}
```

Token substitution rules:
- `ModuleName` → replaced with `-n` value (e.g. `WeatherArbitrage`)
- `RootNamespace` → replaced with `--namespace` value (e.g. `MarkDav.WeatherArbitrage`)
- Both apply to **file contents and file/folder names**

---

## Oqtane-Specific Conventions

### Shared/Models/ModuleName.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RootNamespace.Models
{
    [Table("RootNamespaceModuleName")]
    public class ModuleName
    {
        [Key]
        public int ModuleNameId { get; set; }
        public int ModuleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
```

### Shared/Interfaces/IModuleNameService.cs
```csharp
using Oqtane.Models;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RootNamespace.Services
{
    public interface IModuleNameService
    {
        Task<List<ModuleName>> GetModuleNamesAsync(int moduleId);
        Task<ModuleName> GetModuleNameAsync(int moduleNameId);
        Task<ModuleName> AddModuleNameAsync(ModuleName moduleName);
        Task<ModuleName> UpdateModuleNameAsync(ModuleName moduleName);
        Task DeleteModuleNameAsync(int moduleNameId);
    }
}
```

### Server/Controllers/ModuleNameController.cs
- Inherit from `Controller`
- Use `[Route("api/[controller]")]`
- Inject `IModuleNameManager` and `ILogManager`
- Authorize with `[Authorize(Policy = "ViewModule")]` / `[Authorize(Policy = "EditModule")]`
- Standard CRUD: GET list, GET by id, POST, PUT, DELETE

### Server/Managers/ModuleNameManager.cs
- Inject `IModuleNameRepository`
- Implement `IModuleNameService` interface
- Straightforward pass-through to repository

### Server/Repository/ModuleNameRepository.cs
- Inject `IDbContextFactory<TenantDBContext>`
- Implement full CRUD using EF Core
- Use `using var db = _dbContextFactory.CreateDbContext();` pattern
- Soft delete: set `IsDeleted = true` rather than hard delete

### Server/Registration/ModuleNameRegistration.cs
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;

namespace RootNamespace
{
    public class ModuleNameRegistration : IServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IModuleNameService, ModuleNameManager>();
            services.AddTransient<IModuleNameRepository, ModuleNameRepository>();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider) { }
    }
}
```

### Client/Modules/ModuleName/Index.razor
```razor
@namespace RootNamespace.Modules.ModuleName
@inherits ModuleBase

<div class="container">
    @if (_moduleNames == null)
    {
        <p>Loading...</p>
    }
    else
    {
        <!-- list rendering here -->
    }
</div>

@code {
    private List<Models.ModuleName> _moduleNames;

    protected override async Task OnInitializedAsync()
    {
        // inject and call IModuleNameService
    }
}
```

- All Razor components inherit `ModuleBase`
- Use `@namespace RootNamespace.Modules.ModuleName`
- Inject services via `[Inject]` attribute or `@inject`
- Edit/Add components use `NavigationManager` to return to Index

---

## NuGet Packaging

The `.csproj` should pack as a template:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <PackageId>MarkDav.Oqtane.Module.Template</PackageId>
    <PackageVersion>1.0.0</PackageVersion>
    <Title>Oqtane Module Item Template</Title>
    <Authors>MarkDav</Authors>
    <PackageType>Template</PackageType>
    <TargetFramework>net10.0</TargetFramework>
    <IncludeContentInPack>true</IncludeContentInPack>
    <IncludeBuildOutput>false</IncludeBuildOutput>
    <ContentTargetFolders>content</ContentTargetFolders>
  </PropertyGroup>
  <ItemGroup>
    <Content Include="templates/**/*" Exclude="templates/**/obj/**;templates/**/bin/**" />
  </ItemGroup>
</Project>
```

Build and install locally for testing:
```bash
dotnet pack
dotnet new install ./bin/Release/MarkDav.Oqtane.Module.Template.1.0.0.nupkg
```

---

## Constraints & Notes

- Target **.NET 10** throughout
- Follow Oqtane.Application conventions — do NOT reference Oqtane Framework source directly; use NuGet packages
- The template generates files only — no DB migration, no running instance required
- Namespace inference from solution is a stretch goal; `--namespace` parameter is required for now
- Keep generated code minimal but functional — stubs are fine, no business logic
- Use `IDbContextFactory<TenantDBContext>` not direct `DbContext` injection (Oqtane multi-tenant pattern)
- All service registrations go through `IServerStartup` implementation, not `Program.cs`

---

## Implementation Order

1. `template.json` + token substitution verification
2. Shared models and interfaces
3. Server repository → manager → controller
4. Client Razor components (Index first, then Edit/Add/Detail)
5. Registration class
6. NuGet packaging and local install test
7. End-to-end test: scaffold into a real Oqtane.Application solution and confirm it builds
