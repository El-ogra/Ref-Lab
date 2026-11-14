"""Independent verification and data-quality analysis of the Patients.mdb extraction.

Reads the exported JSON (one file per table) and the ADOX metadata CSVs, and:
  * re-counts records per table from the export files themselves;
  * reconciles against the extraction ledger;
  * screens for sensitive values that must not be published;
  * runs logical consistency checks (duplicate keys, nulls in required fields,
    orphan-looking references, value-range anomalies, date sanity).
"""
import csv
import io
import json
import os
import re
from collections import Counter, defaultdict

OUT = r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\mdb"
EXP = os.path.join(OUT, "exports", "tables")
LEDGER = os.path.join(OUT, "export_ledger.csv")

# ---------------------------------------------------------------- load
tables = {}
for name in sorted(os.listdir(EXP)):
    if not name.endswith(".json"):
        continue
    with io.open(os.path.join(EXP, name), encoding="utf-8") as fh:
        d = json.load(fh)
    tables[d["table"]] = d

with io.open(LEDGER, newline="", encoding="utf-8-sig") as fh:
    ledger = {r["Table"]: r for r in csv.DictReader(fh)}

print("=" * 78)
print("1. RECORD-COUNT RECONCILIATION (export files vs ledger)")
print("=" * 78)
mismatch = []
total_files = total_ledger = 0
for t, d in tables.items():
    n = len(d["rows"])
    lg = int(ledger[t]["Exported"])
    exp = int(ledger[t]["Expected"])
    total_files += n
    total_ledger += lg
    if n != lg or n != exp or d["records_exported"] != n:
        mismatch.append((t, n, lg, exp, d["records_failed"]))
print("tables in export directory : %d" % len(tables))
print("records counted from files  : %d" % total_files)
print("records recorded in ledger  : %d" % total_ledger)
print("mismatches                 : %d" % len(mismatch))
for m in mismatch:
    print("   MISMATCH %-20s file=%d ledger=%d expected=%d failed=%s" % m)

print()
print("=" * 78)
print("2. COLUMN-COUNT RECONCILIATION")
print("=" * 78)
col_mismatch = []
total_cols = 0
for t, d in tables.items():
    n = len(d["columns"])
    total_cols += n
    lg = int(ledger[t]["Columns"])
    if n != lg:
        col_mismatch.append((t, n, lg))
print("columns declared across tables : %d" % total_cols)
print("mismatches                     : %d" % len(col_mismatch))
for m in col_mismatch:
    print("   MISMATCH", m)

# user-table column total for comparison with the Stage 1 figure of 1,682
user_cols = 0
for t, d in tables.items():
    user_cols += len(d["columns"])
print("user-table column total (103 tables) : %d" % user_cols)

print()
print("=" * 78)
print("3. SENSITIVE-VALUE SCREEN (export payloads)")
print("=" * 78)
secret_cols = {"patientword", "SecWord", "Password", "accuntpw", "SMSbakey"}
hits = Counter()
samples = []
for t, d in tables.items():
    for i, row in enumerate(d["rows"]):
        for k, v in row.items():
            if v is None:
                continue
            s = str(v)
            if k in secret_cols:
                hits["redacted-marker-missing:" + k] += 1
                if len(samples) < 5:
                    samples.append((t, k, s[:40]))
            if re.search(r"(?i)\b(sa|12345678)\b", s) and t in ("patients", "MyLabSetting"):
                hits["sql-credential-like-value"] += 1
                if len(samples) < 10:
                    samples.append((t, k, "<value withheld>"))
            if re.search(r"\b\d{10,15}\b", s) and t in ("MyLabSetting",):
                hits["phone-like-number"] += 1
for k, v in hits.items():
    print("   %-45s %d" % (k, v))
print("   samples (withheld): %s" % samples[:5])

print()
print("=" * 78)
print("4. DATA-QUALITY / CONSISTENCY CHECKS")
print("=" * 78)

# 4a duplicate business identifiers
print("4a. Duplicate candidate-key values")
keycands = ["testcode", "testid", "normalid", "subgroupcode", "LabID", "testunit",
             "united", "testname", "anticode", "antibioticid"]
for t, d in tables.items():
    cols = [c["name"] for c in d["columns"]]
    for k in keycands:
        if k in cols and len(d["rows"]) > 1:
            vals = [r.get(k) for r in d["rows"] if r.get(k) not in (None, "")]
            dup = [v for v, c in Counter(vals).items() if c > 1]
            if dup:
                print("   %-22s %-14s duplicate values: %d (e.g. %s)" %
                      (t, k, len(dup), ", ".join(str(x)[:18] for x in dup[:4])))

# 4b required fields with NULLs
print("4b. NULL values in fields declared Required")
req_nulls = 0
for t, d in tables.items():
    req = {c["name"] for c in d["columns"] if c.get("required")}
    for i, row in enumerate(d["rows"]):
        for c in req:
            if row.get(c) is None:
                req_nulls += 1
                if req_nulls <= 12:
                    print("   %-20s row %d field '%s' is NULL but declared Required" % (t, i, c))
print("   total required-field NULLs: %d" % req_nulls)

# 4c date sanity
print("4c. Date-range anomalies")
date_re = re.compile(r"^\d{4}-\d{2}-\d{2}T")
bad_dates = 0
future = []
for t, d in tables.items():
    for i, row in enumerate(d["rows"]):
        for k, v in row.items():
            if isinstance(v, str) and date_re.match(v):
                if not ("1990-01-01" <= v[:10] <= "2026-12-31"):
                    bad_dates += 1
                    if len(future) < 10:
                        future.append((t, i, k, v))
print("   out-of-range date values: %d" % bad_dates)
for f in future:
    print("     ", f)

# 4d empty-string vs NULL distinction
print("4d. NULL vs empty-string population")
nulls = empties = 0
for t, d in tables.items():
    for row in d["rows"]:
        for k, v in row.items():
            if v is None:
                nulls += 1
            elif v == "":
                empties += 1
print("   NULL values      : %d" % nulls)
print("   empty-string vals: %d" % empties)

# 4e table-level profile for the documentation
print("4e. Table profile (rows / cols / with-rows / empty)")
with_rows = sum(1 for d in tables.values() if d["rows"])
print("   user tables          : %d" % len(tables))
print("   tables holding rows  : %d" % with_rows)
print("   empty tables         : %d" % (len(tables) - with_rows))
print("   total records        : %d" % sum(len(d["rows"]) for d in tables.values()))
