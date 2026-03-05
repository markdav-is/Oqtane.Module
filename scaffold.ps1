#Requires -Version 7
# Run from the root of your Oqtane.Application solution.
# Usage: ./scaffold.ps1 <ModuleName>
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$ModuleName
)

$ErrorActionPreference = 'Stop'

# Find first Client or Server .csproj up to 3 levels deep
$csproj = Get-ChildItem -Path . -Recurse -Depth 3 -Filter '*.csproj' |
    Where-Object { $_.Name -match '(?i)(Client|Server)' } |
    Select-Object -First 1

if (-not $csproj) {
    Write-Error "No Client or Server .csproj found in subdirectories.`nRun manually: dotnet new oqtane-module -n $ModuleName --namespace <YourNamespace>"
    exit 1
}

[xml]$xml = Get-Content $csproj.FullName

$namespace = @($xml.Project.PropertyGroup) |
    ForEach-Object { $_.RootNamespace } |
    Where-Object { $_ } |
    Select-Object -First 1

if (-not $namespace) {
    $namespace = @($xml.Project.PropertyGroup) |
        ForEach-Object { $_.AssemblyName } |
        Where-Object { $_ } |
        Select-Object -First 1
}

if (-not $namespace) {
    Write-Error "Could not read RootNamespace from $($csproj.FullName).`nRun manually: dotnet new oqtane-module -n $ModuleName --namespace <YourNamespace>"
    exit 1
}

Write-Host "Namespace : $namespace  (from $($csproj.Name))"
Write-Host "Module    : $ModuleName"
Write-Host ""
dotnet new oqtane-module -n $ModuleName --namespace $namespace
