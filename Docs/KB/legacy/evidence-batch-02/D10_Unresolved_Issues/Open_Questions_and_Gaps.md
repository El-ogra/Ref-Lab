# Open Questions and Evidence Gaps — RealLab v1.605

## Unresolved questions (UNR-002..UNR-009)

- **UNR-002** — Meaning of the 17 `grantN` operator-permission columns. Status: UNRESOLVED.
- **UNR-003** — The seven `IDPassword*` values and what they unlock. Status: UNRESOLVED.
- **UNR-004** — Two 32-character uppercase literals (possible obfuscated credentials). Status: UNRESOLVED.
- **UNR-005** — Whether `accuntpw` is hashed (evidence suggests cleartext). Status: UNRESOLVED.
- **UNR-008** — Mapping of ~1,370 embedded SELECT statements to specific backends. Status: PARTIALLY RESOLVED.
- **UNR-009** — Names of the 5 unnamed external components (header declares 25, 20 named). Status: UNRESOLVED.

## Evidence gaps (missing artefacts)

- `RL_CD2019Access/System/real lab system.exe` — would settle single-vs-multiple builds.
- `bin/calculator.exe`, `bin/real lab timer.exe` — sibling apps referenced by path.
- `real lab Image Gallery.exe` (as a binary, not just metadata) — pictures consumer.
- `vkUserControlsXP.ocx` — orphaned (only .oca present).
- Avira engine (`avipbb.sys` + DLLs/.dat).
- `D:\Lab_data\Attaching.sql` + `Attach_database.bat`.
- VS6sp6B cabinet member contents.
- `Patients_backup.mdb` (as a received file).

## Contradictions carried (not resolved)

- **CON-002** — 169 vs 212 object/form counts (provisional version delta).
- **CON-003** — 1,337 vs 1,370 SELECT count (immaterial).

## Additional evidence required to resolve

- Both builds' VB6 object tables (to confirm CON-002).
- Decompiled method bodies (to map grants/IDPassword* and SELECT-to-backend).
- The missing sibling executables and installer payload.