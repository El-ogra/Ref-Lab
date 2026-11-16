#!/usr/bin/env python3
"""Generate the two large schema deliverables from parsed_schema.json.
Reads only evidence-derived JSON in this _work folder; writes only into the
assigned 04_DATABASE_FORENSICS folder.
"""
import json, os, collections, re

WORK = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.dirname(WORK)
D = json.load(open(os.path.join(WORK, "parsed_schema.json"), encoding="utf-8"))
V1 = {t["table"].lower(): t for t in D["v1_patients"]["tables"]}
V19 = {t["table"].lower(): t for t in D["v2019_patients"]["tables"]}

# ---- domain classification -------------------------------------------------
PROFILE = set("""ABO Abuse BILH BloodGases BONEMAR cardiac cbc CoagFactors crossmat CRP CSFT
cultureanti cultures culturesec Drugs Electrolytes ESR Fertlity GLUCOSETOLERANCE hba1c
Helicobacter hepatitis Hgbectrophoresis hormones Immunoglobulin IMMUNOLOGY iron kidney
Lipids liver Pancreas postcoitalt pregnancy Proelectrophoresis prostaticx pt ratios
renalfailure semen stone stool sugar Suprarenal tasgel TORCH trace TUBERCULINT Tumors
urine Vitamins widal widal2 ZN""".split())

DOMAINS = [
    ("Patients, orders and result workflow (core transactional)", [
        "patientinfo", "patienttest", "patienttestgroup", "testtemp", "patienttotal",
        "barcodes", "history", "wasl", "reportview", "Paste Errors", "phone"]),
    ("Operators, access and audit", [
        "patients", "loggroup", "noteic"]),
    ("Test catalogue, reference knowledge and lookups", [
        "testtable", "normalrange", "subgrouptable", "customgrouptable", "customtesttable",
        "testunits", "converts", "abbreviations", "comments", "constanttable", "PTC",
        "titless", "testname", "testlearn", "insulin", "antibio", "antibiotic"]),
    ("Billing, finance and referring parties", [
        "payment", "Gehats", "sharekat", "sharekathesab", "pricelist", "pricemenu",
        "shopping", "sarfeda3"]),
    ("Laboratory configuration and infrastructure", [
        "MyLabSetting", "Labbranch", "instrument", "daytip", "pictures"]),
    ("Messaging / SMS (new in v1.605)", [
        "SMSSender", "SMStemplate", "smslist", "smsgroups"]),
]
ASSIGNED = set(n for _, ns in DOMAINS for n in ns) | PROFILE
OTHER = sorted([t["table"] for t in D["v2019_patients"]["tables"]
                if t["table"] not in ASSIGNED], key=str.lower)

def domain_of(name):
    for dom, ns in DOMAINS:
        if name in ns:
            return dom
    if name in PROFILE:
        return "Per-test profile result tables (one wide table per test panel)"
    return "Other / ancillary (semantic assignment UNVERIFIED)"

def col_line(c):
    t = c["type"] if c["type"] else "(untyped)"
    return f"`{c['name']}` {t}{' NOT NULL' if c['not_null'] else ''}"

# ---- Complete_Database_Schema.md -------------------------------------------
L = []
L.append("# Complete Database Schema — REAL LAB SYSTEM v1.605 (V-CD-2019)\n")
L.append("## Evidence basis and scope\n")
L.append("This document reconstructs the **shipped `Patients.mdb`** from the authoritative")
L.append("MDB-tools schema dump `v2019-Patients.mdb.sql` (47,464 B), cross-checked against")
L.append("`schema-Patients.sql` / `v1-Patients.mdb.sql` (42,934 B, the v1.00 schema) and the")
L.append("restated counts in `Version_Identity_and_Scope.md` §3.3 and")
L.append("`Data_Model_and_Workflow_Entities_V2.md` §2.1. **No live `.mdb` was opened.**\n")
L.append("| Property | Value | Source |")
L.append("|---|---:|---|")
L.append("| Tables | **103** | `v2019-Patients.mdb.sql` — 103 `CREATE TABLE` statements |")
L.append("| Columns | **1,682** | parsed column lines in `v2019-Patients.mdb.sql` |")
L.append("| Declared `NOT NULL` columns | 132 | `NOT NULL` tokens in `v2019-Patients.mdb.sql` |")
L.append("| Declared PRIMARY / FOREIGN / UNIQUE / CHECK / DEFAULT constraints | 0 | keyword search of all DDL evidence files |")
L.append("| Declared indexes (`CREATE INDEX`) | 0 | keyword search of `v2019-Patients.mdb.sql`, `ddl-all.txt`, `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` |")
L.append("")
L.append("**Physical Jet 4 type families** in the 1,682 shipped columns (parsed from")
L.append("`v2019-Patients.mdb.sql`):\n")
L.append("| Jet type family | Columns |")
L.append("|---|---:|")
fams = collections.Counter()
for t in D["v2019_patients"]["tables"]:
    for c in t["columns"]:
        fams[re.sub(r"\s*\(.*", "", c["type"]).strip()] += 1
for k, v in fams.most_common():
    L.append(f"| `{k}` | {v} |")
L.append("")
L.append("> **CONFLICTED (prior reports).** `00-baseline-summary.md` states \"103 tables ·")
L.append("> 1,617 columns\" and \"+10 tables · +90 columns\". The primary dump")
L.append("> `v2019-Patients.mdb.sql` yields **1,682** columns and a direct v1→v2019 diff of")
L.append("> **+155**; `Version_Identity_and_Scope.md` §3.3 and")
L.append("> `Data_Model_and_Workflow_Entities_V2.md` §2.1 independently restate +155. This")
L.append("> document follows the primary dump. `Business_Rules_V2.md` /")
L.append("> `Functional_Specification_V2.md` describe a **17-new-table** set from the embedded")
L.append("> DDL; the shipped-dump diff is **10** new tables (see `Tables_And_Columns.md`).\n")
L.append("## Keys and constraints\n")
L.append("The dump declares **no** `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`, `CHECK`, `DEFAULT`")
L.append("or `CREATE INDEX` clause. Candidate keys below are therefore **INFERRED from naming")
L.append("and SQL usage, not declared**: `patientinfo.patientid`, `patienttest.patientid`,")
L.append("`testtable.testcode`, `normalrange.normalid`, `patients.patientusid`,")
L.append("`Labbranch.branchid`. Jet 4 does not enforce FKs; the application enforces")
L.append("relationships through SQL joins and `SHAPE … RELATE` commands")
L.append("(see `Relationships.md`, `Constraints_And_Indexes.md`).\n")
L.append("## Tables by domain\n")
L.append("Each entry lists **every column in the shipped dump** with its Jet type;")
L.append("`NOT NULL` is shown only where the dump declares it.\n")

order = [d[0] for d in DOMAINS] + [
    "Per-test profile result tables (one wide table per test panel)",
    "Other / ancillary (semantic assignment UNVERIFIED)"]
for dom in order:
    names = [t["table"] for t in D["v2019_patients"]["tables"] if domain_of(t["table"]) == dom]
    names.sort(key=str.lower)
    L.append(f"### {dom} ({len(names)} tables)\n")
    for n in names:
        t = V19[n.lower()]
        new = " — **NEW in v1.605**" if n.lower() not in V1 else ""
        L.append(f"#### `{n}` — {t['n_cols']} columns{new}\n")
        L.append("; ".join(col_line(c) for c in t["columns"]))
        L.append("")
L.append("## Separate store: `Branch_Data.mdb` (schema only)\n")
L.append("Reconstructed from `schema-Branch_Data.sql` (= `schema.sql`, 26,262 B):")
L.append("**49 tables / 969 columns**, 0 rows. It is a per-branch template, not a populated")
L.append("clinical store (`db-summary.csv`; `Data_Model_and_Workflow_Entities_V2.md` §2.1).")
L.append("`Version_Identity_and_Scope.md` §3.3 records that the `bin\\` copy shipped with 2019")
L.append("is byte-identical to v1.00, while a second `System\\` copy (966,656 B) is 9 columns")
L.append("narrower in `patienttest`. Two distinct templates therefore coexist — marked")
L.append("CONFLICTED until the files themselves are available.\n")
L.append("## Separate store: `data.mdb`\n")
L.append("`v1-data.mdb.sql` and `v2019-data.mdb.sql` (1,420 B each): **3 tables / 34 columns**,")
L.append("0 rows — `history`, `patientinfo`, `wasl` (`rowcounts__[analysis_BATCH-02_db_data].csv`).")
L.append("")
open(os.path.join(OUT, "Complete_Database_Schema.md"), "w", encoding="utf-8").write("\n".join(L))
print("wrote Complete_Database_Schema.md", sum(1 for _ in L), "lines")

# ---- Tables_And_Columns.md -------------------------------------------------
M = []
M.append("# Tables and Columns — REAL LAB SYSTEM v1.605, with v1.00 → v1.605 delta\n")
M.append("## Method and confidence\n")
M.append("Full column lists are parsed from `v2019-Patients.mdb.sql` (103 tables / 1,682 columns)")
M.append("and `v1-Patients.mdb.sql` (93 tables / 1,527 columns). The delta is a direct set")
M.append("comparison of those two dumps — **VERIFIED**. `added-columns.csv`,")
M.append("`created-tables.csv` and `column-evolution.csv` are reconciled below; they derive")
M.append("from the *embedded runtime migration DDL* and are not a full-schema comparison.\n")
M.append("## Cross-version delta (direct dump comparison)\n")
M.append("| Metric | v1.00 (`v1-Patients.mdb.sql`) | v1.605 (`v2019-Patients.mdb.sql`) | Delta |")
M.append("|---|---:|---:|---:|")
M.append("| Tables | 93 | 103 | **+10** |")
M.append("| Columns | 1,527 | 1,682 | **+155** |")
M.append("| Tables removed | — | — | 0 |")
M.append("")
newtabs = sorted([t for t in D["v2019_patients"]["tables"] if t["table"].lower() not in V1],
                 key=lambda t: t["table"].lower())
mcols = sum(t["n_cols"] for t in newtabs)
addrows = []
for k in sorted(set(V1) & set(V19)):
    c1 = {c["name"].lower(): c for c in V1[k]["columns"]}
    c2 = {c["name"].lower(): c for c in V19[k]["columns"]}
    added = [c for n, c in c2.items() if n not in c1]
    removed = [c for n, c in c1.items() if n not in c2]
    if added or removed:
        addrows.append((V19[k]["table"], added, removed))
sc = sum(len(a) for _, a, _ in addrows)
M.append(f"**+155 = {mcols} columns in 10 new tables + {sc} columns added to 12 existing tables.**")
M.append(f"(Removed columns: {sum(len(r) for _, _, r in addrows)}.)\n")
M.append("### The 10 new tables\n")
M.append("| Table | Cols | Columns (name : type) |")
M.append("|---|---:|---|")
for t in newtabs:
    M.append(f"| `{t['table']}` | {t['n_cols']} | " +
             "; ".join(col_line(c) for c in t["columns"]) + " |")
M.append("")
M.append("### Columns added to existing tables (28)\n")
M.append("| Table | Added column : type |")
M.append("|---|---|")
for name, added, removed in addrows:
    for c in added:
        M.append(f"| `{name}` | {col_line(c)} |")
M.append("")
M.append("### Columns removed from existing tables (0 in the direct dump diff)\n")
M.append("None. (`column-evolution.csv` lists many \"removed\" columns, but that file compares")
M.append("the v1 Jet dump with the DDL-statement view, not the two full dumps — see below.)\n")
M.append("### Reconciliation with the migration-DDL evidence files\n")
M.append("- `created-tables.csv` lists runtime-created tables (`FMF`, `Helicobacter`, `LBC`,")
M.append("  `SMSSender`, …); `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` holds 268 DDL")
M.append("  literals (27 `CREATE TABLE`, 241 `ALTER TABLE` = 228 ADD + 13 ALTER COLUMN).")
M.append("- `added-columns.csv` lists runtime `ADD COLUMN` targets, e.g.")
M.append("  `BloodGases.pO2AA1`, `.ctCO2`, `.Aniongap`, `PTC.ConstID`, `Patients.branche`,")
M.append("  `Patients.grant13`…`grant17`.")
M.append("- **CONFLICTED:** `column-evolution.csv` gives `bloodgases` 8 \"v2019 columns\"")
M.append("  (the ALTER-added set) and marks its 10 v1 columns as \"removed\"; the shipped dump")
M.append("  still has those 10 columns. `Version_Identity_and_Scope.md` §3.5 explains why:")
M.append("  61 of the 268 DDL changes never reached the shipped DB (23 SQL-Server-only syntax,")
M.append("  38 Access-applicable but unexecuted). This document therefore treats")
M.append("  `column-evolution.csv` as evidence of the *migration engine's intent*, not of the")
M.append("  shipped schema.\n")
M.append("## Table-by-table column listing (shipped v1.605)\n")
M.append("Format: `column` Type `NOT NULL` where declared. Order is the order in the dump.")
M.append("`NEW` marks a table absent from `v1-Patients.mdb.sql`.\n")
for t in sorted(D["v2019_patients"]["tables"], key=lambda x: x["table"].lower()):
    new = " **NEW in v1.605**" if t["table"].lower() not in V1 else ""
    M.append(f"### `{t['table']}` — {t['n_cols']} columns{new}\n")
    M.append("; ".join(col_line(c) for c in t["columns"]))
    M.append("")
open(os.path.join(OUT, "Tables_And_Columns.md"), "w", encoding="utf-8").write("\n".join(M))
print("wrote Tables_And_Columns.md", sum(1 for _ in M), "lines")
print("new table columns", mcols, "shared added", sc, "total", mcols + sc)
