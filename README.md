# Oqtane.Module.Template

A `dotnet new` item template for scaffolding Oqtane modules into an existing [Oqtane.Application](https://github.com/oqtane/oqtane.framework) solution.

## Overview

Oqtane's web UI provides a module creation wizard, but it requires manual interaction and a running instance. This template eliminates that bottleneck by generating the full module file stack from the CLI into an existing `Oqtane.Application`-based solution.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An existing solution created from the `Oqtane.Application` template

## Installation

```bash
dotnet new install MarkDav.Oqtane.Module.Template
```

## Usage

From the root of your Oqtane.Application solution:

```bash
dotnet new oqtane-module -n MyModule --namespace MyCompany.MyModule
```

### Parameters

| Parameter | Description | Default |
|---|---|---|
| `-n` / `--name` | Module name (PascalCase) | Required |
| `--namespace` | Root namespace | Inferred from solution |

## Generated File Structure

```
Client/
  Modules/
    MyModule/
      Index.razor
      Edit.razor
      Add.razor
      Detail.razor
Shared/
  Models/
    MyModule.cs
  Interfaces/
    IMyModuleService.cs
Server/
  Controllers/
    MyModuleController.cs
  Managers/
    MyModuleManager.cs
  Repository/
    MyModuleRepository.cs
  Registration/
    MyModuleRegistration.cs
```

## Design Approach

This template follows the same internal manifest conventions used by `Oqtane.Application`, ensuring compatibility with Oqtane's module loading and registration system. It is intentionally scoped to file scaffolding only — module registration in the Oqtane UI remains a one-time manual step per module.

## Related

- [oqtane/oqtane.framework](https://github.com/oqtane/oqtane.framework)
- [Oqtane Module Development Docs](https://docs.oqtane.org)

## License

MIT
