# Tool Capability Matrix — Real Lab System 1.605

| Investigation Activity | Required Capability | Available Tool | Status |
|---|---|---|---|
| PE inspection | headers, sections, imports | Python struct + REA open_binary | Available |
| Strings extraction | ASCII/UTF-16LE | Python | Available |
| Managed decompilation | .NET IL/metadata | REA managed providers | N/A (VB6 native, no CLI) |
| Native decompilation | rizin/Ghidra/objdump | REA rizin/Ghidra/objdump | Blocked (provider unavailable) |
| Database schema | MDB/Jet parsing | prior extracted SQL (schema-Patients.sql) | Available (from evidence) |
| Database data | row reading | mdbtools/olefile/pyodbc | Not available |
| Runtime analysis | debugging/tracing | not performed | Blocked (safety) |
| UI inspection | screenshots/events | not performed | Blocked (no runtime) |
| Configuration | INI/UDL parsing | Python | Available |
| SQL analysis | query parsing | Python + manual | Available |