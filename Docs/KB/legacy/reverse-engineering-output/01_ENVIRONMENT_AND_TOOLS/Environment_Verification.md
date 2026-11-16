# Environment Verification — Real Lab System 1.605

Environment: Windows 10 Pro, x64, no admin rights. .NET SDK 9.0.321/8.0.428, Python 3.13.1, Node 24.19.0, Git 2.49.0, Chocolatey 2.4.1, REA MCP (207 tools), SQL Server 2022 Express (running), PostgreSQL 17.6 (listening). Ghidra 12.1.4 installed but blocked by Java resolution (REA Ghidra provider unavailable). No Docker, no native C/C++ compilers, no mdbtools, no olefile/pyodbc/pandas. No MySQL/MongoDB/Redis.

Available for RE: Python (PE parsing, strings, SQLite), REA (open_binary, managed inspection, compare), file, strings, PowerShell, Git. REA native providers (rizin/Ghidra/objdump) unavailable for PE targets. No runtime execution performed (safety).