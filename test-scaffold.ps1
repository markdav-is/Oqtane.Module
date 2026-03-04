#Requires -Version 7
<#
.SYNOPSIS
    Phase 7 end-to-end test: pack, install, scaffold, verify.

.DESCRIPTION
    Run this from the root of the Oqtane.Module.Template repo.
    Prerequisites: .NET 10 SDK, an existing Oqtane.Application solution at $OqtaneSolutionDir.

.PARAMETER OqtaneSolutionDir
    Path to the root of an existing Oqtane.Application solution.
    Defaults to C:\Temp\OqtaneApp or /tmp/OqtaneApp on non-Windows.
#>

param(
    [string]$OqtaneSolutionDir
)

$ErrorActionPreference = 'Stop'

# --- Resolve package metadata from .csproj ---
$csprojFile = Get-Item *.csproj -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $csprojFile) {
    Write-Error "Could not find a .csproj file in the current directory."
    exit 1
}

[xml]$csproj = Get-Content $csprojFile.FullName
$packageId      = $csproj.Project.PropertyGroup.PackageId      | Select-Object -First 1
$packageVersion = $csproj.Project.PropertyGroup.PackageVersion | Select-Object -First 1

if (-not $packageId -or -not $packageVersion) {
    Write-Error "Failed to read PackageId or PackageVersion from $($csprojFile.Name)."
    exit 1
}

$packageFile = "bin/Release/$packageId.$packageVersion.nupkg"

# --- Module names (suffix avoids collisions on repeated runs) ---
$suffix        = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$moduleName    = "WeatherArbitrage$suffix"
$rootNamespace = "MarkDav.WeatherArbitrage$suffix"

# --- Resolve solution directory ---
if (-not $OqtaneSolutionDir) {
    $OqtaneSolutionDir = if ($IsWindows) { 'C:\Temp\OqtaneApp' } else { '/tmp/OqtaneApp' }
}

# ============================================================
Write-Host "`n=== Step 1: Pack ===" -ForegroundColor Cyan
dotnet pack -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "Packed: $packageFile"

# ============================================================
Write-Host "`n=== Step 2: Uninstall any previous version ===" -ForegroundColor Cyan
dotnet new uninstall $packageId 2>$null
Write-Host "(uninstalled or was not installed)"

# ============================================================
Write-Host "`n=== Step 3: Install from local package ===" -ForegroundColor Cyan
dotnet new install $packageFile
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# ============================================================
Write-Host "`n=== Step 4: Scaffold into target solution ===" -ForegroundColor Cyan
if (-not (Test-Path $OqtaneSolutionDir -PathType Container)) {
    Write-Host "ERROR: Oqtane.Application solution not found at: $OqtaneSolutionDir" -ForegroundColor Red
    Write-Host "Set -OqtaneSolutionDir to the root of an existing Oqtane.Application solution."
    Write-Host ""
    Write-Host "Expected structure:"
    Write-Host "  <OqtaneSolutionDir>/"
    Write-Host "    Client/"
    Write-Host "    Server/"
    Write-Host "    Shared/"
    Write-Host "    *.sln"
    exit 1
}

Push-Location $OqtaneSolutionDir
try {
    dotnet new oqtane-module -n $moduleName --namespace $rootNamespace
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally {
    Pop-Location
}

# ============================================================
Write-Host "`n=== Step 5: Verify generated files ===" -ForegroundColor Cyan
$expectedFiles = @(
    "Client/Modules/$moduleName/Index.razor"
    "Client/Modules/$moduleName/Edit.razor"
    "Client/Modules/$moduleName/Settings.razor"
    "Client/Modules/$moduleName/ModuleInfo.cs"
    "Client/Services/${moduleName}Service.cs"
    "Shared/Models/$moduleName.cs"
    "Shared/Interfaces/I${moduleName}Service.cs"
    "Server/Controllers/${moduleName}Controller.cs"
    "Server/Managers/${moduleName}Manager.cs"
    "Server/Repository/${moduleName}Repository.cs"
    "Server/Repository/${moduleName}Context.cs"
    "Server/Registration/${moduleName}Registration.cs"
    "Server/Migrations/${moduleName}ContextModelSnapshot.cs"
    "Server/Migrations/20240101000000_${moduleName}InitialCreate.cs"
)

$pass = 0
$fail = 0
foreach ($f in $expectedFiles) {
    $full = Join-Path $OqtaneSolutionDir $f
    if (Test-Path $full) {
        Write-Host "  [OK]      $f" -ForegroundColor Green
        $pass++
    } else {
        Write-Host "  [MISSING] $f" -ForegroundColor Red
        $fail++
    }
}

# ============================================================
Write-Host "`n=== Step 6: Verify token substitution ===" -ForegroundColor Cyan

$csRazorFiles = Get-ChildItem $OqtaneSolutionDir -Recurse -Include *.cs, *.razor |
    Where-Object { $_.FullName -notmatch '[\\/]\.git[\\/]' }

$rawNamespaceHits = $csRazorFiles | Select-String 'RootNamespace'
if ($rawNamespaceHits.Count -eq 0) {
    Write-Host "  [OK] No leftover RootNamespace tokens" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] $($rawNamespaceHits.Count) hit(s) still contain 'RootNamespace':" -ForegroundColor Red
    $rawNamespaceHits | ForEach-Object { Write-Host "        $($_.Filename):$($_.LineNumber)" }
    $fail++
}

$rawModuleHits = $csRazorFiles | Select-String '"ModuleName"'
if ($rawModuleHits.Count -eq 0) {
    Write-Host "  [OK] No leftover ModuleName tokens" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] $($rawModuleHits.Count) hit(s) still contain literal 'ModuleName':" -ForegroundColor Red
    $rawModuleHits | ForEach-Object { Write-Host "        $($_.Filename):$($_.LineNumber)" }
    $fail++
}

# ============================================================
Write-Host "`n=== Step 7: dotnet build ===" -ForegroundColor Cyan
Push-Location $OqtaneSolutionDir
try {
    dotnet build
    $buildExit = $LASTEXITCODE
} finally {
    Pop-Location
}

# ============================================================
Write-Host "`n=== Results ===" -ForegroundColor Cyan
Write-Host "  Files : $pass OK, $fail FAILED"
if ($buildExit -eq 0) {
    Write-Host "  Build : PASSED" -ForegroundColor Green
} else {
    Write-Host "  Build : FAILED (exit $buildExit)" -ForegroundColor Red
}

if ($fail -eq 0 -and $buildExit -eq 0) {
    Write-Host "`nPhase 7 PASSED: scaffold + build successful." -ForegroundColor Green
    exit 0
} else {
    Write-Host "`nPhase 7 FAILED. Review errors above." -ForegroundColor Red
    exit 1
}
