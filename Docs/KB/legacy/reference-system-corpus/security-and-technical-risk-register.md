# Security and Technical Risk Register — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.** No exploit was
attempted or demonstrated, no credential validated, no external system
contacted, no CVE or advisory asserted.

---

## 1. Risk posture

**RealLab CD 2019 v1.605 is a 1990s–2000s client/server clinical system
still deployed on Windows.** Its risk profile is not a set of subtle
implementation bugs; it is the cumulative consequence of a design that trusted
its network, its local disk and its own operators.

| Dimension | Assessment |
| --- | --- |
| **Confidentiality of clinical data** | **Weak.** Credentials in the binary, operator passwords in a `varchar` column, an SMS gateway over plain HTTP, a mirror with its own backups on every workstation |
| **Integrity of clinical records** | **Weak.** The client runs DDL against the server database; there is no integrity protection on any template or reference document |
| **Availability** | **Moderate.** A 76 MB monolithic executable with no ASLR or DEP; two 16 MB reference documents loaded whole; a 16-bit launcher in the deployment path |
| **Privilege boundary** | **Weak.** 16 ActiveX controls self-registered into the system directory; a renamed Microsoft typeface installed machine-wide |
| **Authentication** | **Weak.** A published default operator, a single shared maintenance password, seven shared print passwords, and no role model |
| **Supply chain** | **Moderate.** An evaluation-edition commercial printer installed silently; orphaned third-party AV descriptors; obsolete Microsoft redistributables |

---

## 2. The seventeen High findings, grouped by theme

### Theme A — clinical data crosses trust boundaries in cleartext

| Unified | Subject | Origin | Evidence |
| --- | --- | --- | --- |
| **U-001** | SQL `sa` credential in cleartext, assembled at run time from separate literals, with `Persist Security Info=True` | B1 FND-001/002/003/007 · FIN FND-F-001 | the three literals in the executable; the same fragments in B2's `NOTEX.dll` |
| **U-002** | SMS gateway called over **plain HTTP** with the account password in the URL query string | B1 FND-017 · FIN FND-F-002 | the reconstructed `InternetExplorer.Application` → `navigate2` flow |
| **U-003** | Operator password stored in `accuntpw varchar(50)` — no hash column, no salt column | FIN FND-F-003 | the executable's DDL and its four `useraccounts` queries |
| **U-005** | Default operator credential **`admin`/`admin`** published in the shipped manual | B4 FND-B4-002 | B4's manual, topic 2 |
| **U-006** | Database Maintenance — which can **zero the entire clinical database** — guarded by the password **`123`** | B4 FND-B4-003 | B4's manual, topic 15 |

**Why this is the dominant theme.** These are not five unrelated bugs. They
are one architectural assumption — *the clinical database, the operator's
credential and the outbound SMS channel are inside the trust boundary* —
applied consistently. In 1990s client/server architecture on a private LAN,
that was correct.

### Theme B — the client holds server-side privilege it does not need

| Unified | Subject | Origin | Evidence |
| --- | --- | --- | --- |
| **U-004** | The executable carries **120 `ALTER TABLE` statements across 31 tables** and 14 `CREATE TABLE`, and runs them against the server database | FIN FND-F-004 | the executable's embedded DDL |

**This is the one finding that is new in kind rather than a new instance.**
Every other High finding would follow from the 1990s design; this one does
not. A user who can start the application can change the schema of the
clinical database, silently and without an audit trail. The application
holds `ALTER` on the clinical database because nothing ever took it away.

### Theme C — authentication and authorisation without a model

| Unified | Subject | Origin | Evidence |
| --- | --- | --- | --- |
| **U-011** | 17 boolean `grantN` columns on `Patients` and seven shared `IDPassword*` print secrets form a coarse, undocumented authorisation model | FIN FND-F-007/008 | the executable's DDL and literals; B4's manual |
| **U-012** | The maintenance password protects irreversible erasure with a 3-character constant | B4 FND-B4-003 | B4's manual |

**There is no role model anywhere in the evidence.** Authorisation is boolean
columns, shared numeric secrets and a maintenance password.

---

## 3. The Medium and Low findings

| Unified | Subject | Severity | Origin |
| --- | --- | --- | --- |
| U-007 | No ASLR, no DEP/NX, no CFG; binaries unsigned | Medium | B1 FND-008, B2 FND-B2-006, FIN FND-F-005 |
| U-008 | Renamed Microsoft Tahoma installed machine-wide; fonts whose own metadata says *"Redistribution strictly prohibited"* | Medium | B3 FND-B3-008, B4 FND-B4-004 |
| U-009 | The report subsystem is a scriptable HTML surface — three script hosts plus IE COM | Medium | B2 FND-B2-001, B3 FND-B3-002, B4 FND-B4-008, FIN FND-F-006 |
| U-010 | Report layouts are per-workstation; the manual says edits apply only to the machine where made | Medium | FIN FND-F-009, B4 FND-B4-009 |
| U-013 | The client migrates the schema without a versioning table or an audit trail | Medium | FIN FND-F-004 |
| U-014 | Obsolete Microsoft components installed system-wide with COM self-registration | Medium | B4 FND-B4-004 |
| U-015 | The installer runs unverified payloads by absolute path; the database is attached from a fixed `D:\Lab_data` | Medium | B4 FND-B4-005 |
| U-016 | An undocumented MySQL back end via a 2006-era ODBC driver | Medium | B4 FND-B4-006 |
| U-017 | A commercial PDF printer silently installed in evaluation mode with an expired certificate | Medium | B4 FND-B4-007 |
| U-018 | No integrity protection on 204 clinical-content files | Medium | B3 FND-B3-001 |
| U-019 | Arabic allergen labels depend on the host codepage | Medium | B3 FND-B3-003 |
| U-020 | The maintenance window can erase the backup store as well as the live database | Medium | B1 FND-005/019, B4 FND-B4-003 |
| U-021 | Orphaned Avira antivirus kernel-driver descriptors shipped with no binaries | Low | B4 FND-B4-011 |
| U-022 | A 16-bit Windows 3.1 executable in the required deployment path | Low | B4 FND-B4-012 |
| U-023 | Microsoft debug symbol files shipped inside the clinical installation | Low | B4 FND-B4-013 |
| U-024 | Hardcoded `D:\real lab system\` paths in the executable | Low | FIN FND-F-009 |
| U-025 | No manifest resource — no UAC level, DPI awareness or common-controls v6 | Info | FIN FND-F-010 |
| U-026 | Vendor, product and version confirmed from three independent embedded sources | Info | FIN FND-F-011 |
| U-027 | The executable's own `MSDBRPTR` / legacy data-access behaviour | Info | B2, FIN |

*(The table above names the principal Medium/Low/Info subjects. The complete
41-subject register, with every original identifier and the full per-origin
text, is `machine-readable/unified-findings-register.json`.)*

---

## 4. Prioritised remediation

| # | Action | Addresses |
| ---: | --- | --- |
| 1 | **Remove the client's DDL privilege.** Ship schema changes as versioned migration scripts run by a DBA, with an audit trail. | U-004, U-013 |
| 2 | **Move the database credential out of the binary** into an OS-protected store; rotate it; drop `Persist Security Info=True` | U-001 |
| 3 | **Move the SMS gateway to HTTPS**, put the credential in a header rather than a URL, and replace the IE automation with a direct client | U-002, U-009 |
| 4 | **Hash the operator passwords** with a salted KDF; migrate `accuntpw` | U-003 |
| 5 | **Remove the three hardcoded credentials** — `sa` password, `admin`/`admin`, maintenance `123` — in favour of per-deployment values | U-001, U-005, U-006, U-012 |
| 6 | **Rebuild with ASLR, DEP/NX and CFG, and sign the binaries.** | U-007 |
| 7 | **Introduce a role model**; replace the `grantN` flags and the shared print secrets with per-user permissions | U-011, U-012 |
| 8 | **Resolve the font redistribution position** and stop installing a renamed Microsoft typeface machine-wide | U-008 |
| 9 | **Manage the report layout centrally** — version it, distribute it from the server, log changes | U-010, U-018 |
| 10 | **Establish the reference-library ↔ test binding**, and close `Data/REPORT.ini` | U-018, UNR-011 |
| 11 | **Decide whether MySQL is supported**; remove `Setup5.bat` if not | U-016 |
| 12 | **Remove the orphaned Avira descriptors and the MS `.dbg` files** from the media | U-021, U-023 |
| 13 | **Retire the VS6 SP6 deployment path** (16-bit launcher, 1996 help file) | U-022 |
| 14 | **Verify installer payloads by digest** before running them | U-015 |

---

## 5. What is a confirmed weakness versus what needs verification

| **Confirmed weakness** (established from content or vendor documentation) | **Requires further verification** |
| --- | --- |
| The credential fragments are in the binary and are concatenated at run time | Whether the substituted password is a literal elsewhere, or operator-supplied |
| The SMS request is plain HTTP with the password in the URL | Whether the deployed endpoint has since moved to HTTPS |
| `accuntpw` is `varchar(50)` with no hash or salt column | Whether the application hashes before the INSERT |
| The manual documents `admin`/`admin` and the maintenance password `123` | Whether deployed systems still use them |
| 120 `ALTER TABLE` statements are embedded | Whether every one executes on every start |
| `DllCharacteristics = 0x0000`; no Authenticode; no manifest | The exploitability of any specific memory-safety bug — none was sought or claimed |
| The fonts' own metadata names Microsoft and Weatherly Systems and says *"Redistribution strictly prohibited"* | Whether the vendor holds separate redistribution licences — unknowable from the files |
| `grantN` columns exist on `Patients` | **What they mean** (UNR-002) |

---

## 6. Boundaries

* **No exploit was demonstrated, attempted, or attempted to be demonstrated.**
* **No credential was validated.**
* **No CVE or advisory is asserted.** No third-party executable in the
  distribution was assessed for known vulnerabilities; the Bullzip certificate
  expiry is an observation, not a vulnerability.
* **B2's findings are inherited**, not re-derived.
* **Every finding is traceable** to an original identifier, a source artefact
  and an evidence location.
