# Failed Analysis Operations — Real Lab System 1.605

- REA binary_overview / list_strings / list_procedures / list_segments / list_names: provider_unavailable (Ghidra Java resolution failure).
- REA inspect_with_objdump: capability_unavailable (PE target, ELF-only).
- REA inspect_pe_resources: resource_constraint (file > 64MB).
- REA inspect_signature: capability_unavailable for target.
- REA batch_decompile: succeeded but 0 items (no addresses; native decompilation unavailable).
- mdbtools: not installed (mdb-tables not found).
- olefile/pyodbc/pandas: not installed.
- Runtime execution: not performed (safety).
- DB row reading: not performed (no tools).
- Method body decompilation: not available (VB6 native code does not store names; Ghidra blocked).