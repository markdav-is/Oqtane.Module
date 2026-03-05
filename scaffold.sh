#!/usr/bin/env bash
# Run from the root of your Oqtane.Application solution.
# Usage: ./scaffold.sh <ModuleName>
set -euo pipefail

MODULE_NAME="${1:-}"
if [[ -z "$MODULE_NAME" ]]; then
  echo "Usage: ./scaffold.sh <ModuleName>"
  exit 1
fi

# Find first Client or Server .csproj up to 3 levels deep
CSPROJ=$(find . -maxdepth 3 -name "*.csproj" | grep -iE "(Client|Server)" | head -1)

if [[ -z "$CSPROJ" ]]; then
  echo "ERROR: No Client or Server .csproj found in subdirectories."
  echo "       Run manually: dotnet new oqtane-module -n $MODULE_NAME --namespace <YourNamespace>"
  exit 1
fi

# Extract RootNamespace (fall back to AssemblyName)
NAMESPACE=$(sed -n 's:.*<RootNamespace>\([^<]*\)</RootNamespace>.*:\1:p' "$CSPROJ" | head -1 || true)
if [[ -z "$NAMESPACE" ]]; then
  NAMESPACE=$(sed -n 's:.*<AssemblyName>\([^<]*\)</AssemblyName>.*:\1:p' "$CSPROJ" | head -1 || true)
fi

if [[ -z "$NAMESPACE" ]]; then
  echo "ERROR: Could not read RootNamespace from $CSPROJ"
  echo "       Run manually: dotnet new oqtane-module -n $MODULE_NAME --namespace <YourNamespace>"
  exit 1
fi

echo "Namespace : $NAMESPACE  (from $CSPROJ)"
echo "Module    : $MODULE_NAME"
echo ""
dotnet new oqtane-module -n "$MODULE_NAME" --namespace "$NAMESPACE"
