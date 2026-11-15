# Output directory — `real lab system.exe` static RE

Read-only static reverse engineering of
`F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
(SHA-256 `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0`).

**Deliverables**

| File | Purpose |
|---|---|
| `exe_findings.md` | **main report** — identity, PE, resources, imports, strings, VB6 structure, DB cross-reference, limitations |
| `tool_log.md` | every tool invoked, version, success/failure |
| `exe_inventory.json` | machine-readable identity + PE + resources + version + imports + VB6 + DB + printing + auth |
| `strings_classified.json` | 101,812 classified strings (offsets, encodings, categories) + 15,143-entry unique-text index |
| `resources.json` | resource tree with file offsets and SHA-256 per payload |
| `imports.json` | full import table (231 functions) |
| `sql_inventory.json` | 2,375 unique SQL strings + 127 table identifiers + 508 qualified column pairs |
| `db_xref.json` | cross-reference against the supplied `Patients.mdb` table list |
| `vb_objects.json` | 210 VB6 project objects (name, member count, ObjectInfo pointer, object type) |
| `vb_members.json` | 11,280 control/member descriptor records in 247 tables |
| `vb_code_entries.json` | 349 compiled-code entry addresses (all VB6-prologue verified) |
| `version_info.json` | VS_VERSIONINFO strings |
| `pe_identity.json`, `pe_dirs.json` | raw PE header / sections / data directories |
| `strings_raw.json` | unfiltered 369,877-string sweep (pre-classification) |
| `resources/` | the 12 extracted resource payloads |
| `*.py` | the analysis scripts (see below) |

## Script → output map

| Script | Produces |
|---|---|
| `pe_parse.py` | `pe_identity.json` |
| `dirs.py` | `pe_dirs.json` |
| `ver_parse.py` | `version_info.json` |
| `res_fix.py` | `resources.json` + `resources/` (supersedes the earlier `res_parse.py`, which mistakenly used resource RVAs as file offsets) |
| `str_raw.py` | `strings_raw.json` |
| `classify2.py` | `strings_classified.json` (supersedes `classify.py`) |
| `sqlx.py` | `sql_inventory.json` |
| `xref.py` | `db_xref.json` |
| `vb_hdr.py`, `vb_find.py` | VB6 `VB5!` header decode |
| `ep.py` | entry-point disassembly (Capstone) |
| `pod.py` | `vb_objects.json` + `vb_code_entries.json` |
| `members.py` | `vb_members.json` |
| `pool.py`, `poolrange.py`, `density.py` | VB6 literal-pool location (partial — see `exe_findings.md` §6.5) |
| `inv.py`, `inv2.py`, `inv3.py` | `exe_inventory.json` |
| `icons.py` | `RT_GROUP_ICON` structure |
| `rep.py`, `sum2.py`, `cat2.py`, `chk*.py`, `final_stats.py`, `chk2.py`, `verify_code.py`, `dump*.py`, `find1.py`, `obj.py`, `objtab.py`, `cnt*.py`, `paste.py`, `pdf.py`, `sqloff.py`, `f10.py`, `reg2.py`, `str_*.py`, `calib*.py`, `diag*.py` | investigation scratch files (evidence of specific checks cited in the report) |

The target was never executed, debugged or modified; nothing under `F:\` was written.
