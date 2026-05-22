#!/usr/bin/env bash
# Run GbIrl.Web on macOS or Linux (.NET 8 required).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT"

PORT="${PORT:-5136}"
URL="http://127.0.0.1:${PORT}"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: .NET SDK not found. Install .NET 8 from https://dotnet.microsoft.com/download/dotnet/8.0" >&2
  exit 1
fi

DOTNET_VERSION="$(dotnet --version)"
case "$DOTNET_VERSION" in
  8.*) ;;
  *)
    echo "Warning: expected .NET 8.x; found ${DOTNET_VERSION}. The project targets net8.0." >&2
    ;;
esac

echo "==> Restoring packages..."
dotnet restore

echo "==> Building solution..."
if ! dotnet build -m:1 --no-restore -v q; then
  echo "==> Retrying build without -m:1..."
  dotnet build --no-restore -v q
fi

echo "==> Starting web app at ${URL}"
echo "    Home:   ${URL}/"
echo "    Upload: ${URL}/Upload"
echo "    Items:  ${URL}/Items"
echo "    Sample: ${ROOT}/samples/sample_irl.xlsx"
echo "    DB:     ${ROOT}/src/GbIrl.Web/gbirl.db (created on first run)"
echo ""
echo "Press Ctrl+C to stop."
echo ""

if [[ "$(uname -s)" == "Darwin" ]] && [[ "${OPEN_BROWSER:-1}" == "1" ]]; then
  (sleep 2 && open "${URL}/") &
fi

exec dotnet run --project src/GbIrl.Web --no-build --urls "${URL}"
