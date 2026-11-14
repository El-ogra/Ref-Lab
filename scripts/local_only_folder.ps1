# local_only_folder.ps1
# Creates "Docs\Outside the Cloud" (local-only, gitignored) and moves .bak + chosen files into it.
# Run from repo root:
#   powershell -ExecutionPolicy Bypass -File scripts\local_only_folder.ps1

$ErrorActionPreference = "Stop"
$repo = "C:\Users\LAP LINK\source\repos\Ref Lab"
$localDir = Join-Path $repo "Docs\Outside the Cloud"

# 1) Create folder if missing
if (-not (Test-Path $localDir)) {
  New-Item -ItemType Directory -Path $localDir | Out-Null
  Write-Host "Created: $localDir"
} else {
  Write-Host "Already exists: $localDir"
}

# 2) Move all .bak files (preserve relative path)
$baks = Get-ChildItem -Path $repo -Recurse -Filter *.bak -File
foreach ($b in $baks) {
  $rel = $b.FullName.Substring($repo.Length + 1)
  $dest = Join-Path $localDir $rel
  $destDir = Split-Path $dest -Parent
  if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
  Move-Item -Path $b.FullName -Destination $dest -Force
  Write-Host "Moved: $rel -> Docs\Outside the Cloud\$rel"
}

# 3) Optional: move other local-only files (edit list as needed)
$extra = @(
  "evidence\reverse-engineering\exports\tables\testtable.json",
  "evidence\reverse-engineering\exports\tables\normalrange.json"
)
foreach ($rel in $extra) {
  $src = Join-Path $repo $rel
  if (Test-Path $src) {
    $dest = Join-Path $localDir $rel
    $destDir = Split-Path $dest -Parent
    if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
    Move-Item -Path $src -Destination $dest -Force
    Write-Host "Moved (extra): $rel"
  }
}

# 4) Ensure gitignored
$gi = Join-Path $repo ".gitignore"
$line = "Docs/Outside the Cloud/"
if (Test-Path $gi) {
  $content = [System.IO.File]::ReadAllText($gi)
  if ($content -notmatch [regex]::Escape($line)) {
    [System.IO.File]::AppendAllText($gi, "`n# Local-only artifacts (never push)`n$line`n")
    Write-Host "Added to .gitignore: $line"
  } else {
    Write-Host ".gitignore already contains: $line"
  }
}

Write-Host ""
Write-Host "Done. Folder: $localDir"
Write-Host "Verify ignored: git check-ignore 'Docs/Outside the Cloud/test.bak'"