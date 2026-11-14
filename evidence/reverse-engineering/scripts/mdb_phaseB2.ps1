# Phase B v2 — complete extraction with a declared, auditable redaction policy.
$ErrorActionPreference = "Stop"
$mdb = "F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb"
$out = "C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\mdb"
$exp = "$out\exports\tables"
$expFull = "$out\exports_full\tables"      # restricted, unpublished copy
New-Item -ItemType Directory -Force -Path $exp, $expFull | Out-Null
$mdbHash = (Get-FileHash -LiteralPath $mdb -Algorithm SHA256).Hash

# --- credential columns: never published
$credCols = @{ 'patientword'='PASSWORD'; 'SecWord'='PASSWORD'; 'accuntpw'='SECRET';
               'SMSbakey'='SECRET'; 'accuntID'='ACCOUNT-ID'; 'strip3'='SQL-ACCOUNT'; 'strip4'='SQL-PASSWORD' }
# --- personal identifiers: pseudonymised in the published copy
$pseudoCols = @{ 'patientid'='PID'; 'column1'='REF' }
# --- personal phone numbers: masked in the published copy
$maskCols  = @{ 'smsphone'='PHONE'; 'phoneNo1'='PHONE'; 'phoneNo2'='PHONE'; 'phoneNo3'='PHONE';
               'phoneNo4'='PHONE'; 'phoneNo5'='PHONE'; 'phoneNo6'='PHONE' }

$conn = New-Object -ComObject ADODB.Connection
$conn.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$mdb;Mode=Read;", [string]::Empty, [string]::Empty)
$cat = New-Object -ComObject ADOX.Catalog
$cat.ActiveConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$mdb;Mode=Read;"

$colMeta = @{}
foreach ($t in $cat.Tables) {
    if ($t.Name -like "MSys*") { continue }
    $cm = @()
    foreach ($c in $t.Columns) {
        $cm += [pscustomobject]@{ name = $c.Name; position = $c.Position; adox_type = $c.Type; size = $c.Size
            required = $c.Required; allow_zero_length = $c.AllowZeroLength; default = $c.Default
            validation_rule = $c.ValidationRule; validation_text = $c.ValidationText; format = $c.Format
            input_mask = $c.InputMask; attributes = $c.Attributes
            numeric_precision = $c.NumericPrecision; numeric_scale = $c.NumericScale
            auto_increment = $c.AutoIncrement; owned = $c.Owned }
    }
    $colMeta[$t.Name] = $cm
}

$ledger = @(); $totalExp = 0; $totalFail = 0
$pseudoMap = @{}
foreach ($t in ($cat.Tables | Sort-Object Name)) {
    if ($t.Name -like "MSys*") { continue }
    $name = $t.Name
    $safe = ($name -replace '[\\/:*?"<>|]', '_')
    $file = "$exp\$safe.json"; $fileFull = "$expFull\$safe.json"
    $expected = 0; $exported = 0; $failed = 0; $err = ""
    $data = @(); $dataFull = @()
    try {
        $rs = New-Object -ComObject ADODB.Recordset
        $rs.CursorLocation = 3
        $rs.Open("SELECT * FROM [$name]", $conn, 3, 1)
        $expected = $rs.RecordCount
        $flds = @(); for ($i = 0; $i -lt $rs.Fields.Count; $i++) { $flds += $rs.Fields.Item($i) }
        $meta = @()
        for ($i = 0; $i -lt $flds.Count; $i++) {
            $m = $colMeta[$name] | Where-Object { $_.name -eq $flds[$i].Name } | Select-Object -First 1
            $meta += [pscustomobject]@{ name = $flds[$i].Name; ado_type = $flds[$i].Type; size = $flds[$i].Size
                adox_type = $(if ($m) { $m.adox_type } else { $null })
                required = $(if ($m) { $m.required } else { $null })
                format = $(if ($m) { $m.format } else { $null }) }
        }
        while (-not $rs.EOF) {
            $o = [ordered]@{}; $oF = [ordered]@{}
            for ($i = 0; $i -lt $flds.Count; $i++) {
                $f = $flds[$i]; $v = $f.Value; $nm = $f.Name
                # ---- published value
                if ($null -eq $v) { $pv = $null }
                elseif ($credCols.ContainsKey($nm)) { $pv = $(if (([string]$v).Length -gt 0) { "<REDACTED:$($credCols[$nm])>" } else { "" }) }
                elseif ($pseudoCols.ContainsKey($nm)) {
                    $k = "$nm|$v"; if (-not $pseudoMap.ContainsKey($k)) { $pseudoMap[$k] = "<$($pseudoCols[$nm])-{0:D4}>" -f ($pseudoMap.Count + 1) }
                    $pv = $pseudoMap[$k]
                }
                elseif ($maskCols.ContainsKey($nm)) { $pv = $(if (([string]$v).Length -gt 0) { "<MASKED:$($maskCols[$nm])>" } else { "" }) }
                elseif ($v -is [DateTime]) { $pv = $v.ToString("yyyy-MM-ddTHH:mm:ss.fff") }
                elseif ($v -is [byte[]]) { $pv = [System.Convert]::ToBase64String($v) }
                elseif ($v -is [double] -or $v -is [decimal] -or $v -is [int] -or $v -is [long] -or $v -is [single] -or $v -is [int16] -or $v -is [byte]) { $pv = $v }
                elseif ($v -is [bool]) { $pv = [bool]$v }
                else { $pv = [string]$v }
                $o[$nm] = $pv
                # ---- restricted value (unredacted)
                if ($null -eq $v) { $oF[$nm] = $null }
                elseif ($v -is [DateTime]) { $oF[$nm] = $v.ToString("yyyy-MM-ddTHH:mm:ss.fff") }
                elseif ($v -is [byte[]]) { $oF[$nm] = [System.Convert]::ToBase64String($v) }
                elseif ($v -is [double] -or $v -is [decimal] -or $v -is [int] -or $v -is [long] -or $v -is [single] -or $v -is [int16] -or $v -is [byte]) { $oF[$nm] = $v }
                elseif ($v -is [bool]) { $oF[$nm] = [bool]$v }
                else { $oF[$nm] = [string]$v }
            }
            $data += [pscustomobject]$o; $dataFull += [pscustomobject]$oF
            $exported++; $rs.MoveNext()
        }
        $rs.Close()
    } catch { $failed = $expected; $err = $_.Exception.Message }

    $doc = [ordered]@{ table = $name; source_file = $mdb; source_sha256 = $mdbHash
        extracted_with = "Microsoft.ACE.OLEDB.12.0 (read-only) via ADO; ADOX for metadata"
        records_expected = $expected; records_exported = $exported; records_failed = $failed; error = $err
        columns = $colMeta[$name]; rows = $data }
    [System.IO.File]::WriteAllText($file, ($doc | ConvertTo-Json -Depth 8), (New-Object System.Text.UTF8Encoding($false)))
    $docFull = [ordered]@{ table = $name; source_sha256 = $mdbHash; classification = "RESTRICTED - contains unredacted credentials and patient identifiers"
        records_expected = $expected; records_exported = $exported; rows = $dataFull }
    [System.IO.File]::WriteAllText($fileFull, ($docFull | ConvertTo-Json -Depth 8), (New-Object System.Text.UTF8Encoding($false)))
    $ledger += [pscustomobject]@{
        Table = $name; Expected = $expected; Exported = $exported; Failed = $failed
        Columns = $colMeta[$name].Count; ExportFile = $file.Replace($out + '\', '')
        ExportSHA256 = (Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash
        ExportBytes = (Get-Item -LiteralPath $file).Length
        Status = $(if ($failed -gt 0) { "PARTIAL" } else { "VERIFIED" }); Error = $err }
    $totalExp += $exported; $totalFail += $failed
}
$ledger | Export-Csv "$out\export_ledger.csv" -NoTypeInformation -Encoding UTF8
"tables exported : $($ledger.Count)"
"records expected: " + (($ledger | Measure-Object -Property Expected -Sum).Sum)
"records exported: $totalExp"
"records failed  : $totalFail"
"partial tables  : " + (@($ledger | Where-Object { $_.Failed -gt 0 }).Count)
$pseudoMap.GetEnumerator() | ForEach-Object { "{0} => {1}" -f $_.Key, $_.Value } | Out-File "$out\pseudonym_map.txt" -Encoding UTF8
"pseudonyms created: " + $pseudoMap.Count
$conn.Close()
"phase B v2 complete"
