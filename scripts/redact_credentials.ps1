# redact_credentials.ps1
# Redacts the legacy credential VALUE (12345678) from reverse-engineering docs.
# Run from repo root:
#   powershell -ExecutionPolicy Bypass -File scripts\redact_credentials.ps1

$ErrorActionPreference = "Stop"
$repo = (Get-Location).Path

$targets = @(
  "Docs\Complete_Reverse_Engineering_Reference_V2.md",
  "Docs\Business_Rules_V2.md",
  "From-Phase-0-to-3-Handoff.md",
  "Documenting the Executable File.md"
)

$secret      = "12345678"
$replacement = "[REDACTED]"

foreach ($rel in $targets) {
  $path = Join-Path $repo $rel
  if (-not (Test-Path $path)) { Write-Host "SKIP (missing): $rel"; continue }

  $content = [System.IO.File]::ReadAllText($path)
  $count   = ([regex]::Matches($content, [regex]::Escape($secret))).Count

  if ($count -eq 0) { Write-Host "OK (no secret): $rel"; continue }

  # backup once
  $bak = "$path.bak"
  if (-not (Test-Path $bak)) { Copy-Item $path $bak }

  $new = $content.Replace($secret, $replacement)
  [System.IO.File]::WriteAllText($path, $new)
  Write-Host "REDACTED $count occurrence(s): $rel  (backup: $rel.bak)"
}

Write-Host ""
Write-Host "Verify with:"
Write-Host "  Select-String -Path 'Docs\*.md','*.md' -Pattern '12345678'"