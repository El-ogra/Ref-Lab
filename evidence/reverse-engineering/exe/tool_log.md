# Tool log — `real lab system.exe` static RE (read-only)

Target: `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
Workdir for all outputs: `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\`
Nothing under `F:\` was written, executed, or modified. The target was never launched.

| # | Tool | Version | Command / API | Result | Notes |
|---|------|---------|----------------|--------|-------|
| 1 | PowerShell `Get-Item` | 7.x | `Get-Item <target>` | **SUCCESS** | Size 76,292,096 bytes — matches expectation |
| 2 | PowerShell `Get-FileHash -Algorithm SHA256` | 7.x | `Get-FileHash -Algorithm SHA256 <target>` | **SUCCESS** | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` — matches expectation exactly. Identity verified before any analysis. |
| 3 | `rea` CLI | 6.7.0 | `rea --help` | **SUCCESS** | 70+ commands enumerated |
| 4 | `rea` CLI | 6.7.0 | `rea doctor` | **PARTIAL** | `environment_healthy:false` overall. Ghidra 12.1.4 @ `C:\Tools\ghidra_12.1.4_PUBLIC` **healthy** (headless OK, JDK 21.0.13 OK). Hopper / IDA / jadx / kuna **missing** (not installed). Requires `JAVA_HOME` set or the Ghidra provider is disabled. |
| 5 | `rea` CLI | 6.7.0 | `rea providers` | **SUCCESS** | Provider capability listing |
| 6 | `rea` CLI | 6.7.0 | `rea inspect-binary-layout <target> --format json` | **FAIL** | Auto-selected `pwntools-elf` provider → `capability_unavailable` (Linux x64 only). Fixed by binding `--provider ghidra`. |
| 7 | `rea` CLI | 6.7.0 | `rea analyze <target> --snapshot ...` (default 330 s timeout) | **FAIL** | `provider_timeout` — "Ghidra did not finish importing and analyzing the target within its 330000 ms startup deadline". Remediation: raise `REA_GHIDRA_STARTUP_TIMEOUT_MS`. |
| 8 | `rea` CLI | 6.7.0 | `rea analyze ... ` with `REA_GHIDRA_STARTUP_TIMEOUT_MS=7200000` + `REA_GHIDRA_DISABLED_ANALYZERS="Aggressive Instruction Finder,Decompiler Parameter ID,Non-Returning Functions - Discovered,Reference ID,Windows x86 PE Exception Handlers,Windows x86 Constant Metadata,Function ID,BSim"` | **FAIL** | Ghidra pre-script aborted: `IllegalArgumentException: Unknown Ghidra analyzer for this program: Reference ID` — the wrapper validates analyzer names against the installed Ghidra. |
| 9 | `rea` CLI | 6.7.0 | same, analyzer list without `Reference ID` | **FAIL** | `Unknown Ghidra analyzer for this program: Windows x86 PE Exception Handlers` |
| 10 | `rea` CLI | 6.7.0 | same, analyzer list = `Aggressive Instruction Finder, Decompiler Parameter ID, Non-Returning Functions - Discovered`; `--provider ghidra` | **RUNNING / see §Ghidra** | Started last; Ghidra headless process observed alive (java PID 11160, CPU >300 s). Result recorded below. |
| 11 | Python | 3.13.1 | `pip install pefile` | **SUCCESS** | pefile 2024.8.26 |
| 12 | Python `pefile` | 2024.8.26 | PE header / section / data-directory / import / resource parse | **SUCCESS** | `pe_identity.json`, `exe_inventory.json` |
| 13 | Python `pefile` | 2024.8.26 | VS_VERSIONINFO / StringFileInfo parse | **SUCCESS** | `version_info.json` |
| 14 | Python `pefile` | 2024.8.26 | Resource directory walk + on-disk extraction to `resources\` | **SUCCESS** | `resources.json`, `resources\*` |
| 15 | Python `re` | stdlib | ASCII (>=4) + UTF-16LE (>=4) string sweep of the whole 76 MB file | **SUCCESS** | 369,877 raw hits → `strings_raw.json` (26 MB) |
| 16 | Python `struct` | stdlib | VB6 `VB5!` header scan + field decode | **SUCCESS** | Header at file offset `0xb8e80` |
| 17 | Python `capstone` | 5.x (pip) | x86 disassembly of entry point RVA `0x7142c` | **SUCCESS** | `push 0x4b8e80 ; call 0x471424` — confirms the VB header address |
| 18 | Python `struct` | stdlib | VB6 PublicObjectDescriptor array parse (48-byte stride, `0x260c04`) | **SUCCESS** | 210 objects with names + member counts + native code VAs → `vb_objects.json` |
| 19 | Python `struct` | stdlib | VB6 member descriptor scan (40-byte stride) over the whole file | **SUCCESS** | 11,280 candidate records / 11,076 in 247 contiguous tables → `vb_members.json` |
| 20 | Python `re` | stdlib | SQL statement/fragment/DDL detection, table-identifier extraction, qualified `table.column` extraction | **SUCCESS** | `sql_inventory.json` (2,375 unique SQL strings, 127 table identifiers, 508 qualified column pairs) |
| 21 | Python `re` | stdlib | Cross-reference of SQL table identifiers against the supplied `Patients.mdb` 103-table list | **SUCCESS** | `db_xref.json` |
| 22 | Python `re` | stdlib | Multi-category string classifier (SQL / path / URL / registry / credential / printer / report / OLE / VB6 name / caption) | **SUCCESS** | `strings_classified.json` (12.2 MB, 101,812 retained entries, 15,143 unique texts) |

### REA MCP server (207 tools, via the Code Mode `execute` runtime)

| # | MCP tool | Result | Notes |
|---|----------|--------|-------|
| 23 | `rea.inspect_pe_resources` | **FAIL — blocked** | `resource_constraint`: *"Selected artifact exceeds 67108864 bytes"*. The target is 76,292,096 bytes, i.e. above the REA MCP default `max_file_bytes` of 64 MiB. The REA MCP resource tool therefore **cannot be used on this target at all** without raising that caller-side budget. PE resources were instead enumerated and extracted with `pefile` (tool #14), which has no such limit. |
| 24 | `rea.open_binary` (`provider_id: ghidra`) | **FAIL — blocked** | `provider_unavailable`. The REA **MCP server process** was started without `JAVA_HOME`, so its Ghidra provider probe resolved `java.exe` from `PATH` as a *relative* command name and the Windows-safe spawn rejected it: `Owned Windows process requires an absolute executable path [java.exe] (Win32 87)`. `JAVA_HOME` cannot be injected into the already-running server process. The **CLI** path works because the shell sets `JAVA_HOME` explicitly. Also reported: Hopper `unsupported_host` (macOS only), IDA `not_configured`, Kuna `not_configured` **and** `target_format_unsupported` (*"pe is not admitted yet"*). |
| 25 | `rea.binary_overview`, `rea.list_procedures`, `rea.list_strings`, `rea.decompile` / `procedure_pseudo_code` | **NOT REACHED** | Depend on a successful `open_binary`, which is blocked by #24. |

Net effect: the only working deep-analysis path in this environment is the **REA CLI with `JAVA_HOME` exported**, and even that is constrained by the 330 s provider startup deadline and by Ghidra's analyzer-name validation.

### §Ghidra

REA's Ghidra provider was the only deep-analysis engine available (Hopper / IDA / jadx / kuna are not installed).
Three attempts failed for tooling reasons, not for lack of analyzability of the target:

* attempt 1 — provider startup deadline (330 s) exceeded while importing a 76 MB binary;
* attempts 2 & 3 — the REA Ghidra wrapper rejects analyzer names that are not registered in the installed Ghidra 12.1.4 build, aborting the run before analysis.

A fourth run was launched with a valid analyzer list (`Aggressive Instruction Finder, Decompiler Parameter ID, Non-Returning Functions - Discovered`) and a 2-hour startup deadline (`REA_GHIDRA_STARTUP_TIMEOUT_MS=7200000`), bound explicitly with `--provider ghidra`.
Ghidra headless started at 05:29:19 and was **still consuming CPU (~660 s of CPU time, ≈ 1 h 10 min wall clock) on the 76 MB import/analysis phase when this report was finalised**; the Ghidra project had not yet been saved, so **no procedure list, string list or decompilation output was produced**. The host has only 7.9 GB RAM (≈ 1–2 GB free during the run), which is the main practical obstacle.

Its outcome is recorded in `exe_findings.md` § "Decompilation" and § "Not recovered / limitations".

### Tools deliberately NOT used

* No debugger, no `Start-Process`, no execution of the target, the installer, or any payload.
* No dynamic analysis (frida / gdb / WinDbg) — out of scope and blocked by the brief.
* No writes anywhere under `F:\`.
