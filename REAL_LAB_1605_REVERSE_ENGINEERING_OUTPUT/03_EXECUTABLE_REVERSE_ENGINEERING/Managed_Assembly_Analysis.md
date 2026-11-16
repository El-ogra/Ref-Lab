# Managed Assembly Analysis — NOT APPLICABLE

## Determination: **NOT APPLICABLE** (VERIFIED)

No target component in the evidence set is a .NET managed assembly. No target code was found that carries CLR metadata, imports `MSCOREE.DLL`, or references `mscorlib`/`System.*` managed assemblies. The executable under investigation is a 32-bit **native** VB6 PE.

## Evidence

1. **Main target (1.605).** `real lab system.exe (2).json → pe.imports` contains **exactly one** imported DLL, `MSVBVM60.DLL` (231 functions). `pe-structure.json → imports` likewise lists only `MSVBVM60.DLL` and `exports: []`. A managed assembly would import `MSCOREE.DLL` (or be IL-only with a CLR header/data directory 14 `COM_DESCRIPTOR`); here `data_directories.com_descriptor` is `{rva:0, size:0}` and `pe-structure.json → pe_type: "PE32"` with `optional_magic 0x010b` and no CLR runtime header. VERIFIED.

2. **Baseline (1.00).** `real lab system.exe.json → pe.imports` is likewise only `MSVBVM60.DLL` (186 functions). VERIFIED.

3. **VB6 runtime identification.** `real lab system.exe (2).json → vb6: {is_vb6:true, imports_msvbvm60:true}`; `vb6-structure.json → vb_header.signature "VB5!"`, `runtime_build 9782` (VB6 SP6). This is the native VB6 toolchain, not the .NET toolchain. VERIFIED.

4. **Auxiliary target EXEs.** Every auxiliary executable for which a PE dump exists imports `MSVBVM60.DLL` only and is flagged VB6: `Patient Data Transport.exe` (84 imports, `Patient Data Transport.exe.json`) and `real lab Image Gallery.exe` (42 imports, `real lab Image Gallery.exe.json`). No managed indicator. VERIFIED for those two; UNVERIFIED for `real lab timer.exe`, `RealLab E-Mail Sender.exe`, `RealLab_Show_Info.exe`, `RLS ACCESS Database Tools.exe`, `RLS SQL Database Tools.exe`, `RLUTool.exe`, `calculator.exe` (no PE dumps exist in the evidence set — see `Executable_Inventory.md`).

5. **The `.NET` artifact in the package is a redistributable, not target code.** `dotNetFx40_Full_x86_x64.exe` is described in `pe-envelope.csv` as Product `Microsoft .NET Framework`, Company `Microsoft Corporation`, Description **`Box Stub`**, file version `10.0.21009.0`, Microsoft-signed, with a 50,258,992-byte overlay — the classic .NET 4 full redistributable bootstrapper. `12-install-media.md` lists it in the PDF toolchain line "PDF\ → .NET 4 + Bullzip PDF Printer", i.e. it is shipped to support Bullzip PDF Printer 11.1, **not** as a component of the RealLab application. `v1.605-package-manifest.csv` places it under `RL_CD2019Access\System\PDF\dotNetFx40_Full_x86_x64.exe`. No analysis in this deliverable set attributes any application business logic to it.

6. **No managed IL, no mixed-mode artifacts were identified** in `version-resources.json` (all listed DLLs/OCXs are native Win32/VB6-era components with Microsoft/RealLab version resources), nor in `pe-analysis.json` (which covers `acmsetup.exe`, `setupsp6.exe` and `Setup_BullzipPDFPrinter_…exe` — all native PE32).

## Scope of the negative result

- VERIFIED: the two main executables, `Patient Data Transport.exe`, `real lab Image Gallery.exe`, and all dumped runtime DLLs are native; the only .NET installer is a redistributable for a third-party printer driver.
- UNVERIFIED: internal type of the seven auxiliary executables that have no PE dump (listed in §4). They are classified by name and manifest path only.
- UNKNOWN: whether any undecomposed embedded payload (e.g. the reported 22.95 MiB high-entropy payload inside `RL_SETUP.exe`, noted in `Version_Identity_and_Scope.md §5.3`) contains managed code — it was not decomposed in this evidence set.

## Conclusion

Managed-code analysis produces no target findings. All reverse-engineering effort belongs to native/VB6 structural analysis and static string/SQL extraction, documented in `Binary_Architecture.md`, `Native_Code_Analysis.md`, `Reconstructed_Code_Structure.md`, and `Executable_Business_Logic.md`.
