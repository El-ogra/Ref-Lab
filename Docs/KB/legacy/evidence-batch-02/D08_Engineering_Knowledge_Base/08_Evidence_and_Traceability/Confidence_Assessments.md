# Confidence Assessments — RealLab v1.605

## Classification scheme

- **Class 1 — Directly established fact** (directly demonstrated by inspected evidence).
- **Class 2 — Evidence-supported engineering inference** (logical analysis of multiple observations).
- **Class 3 — Hypothesis / plausible interpretation** (provisional).
- **Class 4 — Unresolved question** (insufficient / conflicting / ambiguous evidence).

Confidence levels: High / Medium / Low / Undetermined, reflecting evidence quality, directness, consistency, independence, completeness.

## High-confidence (Class 1) areas

Product identity, VB6/native, two builds, dual-DB architecture, schema shape, catalogue/range counts, consolidated security findings (inherited but well-evidenced), version matrix, Jet 4 correction.

## Medium-confidence (Class 2)

Integrations (SMTP/SMS/serial), derivation constants, authorisation model, schema-migration behaviour, per-domain form/report mapping.

## Low / provisional (Class 3)

- Possible MySQL third backend.
- V1.00↔v1.605 object-count reconciliation (CON-002).
- Mapping of grants/IDPassword* to sections.

## Unresolved (Class 4)

UNR-002, UNR-003, UNR-004, UNR-005, UNR-008, UNR-009 — see `09_Unresolved/Open_Questions_and_Gaps.md`.

## Evidence-traceability rule

Every significant finding cites `EVD-*` IDs resolving to `01_Original_Evidence/` entries (SHA-256 + path). Registries: `08_Evidence_and_Traceability/`.