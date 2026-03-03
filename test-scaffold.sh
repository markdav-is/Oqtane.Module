#!/usr/bin/env bash
# Phase 7 end-to-end test: pack, install, scaffold, verify
# Run this from the root of the Oqtane.Module.Template repo.
# Prerequisites: .NET 10 SDK, an existing Oqtane.Application solution at $OQTANE_SOLUTION_DIR

set -euo pipefail

# Allow overriding package metadata via environment; otherwise derive from the .csproj
: "${CSPROJ_FILE:=$(ls *.csproj 2>/dev/null | head -n 1)}"
if [ ! -f "$CSPROJ_FILE" ]; then
    echo "ERROR: Could not find .csproj file in the current directory."
    echo "Set CSPROJ_FILE, PACKAGE_ID, and PACKAGE_VERSION environment variables as needed."
    exit 1
fi

: "${PACKAGE_ID:=$(grep -m1 '<PackageId>' "$CSPROJ_FILE" | sed -E 's/.*<PackageId>(.+)<\/PackageId>.*/\1/')}"
: "${PACKAGE_VERSION:=$(grep -m1 '<PackageVersion>' "$CSPROJ_FILE" | sed -E 's/.*<PackageVersion>(.+)<\/PackageVersion>.*/\1/')}"

if [ -z "${PACKAGE_ID:-}" ] || [ -z "${PACKAGE_VERSION:-}" ]; then
    echo "ERROR: Failed to determine PACKAGE_ID or PACKAGE_VERSION."
    echo "You can set PACKAGE_ID and PACKAGE_VERSION as environment variables to override detection."
    exit 1
fi
PACKAGE_FILE="bin/Release/${PACKAGE_ID}.${PACKAGE_VERSION}.nupkg"

MODULE_NAME="WeatherArbitrage"
ROOT_NAMESPACE="MarkDav.WeatherArbitrage"

# Path to an existing Oqtane.Application solution root (override with env var)
OQTANE_SOLUTION_DIR="${OQTANE_SOLUTION_DIR:-/tmp/OqtaneApp}"

echo "=== Step 1: Pack ==="
dotnet pack -c Release
echo "Packed: $PACKAGE_FILE"

echo ""
echo "=== Step 2: Uninstall any previous version ==="
dotnet new uninstall "$PACKAGE_ID" 2>/dev/null || echo "(not installed, skipping)"

echo ""
echo "=== Step 3: Install from local package ==="
dotnet new install "$PACKAGE_FILE"

echo ""
echo "=== Step 4: Scaffold into target solution ==="
if [ ! -d "$OQTANE_SOLUTION_DIR" ]; then
    echo "ERROR: Oqtane.Application solution not found at $OQTANE_SOLUTION_DIR"
    echo "Set OQTANE_SOLUTION_DIR to the root of an existing Oqtane.Application solution."
    echo ""
    echo "Expected directory structure:"
    echo "  \$OQTANE_SOLUTION_DIR/"
    echo "    Client/"
    echo "    Server/"
    echo "    Shared/"
    echo "    *.sln"
    exit 1
fi

cd "$OQTANE_SOLUTION_DIR"
dotnet new oqtane-module -n "$MODULE_NAME" --namespace "$ROOT_NAMESPACE"

echo ""
echo "=== Step 5: Verify generated files ==="
EXPECTED_FILES=(
    "Client/Modules/${MODULE_NAME}/Index.razor"
    "Client/Modules/${MODULE_NAME}/Add.razor"
    "Client/Modules/${MODULE_NAME}/Edit.razor"
    "Client/Modules/${MODULE_NAME}/Detail.razor"
    "Shared/Models/${MODULE_NAME}.cs"
    "Shared/Interfaces/I${MODULE_NAME}Service.cs"
    "Server/Controllers/${MODULE_NAME}Controller.cs"
    "Server/Managers/${MODULE_NAME}Manager.cs"
    "Server/Repository/${MODULE_NAME}Repository.cs"
    "Server/Registration/${MODULE_NAME}Registration.cs"
)

PASS=0
FAIL=0
for f in "${EXPECTED_FILES[@]}"; do
    if [ -f "$f" ]; then
        echo "  [OK] $f"
        PASS=$((PASS + 1))
    else
        echo "  [MISSING] $f"
        FAIL=$((FAIL + 1))
    fi
done

echo ""
echo "=== Step 6: Verify token substitution ==="
# Check that RootNamespace token was replaced (no literal "RootNamespace" should remain)
RAW_TOKEN_COUNT=$(grep -r "RootNamespace" . --include="*.cs" --include="*.razor" 2>/dev/null | grep -v ".git" | wc -l)
if [ "$RAW_TOKEN_COUNT" -eq 0 ]; then
    echo "  [OK] No leftover RootNamespace tokens in generated files"
else
    echo "  [FAIL] Found $RAW_TOKEN_COUNT file(s) still containing 'RootNamespace'"
    grep -r "RootNamespace" . --include="*.cs" --include="*.razor" -l 2>/dev/null | grep -v ".git"
    FAIL=$((FAIL + 1))
fi

# Check that ModuleName token was replaced
RAW_MODULE_COUNT=$(grep -r '"ModuleName"' . --include="*.cs" --include="*.razor" 2>/dev/null | grep -v ".git" | wc -l)
if [ "$RAW_MODULE_COUNT" -eq 0 ]; then
    echo "  [OK] No leftover ModuleName tokens in generated files"
else
    echo "  [FAIL] Found $RAW_MODULE_COUNT file(s) still containing literal 'ModuleName'"
    FAIL=$((FAIL + 1))
fi

echo ""
echo "=== Step 7: dotnet build ==="
if dotnet build; then
    BUILD_EXIT=0
else
    BUILD_EXIT=$?
fi

echo ""
echo "=== Results ==="
echo "  Files: $PASS OK, $FAIL FAILED"
if [ $BUILD_EXIT -eq 0 ]; then
    echo "  Build: PASSED"
else
    echo "  Build: FAILED (exit code $BUILD_EXIT)"
fi

if [ $FAIL -eq 0 ] && [ $BUILD_EXIT -eq 0 ]; then
    echo ""
    echo "Phase 7 PASSED: scaffold + build successful."
    exit 0
else
    echo ""
    echo "Phase 7 FAILED. Review errors above."
    exit 1
fi
