# Tool Execution Log - verification run 2026-11-16T18:26:49.7039064+02:00

| Check | Status | Observed |
|---|---|---|
| powershell | OK | pwsh 7.6.10 |
| node | OK | node v24.19.0 |
| npm | OK | npm 11.17.0 |
| python | OK | python Python 3.13.1 |
| dotnet | OK | 9.0.321 |
| java | OK | openjdk version "21.0.13.0.2" 2026-10-22 LTS |
| rea-cli | OK | 6.7.0 |
| sqlcmd | OK | Microsoft (R) SQL Server Command Line Tool |
| psql | FAIL | The term 'psql' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again. |
| git | OK | git version 2.49.0.windows.1 |
| ghidra-headless | OK | present |
| JAVA_HOME | OK | (unset) |
| tshark | FAIL | The term 'tshark' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again. |

Note: REA MCP server verified live via tools.rea.binary_session (207 tools); Ghidra provider runtime_unavailable (java.exe absolute-path fault; JAVA_HOME unset at server start).
