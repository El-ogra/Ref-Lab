# Query-Mdb.ps1 - run read-only queries against an Access/Jet .mdb
param(
  [Parameter(Mandatory=$true)][string]$Path,
  [Parameter(Mandatory=$true)][string[]]$Query,
  [string]$OutFile
)
$ErrorActionPreference = 'Continue'
$conn = New-Object System.Data.OleDb.OleDbConnection "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=$Path;"
$conn.Open()
$lines = New-Object System.Collections.Generic.List[string]
foreach ($sql in $Query) {
  $lines.Add("### $sql")
  try {
    $cmd = $conn.CreateCommand(); $cmd.CommandText = $sql
    $rd = $cmd.ExecuteReader()
    $hdr = @(); for ($i = 0; $i -lt $rd.FieldCount; $i++) { $hdr += $rd.GetName($i) }
    $lines.Add(($hdr -join " | "))
    $n = 0
    while ($rd.Read()) {
      $o = @()
      for ($i = 0; $i -lt $rd.FieldCount; $i++) {
        if ($rd.IsDBNull($i)) { $o += "" } else { $o += ([string]$rd.GetValue($i)) }
      }
      $lines.Add(($o -join " | "))
      $n++
      if ($n -ge 500) { $lines.Add("... (truncated at 500 rows)"); break }
    }
    $rd.Close()
  } catch {
    $lines.Add("ERROR: $($_.Exception.Message)")
  }
  $lines.Add("")
}
$conn.Close()
if ($OutFile) { $lines | Out-File -LiteralPath $OutFile -Encoding utf8; "WROTE $OutFile ($($lines.Count) lines)" }
else { $lines -join "`n" }
