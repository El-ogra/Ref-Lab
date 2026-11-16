# Data Access Architecture and SQL Inventory — RealLab v1.605

## Style

**Client-side, string-built ADO access** (no ORM, no parameterisation visible). 1,337 statements extracted (SELECT 1220, DELETE FROM 72, SHAPE 20, OTHER 25). Provider `SQLOLEDB.1` (ADO 2.7) for SQL Server; Jet 4 OLEDB for Access.

## SQL inventory (top tables by statement count)

| Table | Statements |
|---|---:|
| testtable | 352 |
| patientinfo | 307 |
| patienttest | 216 |
| normalrange | 52 |
| comments | 25 |
| gehats | 16 |
| sharekathesab | 15 |
| payment | 13 |
| history | 12 |
| patienttotal | 12 |
| patients | 12 |
| pricelist | 11 |
| phone | 11 |
| subgrouptable | 10 |
| cbc | 10 |

## SHAPE hierarchies (DataReports)

Example: `SHAPE {SELECT subgrouptable.subgroupcode, subgroupname …} APPEND ({SELECT * FROM testtable …} RELATE subgroupcode TO subgroupcode)` — builds parent/child result sets feeding the 66 DataReports.

## Runtime DDL

The executable carries its own schema-migration engine and runs DDL (CREATE TABLE, ALTER) against the clinical DB at start-up (FND-F-004, U-038). This is Class 2 (static evidence).

## Data-access components

- `MSADODC.OCX` (ADO Data Control), `MSDATGRD`/`MSDATLST` (grids), `MSDBRPT.DLL` (DataReports).
- `MSFLXGRD.OCX`, `MSCHRT20.OCX` for tabular/chart rendering.
- `MSCOMM32.OCX` for serial-port analyser I/O.

## Limitations

- Row data was never read (schema-only policy for MDBs); SQL strings are embedded in the binary, not decompiled.
- ~1,370 embedded SELECTs are not fully mapped to issuing objects (UNR-008).