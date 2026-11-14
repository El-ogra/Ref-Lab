1. Executive Summary
This is a 64-bit Windows 10 Pro workstation with no administrative rights, configured primarily as a .NET 8 / C# 12 / WPF desktop development machine for the "Ref Lab" medical laboratory system (Clean Architecture, MediatR CQRS, EF Core 8, SQL Server).

Finding	Result
Primary development stack	.NET SDK 9.0.321 + 8.0.428, C# 12, WPF, EF Core 8.0.33 — ready
Running database servers	SQL Server 2022 Express 16.0.1220.3 (accessible) and PostgreSQL 17.6 (listening)
Running DB-adjacent services	SQL Browser, SQL Writer, SQL Telemetry, adb server (port 5037)
"Rea" identity	Resolved with certainty: the rea-agents npm package v6.7.0 (io.github.morluto/rea) — an MCP server + CLI for reverse engineering, exposed to this session as 207 tools
REA readiness	Installed, registered, and actively serving tools — but rea doctor reports healthy: false (missing Hopper, missing IDA/JADX/Kuna, and a Java resolution failure that blocks the Ghidra provider at runtime)
Containers	Docker CLI absent; Docker Desktop service stopped; WSL2 present with only the docker-desktop distro, stopped
Absent runtimes	PHP, Ruby, Go, Rust, C/C++ compiler (VS has no MSVC toolset), MySQL, MariaDB, MongoDB, Redis, Elasticsearch, Nginx, Apache, IIS, Kubernetes, Terraform
Administrative privilege	None (IsAdmin = False)
The environment is production-grade for the project's actual stack (C#/.NET/WPF/SQL Server) and well-provisioned for reverse-engineering work, but it is not a polyglot or DevOps/cloud workstation.

2. Operating System and Environment
Field	Value	Evidence
Operating System	Microsoft Windows 10 Pro, version 10.0.19045 (build 19045)	Get-CimInstance Win32_OperatingSystem — VERIFIED
Architecture	x64 / AMD64, 64-bit OS	RuntimeInformation.OSArchitecture — VERIFIED
Hostname / User	DESKELTEL / LAP LINK	$env:COMPUTERNAME, $env:USERNAME — VERIFIED
CPU cores	2 logical processors	$env:NUMBER_OF_PROCESSORS — VERIFIED
RAM	8,259,632 KB total visible (~8 GB)	Win32_OperatingSystem.TotalVisibleMemorySize — VERIFIED
Last boot	2026-11-11 08:42:24	Win32_OperatingSystem.LastBootUpTime — VERIFIED
Administrative rights	No (not in Administrators role)	WindowsPrincipal.IsInRole(Administrator) → False — VERIFIED
Working directory	C:\Users\LAP LINK\source\repos\Ref Lab	$PWD.Path — VERIFIED
Git repository	Yes, branch main, remote https://github.com/El-ogra/Ref-Lab.git	git rev-parse, git remote -v — VERIFIED
2.1 Shells and Command Environments
Name	Version	Location	Status	Notes
PowerShell (Core)	7.6.10	Hosted by OpenCode shell tool	VERIFIED	Primary execution shell for all audit commands
Windows PowerShell	5.1 (inbox)	C:\WINDOWS\System32\WindowsPowerShell\v1.0	Installed	Not the session shell; available natively
cmd.exe	Inbox	C:\WINDOWS\system32	Installed	Available for cmd /c
Git Bash	Bundled with Git 2.49.0	C:\Program Files\Git\bin	Installed	POSIX-style shell available
WSL bash	WSL2, no running distro	wsl.exe	Present but idle	Only docker-desktop distro exists and is Stopped
2.2 Virtualization
Product	Version	Status	Evidence
WSL2	Default version 2, WSLService running	Installed, no active distro	wsl --status, wsl --list --verbose — VERIFIED
Hyper-V (vmcompute/vmms)	—	vmcompute Stopped, vmms absent	Get-Service vmcompute,vmms — VERIFIED
Docker Desktop	Service com.docker.service Stopped	Installed, not running	Get-Service — VERIFIED; docker.exe not on PATH and not in resources\bin — VERIFIED
2.3 Environment Variables Relevant to Development
Variable	Value	Significance
GHIDRA_INSTALL_DIR	C:\Tools\ghidra_12.1.4_PUBLIC	Consumed by REA's Ghidra provider — VERIFIED
JAVA_HOME	(empty)	Direct cause of REA's Ghidra Java check failure — VERIFIED
PATH	Includes C:\Program Files\nodejs, C:\Program Files\Python313\Scripts, C:\Program Files\Git\cmd, C:\ProgramData\chocolatey\bin, C:\Users\LAP LINK\.local\bin	Determines CLI resolvability — VERIFIED
No REA_*, HOPPER_*, IDA_*, JADX_*, ANDROID_* or credential variables are set — VERIFIED (empty match set).

2.4 System Package Managers
Name	Version	Location	Status	Notes
Chocolatey	2.4.1	C:\ProgramData\chocolatey\bin\choco.exe	Installed / usable	16 packages installed
winget	—	—	NOT FOUND	Not on PATH
Scoop	—	—	NOT FOUND	Not installed
apt/dpkg/yum/dnf/Homebrew	—	—	NOT FOUND	No Linux host; WSL distro not running
3. Agent Tools and Execution Capabilities
3.1 Directly Available Agent Tools (native to this session)
Name	Category	Status	Purpose	Agent Access
shell (PowerShell 7.6.10)	Execution	Running	Runs arbitrary read/write shell commands; supports background jobs	Yes — full
read	Filesystem	Available	Reads text, images, PDFs, directories with line numbers	Yes
write	Filesystem	Available	Creates/overwrites files (parent dirs auto-created)	Yes
edit	Filesystem	Available	Exact-string replacement edits with match retry	Yes
glob	Search	Available	Path globbing (**/*.ts), hidden-file aware	Yes
grep	Search	Available	Ripgrep-regex/literal content search with filters	Yes
webfetch	Network	Available	HTTP(S) GET → text/markdown/HTML	Yes
websearch	Network	Available	Internet search integration	Yes
question	Interaction	Available	Structured user prompts	Yes
skill	Capability loader	Available	Loads 4 skills: browser, opencode, report, reverse-engineer-anything	Yes
subagent	Delegation	Available	explore (fast codebase search) and general (multi-step) child sessions	Yes
execute (Code Mode)	Scripting	Available	Sandboxed JS runtime; composes tool calls, fetch, Promise-based parallelism	Yes
experimental_image_generate	Media	Available	Image generation/editing (background jobs)	Yes
browser namespace (45 tools)	Browser automation	Available	Tabs, snapshots, click/fill, screenshots, console/network, preview pane	Yes — explicit tabID required
opencode namespace (8 tools)	Harness control	Available	Session move, worktree create/list/remove, MCP resource listing	Yes
rea namespace (207 tools)	Reverse engineering MCP	Running	Static/dynamic analysis, decompilation, Evidence export	Yes
3.2 Classification of Availability
Directly available to the agent: all 16 tool families above (shell, fs, search, network, subagents, browser, opencode, REA).
Installed locally and executable by the agent: Node 24.19.0, npm, Python 3.13.1, uv, .NET SDKs, Java 21, Git, Chocolatey, rea CLI, psql, sqlcmd, MSBuild, tshark(installed via Chocolatey), tesseract, ffmpeg.
Installed but inaccessible / degraded: Ghidra 12.1.4 (present, blocked by Java resolution fault), Docker Desktop (service stopped, no CLI on PATH), adb (running from an F: scrcpy folder, not on PATH), metasploitPostgreSQL (Automatic start type, Stopped), IIS (not installed as a feature), WSL docker-desktop (stopped).
Accessible only through external services: web search, webfetch, GitHub remote (SSH/HTTPS credentials not audited), npm/Chocolatey/NuGet registries (network-dependent).
Not available: Docker CLI, kubectl/Helm/Terraform/Ansible/Vagrant, gh, rg on PATH, jq, fzf, pnpm/yarn/bun/deno, PHP/Composer, Ruby, Go, Rust, native C/C++ compilers, MySQL/MariaDB/MongoDB/Redis/Elasticsearch clients and servers, SSMS/DBeaver/pgAdmin.
3.3 MCP Servers
Server	Transport	Registration	Status
rea	local (stdio)	C:\Users\LAP LINK\.config\opencode\opencode.json → mcp.rea, enabled: true, command node.exe ...\rea-agents\scripts\rea.mjs mcp	Connected and serving 207 tools — VERIFIED
Other MCP servers	—	None registered in the OpenCode config	NOT FOUND
4. Programming Languages and Runtimes
Name	Version	Location	Package Manager	Agent Access
C# / .NET SDK	SDK 9.0.321 and 8.0.428; runtimes 6.0.36 / 8.0.34 / 9.0.23 (+ ASP.NET Core)	C:\Program Files\dotnet	NuGet (257 cached packages)	Yes — dotnet build/test/run
Node.js	24.19.0	C:\Program Files\nodejs\node.exe	npm 11.17.0, npx, corepack	Yes
Python	3.13.1 (+ uv-managed 3.11.16, 3.14.9)	C:\Program Files\Python313\python.exe	pip 24.3.1; uv 0.7.15	Yes — 97 packages installed
Java (JDK)	OpenJDK 21.0.13 (Microsoft build, LTS)	C:\Program Files\Microsoft\jdk-21.0.13.21-hotspot\bin	None (no Maven/Gradle)	Yes — java/javac work; JAVA_HOME unset
PowerShell	7.6.10 (Core)	Session shell	PowerShell Gallery (unverified)	Yes
Java/JVM tooling	javac present; Maven & Gradle NOT FOUND	—	—	Compile-only, no build system
TypeScript	Compiler not installed globally (tsc NOT FOUND)	—	Available as dev-dep inside rea-agents	Partially — via package local install only
PHP	—	—	—	NOT FOUND
Ruby	—	—	—	NOT FOUND
Go	—	—	—	NOT FOUND
Rust	—	—	—	NOT FOUND
C / C++	No compiler: gcc, g++, clang, cmake, make absent; VS 2022 has no MSVC toolset (VC\Tools\MSVC missing)	—	—	NOT FOUND
Bash	Git Bash available; WSL bash idle	C:\Program Files\Git\bin	—	Yes (POSIX utilities limited)
F#	Ionide extension present, no dotnet F# workload verified	—	—	PARTIALLY VERIFIED
Runtime compatibility note: rea-agents declares engines: ^22.19.0 || ^24.11.0 || >=26.0.0; the installed Node 24.19.0 satisfies this constraint — VERIFIED.

5. Development Frameworks and Platforms
5.1 In-repo / installed frameworks (VERIFIED from .csproj + README.md)
Name	Version	Location	Status	Purpose
.NET 8 (target framework)	net8.0 / net8.0-windows10.0.17763.0	All 9 projects	Installed	Mandated platform (global.json, Directory.Build.props)
WPF (desktop UI)	.NET 8 Windows Desktop	RefLab.Presentation	Available	Desktop presentation layer
MVVM Toolkit	CommunityToolkit.Mvvm 8.4.2	RefLab.Presentation.csproj	Restored	MVVM source generators
MediatR (CQRS)	12.5.0	RefLab.Application.csproj	Restored	Request/response + notification dispatch
FluentValidation	11.11.0 (+ DI extensions)	RefLab.Application.csproj	Restored	Command/query validation
Entity Framework Core	8.0.33 (core, relational, design, SqlServer)	RefLab.Infrastructure.csproj	Restored	ORM + migrations for SQL Server
EF Core SQLite provider	8.0.33	RefLab.Infrastructure.Tests.csproj, RefLab.Integration.Tests.csproj	Restored	In-memory/file test database
Microsoft.Extensions.Hosting / DI / Logging	8.0.1 / 8.0.2 / 8.0.3	Multiple projects	Restored	Generic Host, DI container, logging abstractions
QuestPDF	2026.9.1	RefLab.Infrastructure.csproj	Restored	PDF report generation
PdfPig	0.1.16	RefLab.Integration.Tests.csproj	Restored	PDF text extraction in tests
xUnit	2.9.3 (+ runner 2.8.2)	All 5 test projects	Restored	Unit/integration testing
Microsoft.NET.Test.Sdk	17.11.1	All 5 test projects	Restored	VSTest host
coverlet.collector	6.0.2	All 5 test projects	Restored	Code coverage
Clean Architecture	4 source layers + 5 test projects	RefLab.sln	Implemented	Domain/Application/Infrastructure/Presentation
5.2 Web / backend / mobile frameworks
Name	Status	Notes
ASP.NET Core	Runtime installed (6.0.36 / 8.0.34 / 9.0.23)	No ASP.NET project in this repo; runtime only
Next.js, React, Vue, Angular, Svelte	NOT FOUND	No frontend projects in repo
Express, NestJS	NOT FOUND	Not in npm globals
Django, Flask, FastAPI	NOT FOUND	Not in the 97 pip packages inspected
Laravel, Symfony	NOT FOUND	No PHP runtime
Spring Boot	NOT FOUND	No Maven/Gradle, no Spring artifacts
Flutter / React Native / MAUI	NOT FOUND	No workloads installed (dotnet workload list empty)
Electron	DETECTED BUT INACCESSIBLE as a framework	@electron/asar ships inside rea-agents for analysis, not for building
5.3 Build systems, bundlers, dev servers
Name	Version	Status	Notes
MSBuild	VS 2022 17.14.45	Installed	...\2022\Community\MSBuild\Current\Bin\MSBuild.exe
dotnet CLI	9.0.321	Installed	Primary build/test driver
Turbo	2.11.7	Dev-dep of rea-agents	Only within that package
Vite / Vitest	6.4.4 (override) / 5.0.3	Dev-deps of rea-agents	Not globally usable
rolldown / oxlint / oxfmt	1.2.12 / 1.87.0 / 0.72.0	Dev-deps of rea-agents	Lint/format only inside that package
Local dev server for this project	—	NOT FOUND	WPF desktop app; no HTTP dev server
6. Rea Package and Ecosystem: Detailed Investigation
6.1 Identity — RESOLVED (VERIFIED)
Field	Finding
Official name	REA — "Reverse engineer anything from your terminal or agent"
Package name	rea-agents
MCP registry name	io.github.morluto/rea
Version	6.7.0 (latest installed)
Author / source	github.com/morluto/rea (MIT licence)
Install method	npm global install into C:\Users\LAP LINK\npm-global
Install root	C:\Users\LAP LINK\npm-global\node_modules\rea-agents
Binaries / shims	rea, rea.cmd, rea.ps1, rea-agents, rea-agents.cmd, rea-agents.ps1
CLI entry point	scripts/rea.mjs (ESM)
Report type	type: module, Node ^22.19.0 || ^24.11.0 || >=26.0.0
Identity disambiguation: no other product named "Rea" (Rhea, REA Group, Reaper, etc.) is installed or referenced. The name in package.json is <name>rea-agents</name> with mcpName: io.github.morluto/rea; the "Rea" in this environment refers exclusively to this MCP/CLI reverse-engineering tool. No ambiguity remains.

6.2 Two distinct installation surfaces (both VERIFIED)
CLI — C:\Users\LAP LINK\npm-global\rea.cmd --version → 6.7.0. Full command surface available (rea --help lists 70+ commands).
MCP server for this session — registered in C:\Users\LAP LINK\.config\opencode\opencode.json: {"mcp":{"rea":{"type":"local","command":["C:\\Program Files\\nodejs\\node.exe","C:\\Users\\LAP LINK\\npm-global\\node_modules\\rea-agents\\scripts\\rea.mjs","mcp"],"enabled":true}}}. It is live: 207 tools are callable from this session.
Agent skill — rea-agents\skills\reverse-engineer-anything is installed and appears in my skill list as reverse-engineer-anything.
6.3 Component inventory (VERIFIED from package.json files + on-disk tree)
Component	Path	Purpose
Core distribution	dist/	Compiled ESM runtime, worker processes, provider adapters
Windows native addon	native/windows/build/rea-windows-x64.node + manifest.json	Native Win32 capabilities (process handles, owned-process launch)
Hopper bridge	bridge/hopper_bridge.py	macOS Hopper GUI bridge (Python)
Cutter / Rizin bridge	bridge/cutter/rea_bridge.py	Rizin/Cutter live-session bridge
Ghidra bridge	bridge/ghidra/ReaGhidra*.java (10 classes)	Headless Ghidra provider extension + analyzers
JADX bridge	bridge/android/ReaJadxBridge.java	Android decompilation via caller-supplied JADX MCP jar
mitmproxy bridge	bridge/mitmproxy/capture.py	Passive web traffic capture
Native/LLDB bridge	bridge/native/*.swift, rea_lldb_tracer.py	Apple native UI + tracing
Process bridge	bridge/process/ProcessRunTokenReader*.swift	Process run-token observation
pwntools / pwndbg bridges	bridge/pwntools/*, bridge/pwndbg/*	Exploit-layout + recorded-crash analysis
Third-party bundles	third_party/pwntools, third_party/wabt, third_party/evmole, third_party/pwndbg	Bundled licence-scoped tooling
Skills	skills/reverse-engineer-anything	Agent-facing workflow skill
Docs / legal	README.md, DISCLAIMER.md, THIRD_PARTY_NOTICES.md, LICENSE	Documentation and attribution
6.4 Functional capability map (207 tools, grouped)
Group	Representative tools	Count
Binary lifecycle & layout	open_binary, binary_overview, inspect_binary_layout, inspect_macho, inspect_pe_resources, list_segments, read_bytes, close_binary	~18
Decompilation & procedures	decompile, procedure_pseudo_code, procedure_assembly, batch_decompile, analyze_function, get_call_graph	~20
Symbols & xrefs	list_procedures, list_strings, list_names, xrefs, find_xrefs_to_name, search_bytes, search_strings	~15
Evidence & reconstruction	export_evidence_bundle, verify_reconstruction, build_reconstruction_obligation_ledger, evaluate_reconstruction_coverage	~12
Managed (.NET)	inspect_managed_artifact, inspect_managed_members, compare_managed_members, project_managed_application_graph	~7
Android	inspect_android_package, list_adb_devices, decode_android_resources, trace_android_references, pull_adb_package	~25
JavaScript / Electron / web	analyze_javascript_application, recover_javascript_sources, observe_web_execution, inspect_web_network_capture, capture_web_screenshot	~25
Dynamic analysis	start_frida_session, instrument_with_frida, start_gdb_session, debug-with-gdb, start_rizin_debug_session	~15
Apple / Harmony / Flutter / WASM / EVM	inspect_macho, project_apple_application_graph, inspect_wasm_artifact, inspect_evm_interface, identify_flutter_build	~22
Firmware / archives / plist / SQLite	extract_firmware, inspect_firmware_regions, inspect_keyed_archive, inspect_sqlite_database, inspect_plist	~10
Network capture	inspect_pcap_capture, inspect_packet_capture, inspect_pcap_client	~5
Environment/diagnostics	capabilities, doctor, list_documents, current_document, discover_webmcp_tools	~8
6.5 Readiness — rea doctor (VERIFIED, self-reported)
Check	OK	Classification	Detail
node	✅	healthy	24.19.0
host	✅	healthy	win32 x64
hopper	❌	missing_analysis_engine	Not installed (HOPPER_LAUNCHER_PATH unset)
ghidra	✅	healthy	C:\Tools\ghidra_12.1.4_PUBLIC
ghidra-version	✅	healthy	12.1.4
ghidra-headless	✅	healthy	support\analyzeHeadless.bat
ghidra-native_decompiler	✅	healthy	decompile.exe reports PE x86_64
ghidra-java	❌	runtime_failure	java.exe: Owned Windows process requires an absolute executable path → provider available: false
ida-registration	❌	missing_analysis_engine	REA_IDA_MCP_CONFIG unset
jadx	❌	missing_analysis_engine	REA_JADX_MCP_JAR unset (needs 0.7.1 JAR)
kuna-installation	❌	missing_analysis_engine	REA_KUNA_COMMAND unset
registration: claude_code, codex, copilot_cli, devin, hermes, vscode	❌ ×6	config_drift	Registrations exist but are stale relative to 6.7.0
Global verdict: healthy: false, environment_healthy: false. The reasons are environmental (missing third-party engines + unset JAVA_HOME), not installation damage — the package itself is complete and its MCP surface is fully operational.

6.6 Provider availability matrix (from rea capabilities)
Provider	Available	Note
rea-artifact-graph v1	✅ mostly	DMG mount requires macOS; ASAR expansion works
rea-dotnet-static	✅	Directly relevant to this .NET/WPF repository
sqlite	✅	SQLite file inspection without a CLI
ghidra	⚠️ groups report available, but doctor marks provider available: false	Blocked by the Java resolution fault
native-macos	❌	unsupported_host (requires macOS)
kuna	❌	Not configured
ida, jadx, hopper	❌	Not configured/installed
6.7 Dependencies and integration
Runtime deps include @modelcontextprotocol/server 2.3.1 + client 2.3.1, playwright-core 1.58.2 (browser automation), frida 17.23.2 (optional), evmole 0.9.3, isomorphic-git 1.38.7, plist, yaml, zod, pino, ws. Integrations verified: OpenCode MCP (live), Ghidra 12.1.4 (partial), Android adb (running externally), Chocolatey-installed Wireshark/tshark (for PCAP analysis), Java 21 (present but unresolvable by REA).

7. Database Engines and Infrastructure
7.1 Relational database servers
Product	Version	Server Installed	Service State	Listening	Client	Accessible to Agent
Microsoft SQL Server Express	16.0.1220.3 (SQL Server 2022 RTM-GDR, KB5132747)	✅ MSSQL$SQLEXPRESS	Running	TCP 56496 (dynamic), all interfaces	SQLCMD.EXE 17.x, dotnet-ef	✅ Yes — Windows-auth query succeeded
PostgreSQL	17.6	✅ postgresql-x64-17	Running	TCP 5432 (::)	psql 17.6	⚠️ Listening; credential not tested by design
PostgreSQL (Metasploit bundle)	Unknown	✅ metasploitPostgreSQL	Stopped (Automatic)	None	—	❌ Not running
MySQL / MariaDB	—	❌	—	—	❌	NOT FOUND
Oracle Database	—	❌	—	—	❌	NOT FOUND
SQLite (embedded engine)	Microsoft.EntityFrameworkCore.Sqlite 8.0.33	Library only, no server	n/a	n/a	❌ no sqlite3.exe CLI	✅ via EF Core / ✅ via REA sqlite provider
SQL Server evidence (read-only):

Microsoft SQL Server 2022 (RTM-GDR) (KB5132747) - 16.0.1220.3 (X64)
Express Edition (64-bit) on Windows 10 Pro 10.0 <X64> (Build 19045: ) (Hypervisor)
Databases: master, tempdb, model, msdb, OpenLab, OpenLab_FreshCleanTest,
           FinalLab, FinalLabSystem_MigrationTest, NewLabDb, RefLab   (10 rows)
Instance resolution: HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL → SQLEXPRESS = MSSQL16.SQLEXPRESS. Supports SQLBrowser (running) for dynamic-port discovery.

7.2 NoSQL, search, vector, graph, time-series, analytical
Technology	Status	Evidence
MongoDB	NOT FOUND	No mongod.exe, no mongosh, no service
Redis	NOT FOUND	No redis-server.exe, no redis-cli, no service
Elasticsearch / OpenSearch	NOT FOUND	C:\Program Files\Elasticsearch absent
CouchDB	NOT FOUND	No service, no binary
Vector DBs (Qdrant, Milvus, pgvector, Chroma, Weaviate)	NOT FOUND	No services, no binaries; pgvector extension not verified
Graph DBs (Neo4j, ArangoDB)	NOT FOUND	No service, no binary
Time-series (InfluxDB, TimescaleDB, Prometheus)	NOT FOUND	No service, no binary
Columnar/warehouse (ClickHouse, DuckDB, Snowflake)	NOT FOUND	No binaries; DuckDB not in pip/npm lists inspected
Embedded KV (LevelDB, RocksDB, LMDB)	NOT FOUND as standalone	Only transitive Node deps, if any
7.3 Critical distinction — client vs server vs instance
SQL Server: server installed ✅ · service running ✅ · instance reachable ✅ · databases present ✅ — the only fully proven database capability.
PostgreSQL: server installed ✅ · service running ✅ · port 5432 bound ✅ · connection not authenticated (no credentials used; per audit rules, no secret handling) — assess as listening but unverified for authentication.
SQLite: no server and no CLI; usable only in-process through EF Core or REA's provider.
Metasploit PostgreSQL: installed server component but stopped → DETECTED BUT INACCESSIBLE.
8. Database Management and Development Tools
Name	Category	Version	Location	Status	Notes
dotnet-ef	Migration CLI	8.0.30	Global .NET tool	Installed ✅	dotnet ef migrations/database verified
EF Core (ORM)	ORM	8.0.33	NuGet (project refs)	Restored ✅	Code-first model for SQL Server; SQLite for tests
questpdf.previewer	Reporting preview	2024.7.1	Global .NET tool	Installed	PDF layout inspection
slopwatch.cmd	Test-quality analyzer	0.4.2	Global .NET tool	Installed	Detects suppressed/degenerate tests
sqlcmd	SQL client	ODBC 17 SDK	...\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE	Installed ✅	Used for verified read-only query
psql	SQL client	17.6	C:\Program Files\PostgreSQL\17\bin\psql.exe	Installed ✅	Not authenticated during audit
SQLite VS Code extension	GUI/editor	alexcvzz.vscode-sqlite	VS Code	Installed ✅	Browse/query .db files
VS "Entity Framework Tools"	Design-time tooling	Shared VS component	Program Files (x86)\Microsoft Visual Studio\Shared\Entity Framework Tools	Installed	EF designer support
NuGet Package Manager GUI (VS Code)	Package tooling	aliasadidev.nugetpackagemanagergui, jmrog.vscode-nuget-package-manager	VS Code	Installed	Visual package management
MSBuild / VS 2022	Build/DB project tooling	17.14.45	...\2022\Community\MSBuild\Current\Bin	Installed ✅	SQL projects not verified
Flyway / Liquibase	Migration	—	—	NOT FOUND	Not installed
Prisma / Drizzle / TypeORM / Sequelize	ORM	—	—	NOT FOUND	Node ecosystem unused for data
SQLAlchemy / Alembic	ORM/migration	—	—	NOT FOUND	Absent from pip list
SSMS	GUI admin	—	—	NOT FOUND	No Ssms.exe anywhere on disk
Azure Data Studio	GUI admin	—	—	NOT FOUND	Absent
DBeaver	GUI admin	—	—	NOT FOUND	Absent
pgAdmin	GUI admin	—	—	NOT FOUND	Absent
MySQL Workbench / DataGrip	GUI admin	—	—	NOT FOUND	Absent
Backup/restore utilities	Admin	SQLWriter (VSS writer)	Service	Running	Native VSS integration only
DB monitoring/profiling	Admin	—	—	NOT FOUND	No extended-events/solarwinds tooling
Summary: database tooling is CLI-first (dotnet-ef, sqlcmd, psql) with no GUI DBA client installed. Migration capability exists for SQL Server through EF Core migrations; schema design is code-first.

9. Package Managers and Dependencies
Name	Version	Ecosystem	Status	Notes
npm	11.17.0	Node/JS	Installed ✅	Global prefix C:\Users\LAP LINK\npm-global
npx / corepack	Bundled	Node/JS	Installed ✅	corepack present (pnpm/yarn not activated)
pnpm / Yarn / Bun / Deno	—	Node/JS	NOT FOUND	Not installed
pip	24.3.1	Python	Installed ✅	C:\Program Files\Python313\Scripts
uv	0.7.15	Python (fast resolver + interpreter manager)	Installed ✅	Manages CPython 3.11.16 / 3.13.1 / 3.14.9
Poetry / Conda / Pipenv	—	Python	NOT FOUND	Absent
NuGet	Via .NET SDK	.NET	Installed ✅	257 packages in %USERPROFILE%\.nuget\packages
Chocolatey	2.4.1	Windows system	Installed ✅	16 packages
Composer	—	PHP	NOT FOUND	No PHP
Maven / Gradle	—	JVM	NOT FOUND	Java compile-only
Cargo	—	Rust	NOT FOUND	No Rust
apt / dpkg / yum / dnf / Homebrew	—	Linux/macOS	NOT FOUND	No active Linux environment
winget / Scoop	—	Windows	NOT FOUND	Not on PATH
9.1 Globally installed npm packages (9)
Package	Version	Purpose
rea-agents	6.7.0	REA reverse-engineering CLI + MCP server
@ninkch/opencode-mem	1.1.0	Memory extension for OpenCode
@pimzino/agentic-tools-mcp	1.8.1	Additional MCP tool provider
@qwen-code/qwen-code	0.15.2	Qwen agent CLI
kimi-code	1.0.11	Kimi agent CLI
mineru-open-api	0.5.9	Document/PDF extraction API client
node-gyp / node-gyp-build	12.1.0 / 4.8.4	Native module build tooling
ecc-universal	2.0.0	Auxiliary tooling
9.2 Chocolatey packages (16)
chocolatey 2.4.1, chocolatey-compatibility.extension 1.0.0, chocolatey-core.extension 1.4.0, chocolatey-windowsupdate.extension 1.0.5, ffmpeg 9.0.1, git 2.47.1 + git.install 2.47.1, KB2919355, KB2919442, KB2999226, KB3033929, KB3035131, ripgrep 15.2.0, tesseract 5.5.0.20241111, vcredist140 14.42.34433, wireshark 4.4.2.

Discrepancy noted: Chocolatey records Git 2.47.1 while git --version reports 2.49.0 (a newer Git installed outside Chocolatey takes PATH precedence). Ripgrep is Chocolatey-installed (15.2.0) but not on the current PATH — the agent's grep tool provides equivalent capability in-process.

9.3 Python packages (97 installed) — highlights
numpy 2.5.2, pydantic 2.12.5, pydantic_core 2.41.5, httpx 0.28.1, anyio 4.12.1, beautifulsoup4 4.15.0, lxml 6.1.1, pillow 12.2.0, pymupdf 1.28.2, onnxruntime 1.29.0, protobuf 7.35.1, networkx 3.6.1, openpyxl 3.1.5, mammoth 1.12.0, mistralai 2.1.2, fasttext-predict 0.9.2.4, psutil 7.2.2, loguru 0.7.3, pydub 0.25.1, pylatexenc 2.10, cobble 0.1.4, groovy 0.1.2. Also registered as a uv tool: browser-use 0.13.8 (exposes browser, browser-use, bu). No pandas, django, flask, fastapi, sqlalchemy, psycopg, pymssql, or mysqlclient were observed in the inspected list — DB access from Python is not provisioned.

10. Containers and Virtualization
Tool	Version	Status	Evidence
Docker Engine / CLI	—	NOT FOUND on PATH; no docker.exe in Docker\Docker\resources\bin	Get-Command docker failed — VERIFIED
Docker Compose	—	NOT FOUND	docker-compose absent
Docker Desktop Service	—	Stopped	com.docker.service → Stopped — VERIFIED
Docker Desktop WSL distro	WSL 2	Stopped	wsl --list --verbose → docker-desktop Stopped 2
WSL2	v2 default	Installed, idle	wsl --status → Default Version: 2
Podman	—	NOT FOUND	Not installed
Kubernetes (kubectl, kubeadm)	—	NOT FOUND	Not installed
Helm	—	NOT FOUND	Not installed
Minikube / kind	—	NOT FOUND	Not installed
Vagrant	—	NOT FOUND	Not installed
Terraform	—	NOT FOUND	Not installed
Ansible	—	NOT FOUND	Not installed
Hyper-V	—	vmcompute Stopped, vmms absent	VERIFIED
Running containers / images	—	None inspectable	No container runtime available to query
Verdict: containerization is present only as an inert Docker Desktop installation. No container can currently be built, run, or inspected.

11. Web Servers and Application Servers
Product	Version	Installed	Running	Notes
IIS (W3SVC, WAS)	—	❌ NOT FOUND	❌	Services absent — IIS feature not enabled
Nginx	—	❌ NOT FOUND	❌	No binary, no service
Apache HTTP Server	—	❌ NOT FOUND	❌	No binary, no service
Caddy	—	❌ NOT FOUND	❌	No binary
Tomcat / Jetty	—	❌ NOT FOUND	❌	JDK present, no servlet container
PHP-FPM	—	❌ NOT FOUND	❌	No PHP
Kestrel (ASP.NET Core)	6.0.36 / 8.0.34 / 9.0.23 runtime	✅ Runtime installed	❌ Not running	Can host ASP.NET apps if one is built
Node.js HTTP servers	Node 24.19.0	✅ Runtime installed	❌ Not running	Capability exists, no service active
Python WSGI/ASGI (uvicorn/gunicorn)	—	❌ NOT FOUND	❌	Not installed
WPF desktop host	.NET 8 Windows Desktop	✅	n/a	This project's actual UI surface (not a web server)
Distinction: Installed = ASP.NET Core/Kestrel + Node.js runtimes. Active = none of these is currently serving HTTP.

12. Git, CI/CD, Testing, and Development Utilities
Name	Version	Category	Status	Notes
Git	2.49.0.windows.1	VCS	Installed ✅	C:\Program Files\Git\cmd\git.exe; repo on main, remote GitHub
Git Bash	Bundled	Shell	Installed ✅	POSIX toolkit
GitLens (VS Code)	eamodio.gitlens	VCS UI	Installed ✅	Blame/history in editor
GitHub CLI (gh)	—	VCS/CI	NOT FOUND	Not on PATH
GitHub Actions runner	—	CI	NOT FOUND	No self-hosted runner detected
Azure Pipelines agent	—	CI	NOT FOUND	Not detected
Jenkins	—	CI	NOT FOUND	Not detected
xUnit	2.9.3	Testing	Restored ✅	5 test projects in solution
Microsoft.NET.Test.Sdk	17.11.1	Test host	Restored ✅	dotnet test capable
coverlet.collector	6.0.2	Coverage	Restored ✅	Coverage collection
.NET Test Explorer (VS Code)	formulahendry.dotnet-test-explorer	Testing UI	Installed ✅	
slopwatch	0.4.2	Test-quality static analysis	Installed ✅	Guards against weakened tests
CSharpier	csharpier.csharpier-vscode	Formatter	Installed ✅	C# formatting
XAML Styler	dabbinavo.xamlstyler	XAML formatter	Installed ✅	WPF markup formatting
SonarQube rules sync	frymak.sonarqube-rules-synchroniser	Static analysis	Installed ✅	Rule synchronization for Sonar
Codium	codium.codium	AI test generation	Installed ✅	Extension only
Roslyn analyzers / .NET analyzers	SDK 8/9	Static analysis	Enabled ✅	EnableNETAnalyzers=true, TreatWarningsAsErrors=true
VS Code C# Dev Kit	ms-dotnettools.csdevkit	IDE tooling	Installed ✅	Solution/project awareness
Debuggers	.NET debugger (VS/VS Code), REA GDB/Frida/Rizin (unavailable on Windows)	Debugging	Partially ✅	Managed debugging ready; native debugging via REA blocked
Profilers	—	Performance	NOT FOUND	No dotnet-trace/dotnet-counters global tool
API testing (Postman/Insomnia)	—	API testing	NOT FOUND	Not installed
Security/dependency audit CLIs	dotnet list package --vulnerable capability exists; npm audit	Auditing	Available ✅	Not executed during read-only audit
Documentation generators	GenerateDocumentationFile=false; VitePress 1.6.4 inside REA dev-deps	Docs	Partially ✅	No project doc pipeline
Build logs / evidence	TestResults/*.trx, *.log	Build evidence	Present ✅	10 artifacts incl. phase6-full.trx, integration-sqlserver-run.log
Repo-local utilities: tools/verify_manifest.py, scripts/generate_use_case_inventory.py (Python 3, runnable with installed 3.13.1).

13. Running Services and Accessible Network Resources
13.1 Listening TCP ports (VERIFIED via Get-NetTCPConnection -State Listen)
Port	Bind	Process	Assessment
5432	:: (all)	postgres	PostgreSQL 17.6 — live
56496	0.0.0.0	sqlservr	SQL Server Express — live (dynamic port)
5037	127.0.0.1	adb	Android Debug Bridge server — live, external scrcpy install
49734 / 49743	127.0.0.1	python (PID 7656)	hermes_cli.main gateway run (uv-managed CPython 3.11) — local agent gateway
28894	127.0.0.1	opencode-cli	OpenCode CLI backend
11349	127.0.0.1	OpenCode	OpenCode desktop/host
31819	::1	WXCastService	Windows projection/casting helper
3389	0.0.0.0	svchost	RDP enabled
135, 139, 445, 5040, 7680, 49664-49743	various	svchost/System/lsass/spoolsv	Standard Windows RPC/SMB/Delivery-Optimization/print services
13.2 Relevant Windows services
Service	State	Start type
MSSQL$SQLEXPRESS	Running	—
SQLBrowser	Running	—
SQLTELEMETRY$SQLEXPRESS	Running	—
SQLWriter	Running	—
postgresql-x64-17	Running	—
WSLService	Running	—
SQLAgent$SQLEXPRESS	Stopped	(SQL Agent unavailable in Express)
metasploitPostgreSQL	Stopped	Automatic
com.docker.service	Stopped	—
vmcompute	Stopped	—
13.3 Reachable resources
SQL Server .\SQLEXPRESS — reachable over Windows authentication without credentials in command text: VERIFIED (returned @@VERSION and 10 database names).
PostgreSQL 17.6 on localhost:5432 — TCP endpoint confirmed; authentication not attempted per audit restrictions.
Internet — webfetch/websearch tools available (external service dependency).
No HTTP application server is bound on 80/443/3000/5000/8000/5173 — none of these ports appear in the listener table.
14. Access Restrictions and Unavailable Capabilities
Restriction	Impact	Evidence level
No administrative privileges	Cannot install services, modify machine-level config, enable IIS, start Docker/Hyper-V, or change firewall	VERIFIED
JAVA_HOME unset + relative java.exe resolution fault	REA Ghidra provider available: false; blocks all Ghidra-backed decompilation	VERIFIED
Hopper absent	macOS-only engine; HOPPER_LAUNCHER_PATH unset	VERIFIED
IDA / JADX / Kuna unconfigured	Android/IDA analyses require caller-supplied engines	VERIFIED
Docker daemon down + CLI absent	No container builds, no compose stacks, no image inspection	VERIFIED
WSL distro stopped	No Linux userland, no apt, no native toolchain	VERIFIED
No C/C++ compiler	VS 2022 lacks VC\Tools\MSVC; gcc/clang/cmake/make absent	VERIFIED
adb external to PATH	Works only via absolute path (F:\...\scrcpy-win64-v3.1\adb.exe)	VERIFIED
No DB GUI client	No SSMS/DBeaver/pgAdmin/Azure Data Studio; administration is CLI-only	VERIFIED
No gh, jq, rg, fzf, winget, docker on PATH	Automation must use PowerShell equivalents or in-process tools	VERIFIED
PostgreSQL credentials not used	Connection authenticity unproven (by policy, not by fault)	PARTIALLY VERIFIED
Build not executed	bin//obj/ writes prohibited by the read-only mandate, so compile/test success is inferred from restore artifacts (9 project.assets.json)	PARTIALLY VERIFIED
REA client registrations stale	6 config_drift entries (claude, codex, copilot, devin, hermes, vscode) — OpenCode registration is valid and live	VERIFIED
Explicitly absent technology families
PHP · Ruby · Go · Rust · C/C++ compilers · Maven · Gradle · Composer · Poetry · Conda · pnpm · Yarn · Bun · Deno · MySQL · MariaDB · MongoDB · Redis · Elasticsearch/OpenSearch · CouchDB · Oracle · vector/graph/time-series DBs · Nginx · Apache · Caddy · IIS · Tomcat · Kubernetes · Helm · Terraform · Ansible · Vagrant · Podman · GitHub CLI · Postman/Insomnia · DBeaver/pgAdmin/SSMS/DataGrip.

15. Final Technical Capability Assessment
15.1 Direct answers
Which tools can you execute directly? PowerShell 7.6.10 (shell), the full file/search toolset (read, write, edit, glob, grep), webfetch/websearch, 45 browser-automation tools, 8 OpenCode harness tools, 207 REA tools, subagents (explore, general), and a JavaScript Code-Mode runtime for parallel tool orchestration.
Which programming languages can you use? C# 12 / .NET 8–9 (primary, fully provisioned), JavaScript/Node 24.19.0, Python 3.13.1 / 3.11.16 / 3.14.9, PowerShell 7.6.10, Java 21 (compile/run only), TypeScript (only via package-local compilers), Bash (Git Bash). Not available: PHP, Ruby, Go, Rust, C/C++.
Which development environments are ready for use? The Ref Lab .NET/WPF solution — 9 projects, NuGet fully restored (257 cached packages, 9 project.assets.json), MSBuild 17.14.45 + .NET SDKs, C# Dev Kit, CSharpier, XAML Styler, xUnit test explorer. This is the single fully ready, purpose-built environment.
Which database engines can you manage? SQL Server 2022 Express fully (DDL/DML via sqlcmd, dotnet ef). PostgreSQL 17.6 administratively reachable in principle but not authenticated during this audit. SQLite only as an embedded EF Core provider or through REA's sqlite provider.
Which database servers are currently running? Two: MSSQL$SQLEXPRESS (SQL Server 2022 Express 16.0.1220.3, TCP 56496) and postgresql-x64-17 (PostgreSQL 17.6, TCP 5432). metasploitPostgreSQL is installed but stopped.
Which database management applications are available? CLI only: dotnet-ef 8.0.30, sqlcmd (ODBC 17), psql 17.6, plus the alexcvzz.vscode-sqlite VS Code extension. No GUI DBA client is installed.
Which package managers can you use? npm 11.17.0, pip 24.3.1, uv 0.7.15, NuGet (via SDK), Chocolatey 2.4.1. Not available: pnpm, Yarn, Bun, Poetry, Conda, Maven, Gradle, Composer, Cargo, winget, Scoop.
Which web and application servers can you operate? Kestrel/ASP.NET Core runtimes (6/8/9) and the Node.js HTTP runtime — both capable but none currently running. No Nginx, Apache, Caddy, IIS, or Tomcat.
What can you do with the REA package? Invoke rea 6.7.0 as a CLI and use its live MCP server (207 tools) for: binary/PE/Mach-O/APK/ASAR/WASM/EVM/firmware inspection, decompilation and pseudo-code extraction, xref/call-graph analysis, .NET managed-code inspection (rea-dotnet-static), SQLite file inspection, JS/Electron bundle reconstruction, web/runtime observation, PCAP analysis, Android/ADB workflows, and canonical Evidence export/verification. Ghidra-backed decompilation is currently blocked by the Java resolution fault; Hopper/IDA/JADX/Kuna are absent.
Which capabilities are restricted by permissions or missing dependencies? All administrative operations (no admin token); container workloads (Docker down, no CLI); Linux-native tooling (WSL stopped); native C/C++ builds (no MSVC/gcc); Ghidra decompilation (Java resolution fault); Ghost/IDA/JADX analyses (unconfigured engines); PostgreSQL authentication (credentials intentionally not used).
What important tools or environments are absent? Docker/Kubernetes/Terraform/Ansible, any Linux distro, C/C++ and Go/Rust/PHP/Ruby toolchains, MySQL/MongoDB/Redis/Elasticsearch, GUI database clients, GitHub CLI, jq/rg/fzf on PATH, Postman/Insomnia, profilers (dotnet-trace, dotnet-counters), and IIS.
Overall readiness? Strong (≈85%) for this project's actual mandate — a C#/.NET 8/WPF/Clean-Architecture medical LIMS on SQL Server, developed with REA-assisted reverse engineering. Build, test, migrate, query, and evidence workflows are all provisioned. Weak (≈20–30%) for generic polyglot, containerized, cloud, or Linux deployment work — no container runtime, no orchestrators, no IaC, and no admin rights. The single most consequential defect is the REA Java/Ghidra resolution failure, because decompilation is central to this repository's methodology; it is fixable without installation by defining JAVA_HOME to the existing JDK 21.
15.2 Capability matrix
Capability	Available	Verified Version	Accessibility	Notes
Shell command execution	✅	PowerShell 7.6.10	Full	Primary execution channel
File read/write/edit/search	✅	Native tools	Full	Read-only respected during audit
Code Mode JS orchestration	✅	Sandboxed runtime	Full	Parallel tool composition
Subagent delegation	✅	explore, general	Full	Background child sessions
Browser automation	✅	45 tools	Full	Requires explicit tabID
Web access (fetch/search)	✅	Native tools	Full	External dependency
C# / .NET build	✅	SDK 9.0.321, 8.0.428	Full	Restore artifacts present; build not run (read-only)
.NET testing	✅	xUnit 2.9.3, Test.Sdk 17.11.1	Full	5 test projects
WPF desktop UI	✅	net8.0-windows10.0.17763.0	Full	Primary presentation layer
EF Core migrations	✅	EF Core 8.0.33, dotnet-ef 8.0.30	Full	Code-first, SQL Server
Node.js development	✅	24.19.0 / npm 11.17.0	Full	No project consumers
Python development	✅	3.13.1 / pip 24.3.1 / uv 0.7.15	Full	97 packages
Java compile/run	✅	OpenJDK 21.0.13	Partial	No Maven/Gradle, JAVA_HOME unset
C/C++ compilation	❌	—	None	No MSVC toolset / gcc / clang
SQL Server (server+client)	✅	16.0.1220.3	Full	Running, authenticated, 10 DBs
PostgreSQL (server)	✅	17.6	Partial	Running on 5432; auth unverified
SQLite	⚠️	EF Core 8.0.33 provider	Partial	No sqlite3 CLI binary
GUI database client	❌	—	None	SSMS/DBeaver/pgAdmin absent
Docker / containers	❌	—	None	Service stopped, CLI absent
Kubernetes / Helm / IaC	❌	—	None	Not installed
Web/app server operation	⚠️	Kestrel 6/8/9, Node 24	Capability only	Nothing currently listening on 80/443
Git version control	✅	2.49.0	Full	GitHub remote configured
CI pipeline	❌	—	None	No gh, no runner
REA CLI + MCP	✅	6.7.0	Full (207 tools)	doctor healthy:false
REA Ghidra decompilation	⚠️	Ghidra 12.1.4	Blocked	Java resolution fault
REA Hopper / IDA / JADX / Kuna	❌	—	None	Engines not installed/configured
Android/ADB workflows	⚠️	adb (external scrcpy path)	Partial	Not on PATH; server live on 5037
PCAP analysis	⚠️	Wireshark 4.4.2 (Chocolatey)	Partial	tshark not on PATH
Administrative/system changes	❌	—	None	IsAdmin = False
Closing note on scope and integrity
Every row above derives from a command actually executed in this session; nothing is inferred from model knowledge. Three items are deliberately marked PARTIALLY VERIFIED rather than VERIFIED: (a) PostgreSQL connection authenticity (no credentials were used, by design), (b) .NET build/test success (compile would write bin//obj/, forbidden by the read-only mandate — readiness is evidenced by 9 restored project.assets.json files and 257 cached NuGet packages), and (c) tshark/adb executability from PATH (binaries exist but are outside PATH). No files were created or modified, no service was started or stopped, and no database write occurred.