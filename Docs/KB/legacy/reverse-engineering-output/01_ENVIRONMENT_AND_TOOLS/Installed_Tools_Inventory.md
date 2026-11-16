# Installed Tools Inventory — Real Lab System 1.605

| Tool | Version | Status | Relevance |
|---|---|---|---|
| Python | 3.13.1 | Available | PE parsing, strings, CSV/JSON, SQLite |
| Node.js | 24.19.0 | Available | REA MCP runtime |
| Git | 2.49.0 | Available | version control |
| Chocolatey | 2.4.1 | Available | package manager |
| REA MCP | 207 tools | Connected | open_binary, managed inspection, compare, strings |
| SQL Server Express | 16.0.1220.3 | Running | Ref Lab dev DB (not target) |
| PostgreSQL | 17.6 | Listening | Ref Lab dev DB |
| Ghidra | 12.1.4 | Blocked (Java) | native decompilation unavailable |
| Docker | absent | Not available | — |
| mdbtools | not installed | Not available | Access DB parsing unavailable |
| olefile/pyodbc/pandas | not installed | Not available | — |
| tshark/tesseract/ffmpeg | installed | Available | packet OCR media (not used) |

Tool-to-task matrix: PE → Python (struct) + REA open_binary; strings → Python; managed → REA (returns not-managed for VB6 native); DB schema → prior extracted SQL; DB data → not accessible (no mdbtools); runtime → not performed (safety).