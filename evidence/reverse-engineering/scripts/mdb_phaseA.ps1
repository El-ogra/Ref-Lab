# Phase A: ADOX catalogue + per-table metadata (columns, indexes) + row counts.
$ErrorActionPreference = "Stop"
$mdb = "F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb"
$out = "C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\mdb"
New-Item -ItemType Directory -Force -Path $out | Out-Null

$cat = New-Object -ComObject ADOX.Catalog
$cat.ActiveConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$mdb;Mode=Read;"

$conn = New-Object -ComObject ADODB.Connection
$conn.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$mdb;Mode=Read;", [string]::Empty, [string]::Empty)

# ---- tables
$tblMeta = @()
foreach ($t in $cat.Tables) {
    $cols = @()
    foreach ($c in $t.Columns) {
        $cols += [pscustomobject]@{
            Table = $t.Name; Column = $c.Name; Position = $c.Position; Type = $c.Type
            Size = $c.Size; Required = $c.Required; AllowZeroLength = $c.AllowZeroLength
            Default = $c.Default; ValidationRule = $c.ValidationRule; ValidationText = $c.ValidationText
            Format = $c.Format; InputMask = $c.InputMask; Attributes = $c.Attributes
            NumericPrecision = $c.NumericPrecision; NumericScale = $c.NumericScale
            AutoIncrement = $c.AutoIncrement; Owned = $c.Owned; Expr = $c.Expr
        }
    }
    $idx = @()
    foreach ($i in $t.Indexes) {
        $ic = @()
        foreach ($ic2 in $i.Columns) { $ic += ("{0}{1}" -f $ic2.Name, $(if ($ic2.Descending) { " DESC" } else { " ASC" })) }
        $idx += [pscustomobject]@{
            Table = $t.Name; Index = $i.Name; Type = $i.Type; Primary = $i.Primary
            Unique = $i.Unique; IgnoreNulls = $i.IgnoreNulls; Required = $i.Required
            Clustered = $i.Clustered; Columns = ($ic -join ", ")
        }
    }
    $tblMeta += [pscustomobject]@{
        Table = $t.Name; Type = $t.Type; DateCreated = $t.DateCreated; DateModified = $t.DateModified
        ColumnCount = $cols.Count; IndexCount = $idx.Count; Columns = $cols; Indexes = $idx
    }
}
$tblMeta | Export-Csv "$out\adox_tables.csv" -NoTypeInformation -Encoding UTF8
$tblMeta | ForEach-Object { $_.Columns } | Export-Csv "$out\adox_columns.csv" -NoTypeInformation -Encoding UTF8
$tblMeta | ForEach-Object { $_.Indexes } | Export-Csv "$out\adox_indexes.csv" -NoTypeInformation -Encoding UTF8
"tables: $($tblMeta.Count)"
"columns: " + ($tblMeta | ForEach-Object { $_.ColumnCount } | Measure-Object -Sum).Sum
"indexes: " + ($tblMeta | ForEach-Object { $_.IndexCount } | Measure-Object -Sum).Sum
"--- table type histogram ---"
$tblMeta | Group-Object Type | ForEach-Object { "  type {0} : {1}" -f $_.Name, $_.Count }
"--- system tables ---"
($tblMeta | Where-Object { $_.Table -like "MSys*" }).Table -join ", "

# ---- relations
$rel = @()
foreach ($r in $cat.Relations) {
    $rc = @(); foreach ($x in $r.Columns) { $rc += $x.Name }
    $rf = @(); foreach ($x in $r.ForeignColumns) { $rf += $x.Name }
    $rel += [pscustomobject]@{
        Relation = $r.Name; Table = $r.Table; ForeignTable = $r.ForeignTable
        Columns = ($rc -join ", "); ForeignColumns = ($rf -join ", ")
        Attributes = $r.Attributes; IsCascadeOnUpdate = $r.IsCascadeOnUpdate
        IsCascadeOnDelete = $r.IsCascadeOnDelete; IsLeftJoin = $r.IsLeftJoin
    }
}
$rel | Export-Csv "$out\adox_relations.csv" -NoTypeInformation -Encoding UTF8
"relations: $($rel.Count)"

# ---- procedures (saved queries)
$proc = @()
foreach ($p in $cat.Procedures) { $proc += [pscustomobject]@{ Name = $p.Name; Type = $p.Type; Command = $p.Command } }
$proc | Export-Csv "$out\adox_procedures.csv" -NoTypeInformation -Encoding UTF8
"procedures: $($proc.Count)"

# ---- row counts
$rows = @()
foreach ($t in $tblMeta) {
    if ($t.Table -like "MSys*") { continue }
    $cnt = $null; $err = ""
    try {
        $rs = New-Object -ComObject ADODB.Recordset
        $rs.Open("SELECT COUNT(*) AS n FROM [$($t.Table)]", $conn, 3, 1)
        $rs.MoveFirst(); $cnt = $rs.Fields.Item("n").Value; $rs.Close()
    } catch { $cnt = -1; $err = $_.Exception.Message }
    $rows += [pscustomobject]@{ Table = $t.Table; Rows = $cnt; Columns = $t.ColumnCount; Indexes = $t.IndexCount; Error = $err }
}
$rows | Export-Csv "$out\rowcounts.csv" -NoTypeInformation -Encoding UTF8
"--- user tables with rows ---"
$rows | Where-Object { $_.Rows -gt 0 } | Sort-Object { -[int]$_.Rows } | ForEach-Object { "  {0,-26} rows={1,-6} cols={2} idx={3}" -f $_.Table, $_.Rows, $_.Columns, $_.Indexes }
"TOTAL rows (user tables): " + (($rows | Measure-Object -Property Rows -Sum).Sum)
"empty user tables: " + (@($rows | Where-Object { $_.Rows -eq 0 }).Count)
"failed counts: " + (@($rows | Where-Object { $_.Error -ne "" }).Count)

$conn.Close()
"phase A complete"
