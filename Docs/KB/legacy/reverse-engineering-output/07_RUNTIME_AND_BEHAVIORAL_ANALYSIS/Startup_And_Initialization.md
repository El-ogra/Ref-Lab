# Startup and Initialization — Real Lab System 1.605

Startup: PE entry → MSVBVM60 → VB6 Form_Load → database connection (BASE.ini/UDL) → schema-migration engine (DDL at startup, U-038) → main menu (labmain). Window creation per form. Initialization sequence not runtime-observed (static inference). See System_Behavior.md.