#!/usr/bin/env python3
"""Parse MDB-tools schema dumps into structured JSON for forensic analysis.
Read-only against evidence; writes only into this _work folder.
"""
import json, re, os, sys

BASE = r"D:\Collective work"
OUT = os.path.dirname(os.path.abspath(__file__))

FILES = {
    "v2019_patients": "v2019-Patients.mdb.sql",
    "v1_patients": "v1-Patients.mdb.sql",
    "schema_patients": "schema-Patients.sql",
    "branch_data": "schema-Branch_Data.sql",
    "schema_branch": "schema.sql",
    "data_v1": "v1-data.mdb.sql",
    "data_v2019": "v2019-data.mdb.sql",
    "backup_b02": "schema__[analysis_BATCH-02_db_Patients_backup].sql",
    "patients_b02": "schema__[analysis_BATCH-02_db_Patients].sql",
    "data_b02": "schema__[analysis_BATCH-02_db_data].sql",
}

# matches: CREATE TABLE [name] ( ... );
TABLE_RE = re.compile(
    r"CREATE\s+TABLE\s+(?:\[([^\]]+)\]|([^\s(]+))\s*\((.*?)\)\s*;",
    re.IGNORECASE | re.DOTALL,
)
# column line: [name]  Type (N) NOT NULL,
COL_RE = re.compile(r"^\s*\[?([^\]\s]+)\]?\s+(.+?)\s*$", re.MULTILINE)


def parse(path):
    with open(path, "r", encoding="utf-8", errors="replace") as fh:
        text = fh.read()
    tables = []
    for m in TABLE_RE.finditer(text):
        name = (m.group(1) or m.group(2)).strip()
        body = m.group(3)
        cols = []
        for line in body.splitlines():
            line = line.strip().rstrip(",").strip()
            if not line or line.startswith("--"):
                continue
            cm = COL_RE.match(line)
            if not cm:
                continue
            colname = cm.group(1).strip()
            typ = cm.group(2).strip()
            notnull = bool(re.search(r"\bNOT\s+NULL\b", typ, re.IGNORECASE))
            # strip NOT NULL from type for cleanliness
            typ_clean = re.sub(r"\bNOT\s+NULL\b", "", typ, flags=re.IGNORECASE).strip().rstrip(",").strip()
            cols.append({"name": colname, "type": typ_clean, "not_null": notnull, "raw": line})
        tables.append({"table": name, "n_cols": len(cols), "columns": cols})
    return tables, text


def main():
    result = {}
    for key, fn in FILES.items():
        path = os.path.join(BASE, fn)
        if not os.path.exists(path):
            print("MISSING", fn)
            continue
        tables, text = parse(path)
        result[key] = {
            "file": fn,
            "bytes": os.path.getsize(path),
            "n_tables": len(tables),
            "n_columns": sum(t["n_cols"] for t in tables),
            "tables": tables,
        }
        print(f"{key:16s} {fn:55s} tables={len(tables):3d} cols={result[key]['n_columns']:5d}")
    outpath = os.path.join(OUT, "parsed_schema.json")
    with open(outpath, "w", encoding="utf-8") as fh:
        json.dump(result, fh, indent=1, ensure_ascii=False)
    print("WROTE", outpath)


if __name__ == "__main__":
    main()
