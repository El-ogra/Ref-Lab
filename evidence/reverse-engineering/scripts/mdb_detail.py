import csv
import io
import json
import os

BASE = r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\mdb"
EXP = os.path.join(BASE, "exports", "tables")


def load(t):
    with io.open(os.path.join(EXP, t + ".json"), encoding="utf-8") as fh:
        return json.load(fh)


def show(t, limit=None, redact=True):
    d = load(t)
    print("=== %s (%d rows, %d cols) ===" % (t, len(d["rows"]), len(d["columns"])))
    for i, r in enumerate(d["rows"]):
        if limit and i >= limit:
            print("   ... (%d more rows)" % (len(d["rows"]) - limit))
            break
        out = {}
        for k, v in r.items():
            if v in (None, ""):
                continue
            if redact and k in ("patientword", "SecWord", "accuntpw", "SMSbakey"):
                v = "<REDACTED>"
            out[k] = v
        print("   " + json.dumps(out, ensure_ascii=False)[:900])
    print()


for t in ("patients", "MyLabSetting", "history", "reportview", "Paste Errors",
          "testtemp", "smslist", "smsgroups", "Labbranch", "loggroup", "comments",
          "constanttable", "ratios", "PTC", "testlearn", "titless", "daytip"):
    try:
        show(t, limit=6)
    except FileNotFoundError:
        print("missing export for", t)

print("=" * 78)
print("INDEX INVENTORY (72 indexes across the 103 user tables)")
print("=" * 78)
with io.open(os.path.join(BASE, "adox_indexes.csv"), newline="", encoding="utf-8-sig") as fh:
    idx = list(csv.DictReader(fh))
by_table = {}
for r in idx:
    by_table.setdefault(r["Table"], []).append(r)
for t in sorted(by_table):
    rs = by_table[t]
    prim = sum(1 for r in rs if r["Primary"].upper() == "TRUE")
    uniq = sum(1 for r in rs if r["Unique"].upper() == "TRUE")
    print("  %-22s indexes=%-3d primary=%-2d unique=%-2d  %s" %
          (t, len(rs), prim, uniq,
           "; ".join("%s[%s]%s" % (r["Index"], r["Columns"][:44],
                                   " PRIMARY" if r["Primary"].upper() == "TRUE" else "")
                     for r in rs[:4])))
