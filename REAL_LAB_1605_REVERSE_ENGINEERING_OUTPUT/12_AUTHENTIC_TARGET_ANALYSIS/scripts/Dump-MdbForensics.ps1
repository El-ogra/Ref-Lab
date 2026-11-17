# Dump-MdbForensics.ps1 - read-only forensic dump of an Access/Jet .mdb via ACE OLEDB
param(
  [Parameter(Mandatory=$true)][string]$Path,
  [Parameter(Mandatory=$true)][string]$OutDir
)
$ErrorActionPreference = 'Continue'
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# hash BEFORE opening the file (ACE takes a handle)
$preHash = (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash
$preSize = (Get-Item -LiteralPath $Path).Length

function Get-Prop($row, $name) {
  try { $v = $row[$name]; if ($v -is [DBNull]) { return $null } return $v } catch { return $null }
}
function Save($obj, $file) {
  $obj | ConvertTo-Json -Depth 6 | Out-File -LiteralPath (Join-Path $OutDir $file) -Encoding utf8
}
$errs = New-Object System.Collections.Generic.List[string]

$provs = @("Microsoft.ACE.OLEDB.16.0", "Microsoft.ACE.OLEDB.12.0")
$conn = $null
$usedProv = $null
foreach ($p in $provs) {
  try {
    $cs = "Provider=$p;Data Source=$Path;Persist Security Info=False;"
    $conn = New-Object System.Data.OleDb.OleDbConnection $cs
    $conn.Open(); $usedProv = $p; break
  } catch { $conn = $null; $errs.Add("PROVIDER_FAIL $p : $($_.Exception.Message)") }
}
if ($null -eq $conn) { $errs | Out-File -LiteralPath (Join-Path $OutDir "errors.txt") -Encoding utf8; Write-Error "no provider opened $Path"; exit 2 }

Save ([ordered]@{ path=$Path; sizeBytes=$preSize; sha256=$preHash; provider=$usedProv; connectionString=$conn.ConnectionString }) "dbinfo.json"

# ---------- tables ----------
$tables = @()
try {
  $s = $conn.GetOleDbSchemaTable([System.Data.OleDb.OleDbSchemaGuid]::Tables, $null)
  foreach ($r in $s.Rows) {
    $n = [string]$r["TABLE_NAME"]
    if ($n -like 'MSys*') { continue }
    $tables += [ordered]@{ name=$n; type=[string]$r["TABLE_TYPE"]; remarks=[string]$r["REMARKS"] }
  }
} catch { $errs.Add("TABLES: $($_.Exception.Message)") }
Save $tables "tables.json"

# ---------- columns ----------
$cols = @()
try {
  $s = $conn.GetOleDbSchemaTable([System.Data.OleDb.OleDbSchemaGuid]::Columns, $null)
  foreach ($r in $s.Rows) {
    $t = [string]$r["TABLE_NAME"]
    if ($t -like 'MSys*') { continue }
    $dt = $null; try { if ($r["DATA_TYPE"] -isnot [DBNull]) { $dt = [int]$r["DATA_TYPE"] } } catch {}
    $cols += [ordered]@{
      table=$t; name=[string]$r["COLUMN_NAME"]
      ordinal = [int]$r["ORDINAL_POSITION"]
      dataTypeOleDb=$dt; typeName=[string]$r["COLUMN_TYPE"]
      size=(Get-Prop $r "COLUMN_SIZE")
      charMaxLen=(Get-Prop $r "CHARACTER_MAXIMUM_LENGTH")
      numericPrecision=(Get-Prop $r "NUMERIC_PRECISION"); numericScale=(Get-Prop $r "NUMERIC_SCALE")
      datetimePrecision=(Get-Prop $r "DATETIME_PRECISION")
      nullable=(Get-Prop $r "NULLABLE"); isUnique=(Get-Prop $r "COLUMN_UNIQUE")
      isKey=(Get-Prop $r "COLUMN_KEY"); isAutoIncrement=(Get-Prop $r "COLUMN_AUTOINCREMENT")
      defaultValue=(Get-Prop $r "COLUMN_DEFAULT"); collation=(Get-Prop $r "COLLATION")
      flags=(Get-Prop $r "COLUMN_FLAGS")
    }
  }
} catch { $errs.Add("COLUMNS: $($_.Exception.Message)") }
Save $cols "columns.json"

# ---------- indexes ----------
$idx = @()
try {
  $s = $conn.GetOleDbSchemaTable([System.Data.OleDb.OleDbSchemaGuid]::Indexes, $null)
  foreach ($r in $s.Rows) {
    $t = [string]$r["TABLE_NAME"]
    if ($t -like 'MSys*') { continue }
    $idx += [ordered]@{
      table=$t; name=[string]$r["INDEX_NAME"]; column=(Get-Prop $r "COLUMN_NAME")
      ordinal=(Get-Prop $r "ORDINAL_POSITION"); type=(Get-Prop $r "TYPE")
      unique=(Get-Prop $r "UNIQUE"); primary=(Get-Prop $r "PRIMARY_KEY")
      clustered=(Get-Prop $r "CLUSTERED"); cardinality=(Get-Prop $r "CARDINALITY")
      filter=(Get-Prop $r "FILTER_CONDITION")
    }
  }
} catch { $errs.Add("INDEXES: $($_.Exception.Message)") }
Save $idx "indexes.json"

# ---------- primary keys ----------
$pks = @()
try {
  $s = $conn.GetOleDbSchemaTable([System.Data.OleDb.OleDbSchemaGuid]::PrimaryKeys, $null)
  foreach ($r in $s.Rows) {
    $t = [string]$r["TABLE_NAME"]
    if ($t -like 'MSys*') { continue }
    $pks += [ordered]@{ table=$t; column=[string]$r["COLUMN_NAME"]; ordinal=[int]$r["ORDINAL_POSITION"] }
  }
} catch { $errs.Add("PK: $($_.Exception.Message)") }
Save $pks "primarykeys.json"

# ---------- foreign keys ----------
$fks = @()
try {
  $s = $conn.GetOleDbSchemaTable([System.Data.OleDb.OleDbSchemaGuid]::ForeignKeys, $null)
  foreach ($r in $s.Rows) {
    $fks += [ordered]@{
      fkName=[string]$r["FK_NAME"]; fkTable=[string]$r["FK_TABLE_NAME"]
      fkColumn=[string]$r["FK_COLUMN_NAME"]; pkTable=[string]$r["PK_TABLE_NAME"]
      pkColumn=[string]$r["PK_COLUMN_NAME"]; def=(Get-Prop $r "FK_DEF_")
    }
  }
} catch { $errs.Add("FK: $($_.Exception.Message)") }
Save $fks "foreignkeys.json"

# ---------- row counts ----------
$counts = @()
foreach ($t in $tables) {
  if ($t.type -ne 'TABLE') { continue }
  $n = $null; $e = $null
  try { $cmd = $conn.CreateCommand(); $cmd.CommandText = "SELECT COUNT(*) FROM [$($t.name)]"; $n = $cmd.ExecuteScalar() }
  catch { $e = $_.Exception.Message }
  $counts += [ordered]@{ table=$t.name; rows=$n; error=$e }
}
Save $counts "rowcounts.json"

# ---------- MSysObjects (Access object catalogue: tables, queries, forms, macros) ----------
$q = @()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "SELECT Id, ParentId, Name, Type, Flags FROM MSysObjects WHERE Name IS NOT NULL ORDER BY Type, Name"
  $rd = $cmd.ExecuteReader()
  while ($rd.Read()) {
    $q += [ordered]@{ id=$rd.GetValue(0); parentId=(Get-Prop $rd "ParentId"); name=[string]$rd.GetValue(2); type=[int]$rd.GetValue(3); flags=[int]$rd.GetValue(4) }
  }
  $rd.Close()
} catch { $errs.Add("MSYSOBJECTS: $($_.Exception.Message)") }
Save $q "msysobjects.json"

# ---------- query definitions ----------
$qd = @()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "SELECT Id, Name, Type FROM MSysQueries ORDER BY Name"
  $rd = $cmd.ExecuteReader()
  while ($rd.Read()) { $qd += [ordered]@{ id=$rd.GetValue(0); name=[string]$rd.GetValue(1); type=[int]$rd.GetValue(2) } }
  $rd.Close()
} catch { $errs.Add("SYSQUERIES: $($_.Exception.Message)") }
Save $qd "sysqueries.json"

# ---------- Access column properties (defaults, required, validation) ----------
$cp = @()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "SELECT Name, Type, Required, AllowZeroLength, DefaultValue, ValidationRule, ValidationText FROM MSysColumns"
  $rd = $cmd.ExecuteReader()
  while ($rd.Read()) {
    $cp += [ordered]@{
      column=[string]$rd.GetValue(0); type=[int]$rd.GetValue(1)
      required=[bool]$rd.GetValue(2); allowZeroLength=[bool]$rd.GetValue(3)
      defaultValue=(Get-Prop $rd "DefaultValue")
      validationRule=(Get-Prop $rd "ValidationRule"); validationText=(Get-Prop $rd "ValidationText")
    }
  }
  $rd.Close()
} catch { $errs.Add("MSYSCOLUMNS: $($_.Exception.Message)") }
Save $cp "msyscolumns.json"

# ---------- Access indexes (MSysIndexes) ----------
$mi = @()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "SELECT Name, Owner, TableId, IndexId, Flags FROM MSysIndexes"
  $rd = $cmd.ExecuteReader()
  while ($rd.Read()) { $mi += [ordered]@{ name=[string]$rd.GetValue(0); owner=[string]$rd.GetValue(1); tableId=$rd.GetValue(2); indexId=$rd.GetValue(3); flags=$rd.GetValue(4) } }
  $rd.Close()
} catch { $errs.Add("MSYSINDEXES: $($_.Exception.Message)") }
Save $mi "msysindexes.json"

# ---------- Access relations (MSysRelationships) ----------
$mr = @()
try {
  $cmd = $conn.CreateCommand()
  $cmd.CommandText = "SELECT Name, RefTableId, RefColumnId, TableId, ColumnId, Flags FROM MSysRelationships"
  $rd = $cmd.ExecuteReader()
  while ($rd.Read()) { $mr += [ordered]@{ name=[string]$rd.GetValue(0); refTableId=$rd.GetValue(1); refColumnId=$rd.GetValue(2); tableId=$rd.GetValue(3); columnId=$rd.GetValue(4); flags=$rd.GetValue(5) } }
  $rd.Close()
} catch { $errs.Add("MSYSRELATIONSHIPS: $($_.Exception.Message)") }
Save $mr "msysrelationships.json"

$conn.Close()
if ($errs.Count -gt 0) { $errs | Out-File -LiteralPath (Join-Path $OutDir "errors.txt") -Encoding utf8 }
"DUMP_OK $Path tables=$($tables.Count) cols=$($cols.Count) idx=$($idx.Count) pk=$($pks.Count) fk=$($fks.Count) msysobj=$($q.Count) sysq=$($qd.Count) msyscol=$($cp.Count) msysidx=$($mi.Count) msysrel=$($mr.Count) errors=$($errs.Count)"
