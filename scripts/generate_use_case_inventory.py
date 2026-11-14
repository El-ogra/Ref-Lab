#!/usr/bin/env python3
"""Generates the Ref Lab use-case inventory by scanning the Application layer.

The inventory is derived from the compiled source, never hand-maintained, so it
cannot drift from the implementation. Run it from the repository root:

    python scripts/generate_use_case_inventory.py

It writes Docs/USE_CASE_INVENTORY.md and exits non-zero when the number of
use cases is not the expected total.

The output directory is `Docs` with a capital D — the same casing as the
repository folder. An earlier revision of this script wrote to a lowercase
`docs`, which is invisible on a case-insensitive Windows file system but
creates a *second* folder on a case-sensitive one, leaving the tracked
document untouched. Do not reintroduce that.
"""
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
FEATURES = os.path.join(ROOT, "src", "RefLab.Application", "Features")
VALIDATION = os.path.join(ROOT, "src", "RefLab.Application", "Validation")
TESTS = os.path.join(ROOT, "tests", "RefLab.Application.Tests", "Features")

EXPECTED_TOTAL = 73

# The legacy requirement each Ref Lab use case satisfies. Several legacy
# requirements are split into more than one use case; the SMS and NATIGH.COM
# requirements were removed from scope by the project owner.
LEGACY = {
    "SignIn": "V2-REQ-004", "ChangePassword": "V2-REQ-033",
    "CreateOperator": "V2-REQ-032", "UpdateOperator": "V2-REQ-032",
    "AssignOperatorPermissions": "V2-REQ-032", "GetOperators": "V2-REQ-032",
    "RegisterPatient": "V2-REQ-007", "AllocateOrderIdentifier": "V2-REQ-008",
    "AttachOrderIdentifier": "V2-REQ-008",
    "GeneratePatientBarcode": "V2-REQ-009", "SearchPatients": "V2-REQ-010",
    "GetPatientOrder": "V2-REQ-011", "GetPatientHistory": "V2-REQ-011",
    "AttachPatientPicture": "V2-REQ-038", "FollowUpCase": "V2-REQ-071",
    "OrderTests": "V2-REQ-007", "HandOverResults": "V2-REQ-018", "GetTestProfile": "V2-REQ-012",
    "EnterResult": "V2-REQ-013", "EnterAdditionalResult": "V2-REQ-013",
    "ReviewResult": "V2-REQ-017", "GetMethodSheet": "V2-REQ-039",
    "GetCriticalValues": "V2-REQ-039",
    "DeriveHematocrit": "V2-REQ-015", "DeriveHemoglobinPercentage": "V2-REQ-015",
    "DeriveInternationalSensitivityIndex": "V2-REQ-015",
    "InterpolateProthrombinControl": "V2-REQ-015",
    "EnterDerivedRatio": "V2-REQ-015",
    "GetTestCatalogue": "V2-REQ-020", "CreateTestDefinition": "V2-REQ-020",
    "UpdateTestDefinition": "V2-REQ-020", "CreateReferenceRange": "V2-REQ-021",
    "UpdateReferenceRange": "V2-REQ-021", "GetReferenceRangeForPatient": "V2-REQ-022",
    "ManageUnitsAndConversions": "V2-REQ-029", "ManageAbbreviations": "V2-REQ-030",
    "ManageClinicalSubGroups": "V2-REQ-031",
    "CreateTestGroup": "V2-REQ-031",
    "PrintResultReport": "V2-REQ-041, V2-REQ-043", "GetPrintAuditTrail": "V2-REQ-041",
    "ProduceConsolidatedReport": "V2-REQ-062", "ProduceEmptyReport": "V2-REQ-063",
    "PrintPatientInvoice": "V2-REQ-042", "PrintPatientIdCard": "V2-REQ-042",
    "CustomiseReportLayout": "V2-REQ-023, V2-REQ-036",
    "ConfigurePrinters": "V2-REQ-034",
    "ConfigureReportHeaderFooter": "V2-REQ-035, V2-REQ-070",
    "ConfigureConnectionReceipt": "V2-REQ-067",
    "ConfigureLaboratorySettings": "V2-REQ-035",
    "RecordPayment": "V2-REQ-018", "GetPayments": "V2-REQ-025",
    "ComputeCommission": "V2-REQ-019", "RunPeriodStockTake": "V2-REQ-025",
    "RunStockTakeByPhysicianOrParty": "V2-REQ-027",
    "SetDefaultAccountType": "V2-REQ-066",
    "GetReferringParties": "V2-REQ-037", "ManageTreatingPhysicians": "V2-REQ-037",
    "ManagePriceLists": "V2-REQ-064", "PrintPriceList": "V2-REQ-065",
    "CreateSendOutRequest": "V2-REQ-024", "DispatchSendOutSample": "V2-REQ-024",
    "SettleSendOutSample": "V2-REQ-024", "GetSendOutQueue": "V2-REQ-024",
    "SplitOrWithdrawSample": "V2-REQ-061",
    "RecordCultureResult": "V2-REQ-016", "GetCultureResults": "V2-REQ-016",
    "ManageAntibioticCatalogue": "V2-REQ-060", "ManageFarmResults": "V2-REQ-059",
    "GetLaboratoryStatistics": "V2-REQ-054", "GetPerformanceStatistics": "V2-REQ-055",
    "BuildPatientGroupWorkSheet": "V2-REQ-057", "BuildTestGroupWorkSheet": "V2-REQ-058",
    "RecordAttendance": "V2-REQ-056",
}

RECORD = re.compile(r"public sealed record (\w+)(Command|Query)")
HANDLER = re.compile(r"public sealed class (\w+)Handler\s*:\s*IRequestHandler<(\w+)(?:Command|Query),\s*([\w<>,\s\.]+)>")
PERMISSION = re.compile(r"public Permission Permission => Permission\.(\w+);")



def extract_generic(text, open_index):
    """Extracts the type argument that starts at open_index, balancing the angle brackets.

    A regular expression cannot do this: `IRequest<IReadOnlyList<OrderTestDto>>`
    ends in two closing brackets, and the greedy and lazy forms each get it
    wrong in a different way.
    """
    depth = 1
    index = open_index

    while index < len(text) and depth > 0:
        if text[index] == '<':
            depth += 1
        elif text[index] == '>':
            depth -= 1
            if depth == 0:
                break
        index += 1

    return text[open_index:index].strip()


def parse_record(content, record_match):
    """Returns the parameter list and the response type of a request record."""
    params = ""
    response = ""

    cursor = record_match.end()

    while cursor < len(content):
        character = content[cursor]

        if character == '(':
            depth = 1
            index = cursor + 1
            while index < len(content) and depth > 0:
                if content[index] == '(':
                    depth += 1
                elif content[index] == ')':
                    depth -= 1
                    if depth == 0:
                        break
                index += 1
            params = content[cursor + 1:index]
            cursor = index + 1
            continue

        if content.startswith("IRequest<", cursor):
            response = extract_generic(content, cursor + len("IRequest<"))
            break

        cursor += 1

    return " ".join(params.split()), response


def read(path):
    with open(path, encoding="utf-8") as handle:
        return handle.read()


def collect_validators():
    found = set()
    if not os.path.isdir(VALIDATION):
        return found
    for name in os.listdir(VALIDATION):
        if not name.endswith(".cs"):
            continue
        # The capture group is the base name; the literal suffix is outside it.
        for match in re.finditer(r"public sealed class (\w+)Validator", read(os.path.join(VALIDATION, name))):
            found.add(match.group(1))
    return found


def collect_test_coverage():
    """Maps a use-case name to the test file that mentions it."""
    coverage = {}
    for base, _, files in os.walk(TESTS):
        for name in files:
            if not name.endswith(".cs"):
                continue
            content = read(os.path.join(base, name))
            for match in re.finditer(r"\b(\w+)(?:Command|Query)\b", content):
                coverage.setdefault(match.group(1), set()).add(
                    os.path.relpath(os.path.join(base, name), ROOT).replace("\\", "/"))
    return coverage


def summarise():
    """Returns the inventory entries and the totals, so other tools never hard-code them."""
    validators = collect_validators()
    coverage = collect_test_coverage()

    entries = []
    for area in sorted(os.listdir(FEATURES)):
        area_dir = os.path.join(FEATURES, area)
        if not os.path.isdir(area_dir):
            continue
        for name in sorted(os.listdir(area_dir)):
            if not name.endswith(".cs") or name == "Dtos.cs":
                continue
            content = read(os.path.join(area_dir, name))
            record = RECORD.search(content)
            handler = HANDLER.search(content)
            if record is None or handler is None:
                continue
            base_name = record.group(1)
            request_name = base_name + record.group(2)
            params, response = parse_record(content, record)
            entries.append({
                "area": area,
                "name": request_name,
                "kind": record.group(2),
                "params": params,
                "response": response,
                "handler": handler.group(1) + "Handler",
                "permission": (PERMISSION.search(content).group(1)
                               if PERMISSION.search(content) else "—"),
                "validator": base_name + "Validator" if base_name in validators else "—",
                "tests": sorted(coverage.get(base_name, [])),
                "legacy": LEGACY.get(base_name, "—"),
            })

    entries.sort(key=lambda item: (item["area"], item["name"]))

    totals = {
        "useCases": len(entries),
        "commands": sum(1 for e in entries if e["kind"] == "Command"),
        "queries": sum(1 for e in entries if e["kind"] == "Query"),
        "withValidator": sum(1 for e in entries if e["validator"] != "—"),
        "withPermission": sum(1 for e in entries if e["permission"] != "—"),
        "withTest": sum(1 for e in entries if e["tests"]),
        "areas": len(set(e["area"] for e in entries)),
    }
    return entries, totals


def main():
    validators = collect_validators()
    coverage = collect_test_coverage()

    entries = []
    for area in sorted(os.listdir(FEATURES)):
        area_dir = os.path.join(FEATURES, area)
        if not os.path.isdir(area_dir):
            continue
        for name in sorted(os.listdir(area_dir)):
            if not name.endswith(".cs") or name == "Dtos.cs":
                continue
            content = read(os.path.join(area_dir, name))
            record = RECORD.search(content)
            handler = HANDLER.search(content)
            if record is None or handler is None:
                continue
            base_name = record.group(1)
            request_name = base_name + record.group(2)
            params, response = parse_record(content, record)
            entries.append({
                "area": area,
                "name": request_name,
                "kind": record.group(2),
                "params": params,
                "response": response,
                "handler": handler.group(1) + "Handler",
                "permission": (PERMISSION.search(content).group(1)
                               if PERMISSION.search(content) else "—"),
                "validator": base_name + "Validator" if base_name in validators else "—",
                "tests": sorted(coverage.get(base_name, [])),
                "legacy": LEGACY.get(base_name, "—"),
            })

    entries.sort(key=lambda item: (item["area"], item["name"]))

    lines = []
    lines.append("# Ref Lab — Use Case Inventory\n")
    lines.append("**Document ID:** `RL-USE-CASE-INVENTORY`  ")
    lines.append("**Generated by:** `scripts/generate_use_case_inventory.py`, scanning the compiled source.  ")
    lines.append("**Do not edit by hand** — regenerate it instead.\n")
    lines.append("---\n")
    lines.append("## 1. Totals\n")
    lines.append("| Metric | Value |")
    lines.append("|---|---:|")
    lines.append(f"| Use cases implemented | **{len(entries)}** |")
    lines.append(f"| Commands | {sum(1 for e in entries if e['kind'] == 'Command')} |")
    lines.append(f"| Queries | {sum(1 for e in entries if e['kind'] == 'Query')} |")
    lines.append(f"| With a FluentValidation validator | {sum(1 for e in entries if e['validator'] != '—')} |")
    lines.append(f"| Requiring a permission | {sum(1 for e in entries if e['permission'] != '—')} |")
    lines.append(f"| Referenced by a test | {sum(1 for e in entries if e['tests'])} |")
    lines.append(f"| Feature areas | {len(set(e['area'] for e in entries))} |")
    lines.append("")

    lines.append("## 2. Inventory\n")
    lines.append("| # | ID | Use case | Area | Kind | Permission | Validator | Legacy requirement |")
    lines.append("|---:|---|---|---|---|---|---|---|")
    for index, entry in enumerate(entries, start=1):
        lines.append(
            f"| {index} | UC-{index:03d} | `{entry['name']}` | {entry['area']} | {entry['kind']} | "
            f"{entry['permission']} | {entry['validator']} | {entry['legacy']} |")
    lines.append("")

    lines.append("## 3. Contracts\n")
    lines.append("| Use case | Request contract | Response contract | Handler |")
    lines.append("|---|---|---|---|")
    for entry in entries:
        params = entry["params"] or "—"
        lines.append(
            f"| `{entry['name']}` | `({params})` | `{entry['response']}` | `{entry['handler']}` |")
    lines.append("")

    lines.append("## 4. Test coverage\n")
    lines.append("| Use case | Test file |")
    lines.append("|---|---|")
    for entry in entries:
        files = "<br>".join(f"`{f}`" for f in entry["tests"]) if entry["tests"] else "—"
        lines.append(f"| `{entry['name']}` | {files} |")
    lines.append("")

    lines.append("## 5. Removed from scope\n")
    lines.append("| Legacy requirement | Feature | Decision |")
    lines.append("|---|---|---|")
    for req in ("V2-REQ-047", "V2-REQ-048", "V2-REQ-072"):
        lines.append(f"| {req} | NATIGH.COM result publication and retrieval | Removed by owner decision |")
    for req in ("V2-REQ-049", "V2-REQ-050"):
        lines.append(f"| {req} | SMS notification, recipients and templates | Removed by owner decision |")
    lines.append("")

    output = os.path.join(ROOT, "Docs", "USE_CASE_INVENTORY.md")
    os.makedirs(os.path.dirname(output), exist_ok=True)
    with open(output, "w", encoding="utf-8") as handle:
        handle.write("\n".join(lines) + "\n")

    print(f"use cases found: {len(entries)} (expected {EXPECTED_TOTAL})")
    print(f"inventory written: {output}")

    if len(entries) != EXPECTED_TOTAL:
        print("FAILURE: the use-case count does not match the expected total.", file=sys.stderr)
        return 1

    unmapped = [e["name"] for e in entries if e["legacy"] == "—"]

    if unmapped:
        print(f"WARNING: no legacy requirement is mapped for {unmapped}", file=sys.stderr)
        return 3

    without_tests = [e["name"] for e in entries if not e["tests"]]
    if without_tests:
        print(f"WARNING: no test references {without_tests}", file=sys.stderr)
        return 2

    return 0


if __name__ == "__main__":
    sys.exit(main())
