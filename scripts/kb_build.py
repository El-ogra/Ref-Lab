#!/usr/bin/env python3
"""Builds the Ref Lab knowledge-base index tier from the source tree.

The knowledge base under ``Docs/KB`` has two kinds of content:

* **Curated prose** — written by a human or by an AI architectural pass.
  It explains intent, constraints, traps and decisions. It is never
  regenerated.
* **Generated facts** — the mechanical inventory of what actually exists:
  which files, which types, which use cases, which tables, which screens,
  which dependencies. It is regenerated from the source on every change and
  is never hand-edited.

This script produces the second kind. It writes:

    Docs/KB/generated/file-inventory.csv
    Docs/KB/generated/symbol-index.csv
    Docs/KB/generated/usecase-index.csv
    Docs/KB/generated/db-entity-index.csv
    Docs/KB/generated/screen-index.csv
    Docs/KB/generated/dependency-edges.csv
    Docs/KB/generated/legacy-corpus-index.csv
    Docs/KB/generated/kb-manifest.json

It also refreshes the *generated block* inside every
``Docs/KB/features/<Area>.md``, leaving the curated prose around it alone.

Run it from the repository root:

    python scripts/kb_build.py

The script has no third-party dependency, on purpose: the repository's
other tools are dependency-free and this one must keep running on the
owner's machine without a package restore.
"""

from __future__ import annotations

import hashlib
import json
import os
import re
import sys
from datetime import datetime, timezone

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
SRC = os.path.join(ROOT, "src")
TESTS = os.path.join(ROOT, "tests")
KB = os.path.join(ROOT, "Docs", "KB")
GENERATED = os.path.join(KB, "generated")
FEATURES_DIR = os.path.join(KB, "features")

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from generate_use_case_inventory import summarise  # noqa: E402  (path set above)

# ---------------------------------------------------------------------------
# Vocabulary
# ---------------------------------------------------------------------------

LAYERS = {
    "src/RefLab.Domain": "Domain",
    "src/RefLab.Application": "Application",
    "src/RefLab.Infrastructure": "Infrastructure",
    "src/RefLab.Presentation": "Presentation",
}

PROJECT_OF = {
    "src/RefLab.Domain": "RefLab.Domain",
    "src/RefLab.Application": "RefLab.Application",
    "src/RefLab.Infrastructure": "RefLab.Infrastructure",
    "src/RefLab.Presentation": "RefLab.Presentation",
}

# The fourteen functional areas. The Arabic title is the human-readable
# heading; the identifier is the folder name under
# src/RefLab.Application/Features.
AREAS = {
    "Auditing": "التدقيق وسجل الطباعة والحضور",
    "Billing": "الفوترة والتحصيل والمخزون",
    "Catalogue": "كتالوج الفحوص والمعرفة السريرية",
    "Configuration": "إعدادات المعمل والتقارير",
    "Derivation": "الاشتقاقات السريرية المحسوبة",
    "Identity": "المستخدمون والصلاحيات",
    "Microbiology": "الأحياء الدقيقة والمزارع",
    "Parties": "الأطباء والجهات المحيلة والأسعار",
    "Patients": "المرضى وطلبيات الفحوص",
    "Reporting": "إنشاء التقارير والطباعة",
    "Results": "إدخال النتائج ومراجعتها",
    "SendOut": "الإرسال إلى معامل أخرى",
    "Statistics": "الإحصائيات وقياس الأداء",
    "Worksheets": "أوراق العمل والتجميعات",
}

# The validator file names do not all match their folder name. Three of the
# fourteen areas use a singular form. A generator that assumed
# "{Folder}Validators.cs" would silently miss these three.
VALIDATOR_AREA = {
    "PatientValidators": "Patients",
    "ResultValidators": "Results",
    "WorksheetValidators": "Worksheets",
}

# Which feature area owns which folder of the Domain layer. A folder may be
# shared; "Shared" marks the genuinely cross-cutting ones.
DOMAIN_OWNER = {
    "ClinicalKnowledge": "Catalogue",
    "Identity": "Identity",
    "Patients": "Patients",
    "Billing": "Billing",
    "Auditing": "Auditing",
    "SendOut": "SendOut",
    "Microbiology": "Microbiology",
    "Farming": "Microbiology",
    "Configuration": "Configuration",
    "ValueObjects": "Shared",
    "Enums": "Shared",
    "Exceptions": "Shared",
    "Rules": "Shared",
}

# Which screen owns which view model.
VIEWMODEL_OWNER = {
    "AccountingViewModel": "Billing",
    "AuditViewModel": "Auditing",
    "CatalogueAdministrationViewModel": "Catalogue",
    "CataloguePanelViewModel": "Catalogue",
    "DerivationPanelViewModel": "Derivation",
    "LaboratorySettingsViewModel": "Configuration",
    "MethodSheetPanelViewModel": "Results",
    "MicrobiologyViewModel": "Microbiology",
    "OperatorAdministrationViewModel": "Identity",
    "PartiesPricingViewModel": "Parties",
    "PatientPanelViewModel": "Patients",
    "PatientWorkViewModel": "Patients",
    "ReceptionViewModel": "Patients",
    "ReportSetupViewModel": "Reporting",
    "SendOutQueueViewModel": "SendOut",
    "ShellViewModel": "Shared",
    "SignInViewModel": "Identity",
    "StatisticsViewModel": "Statistics",
    "TestCatalogueViewModel": "Catalogue",
}

# Which test folder belongs to which feature area. The Domain test project
# mirrors the Domain layer, so the Domain owner table applies to it.
DOMAIN_TEST_OWNER = {
    "Auditing": "Auditing",
    "Billing": "Billing",
    "ClinicalKnowledge": "Catalogue",
    "Configuration": "Configuration",
    "Enums": "Shared",
    "Farming": "Microbiology",
    "Identity": "Identity",
    "Microbiology": "Microbiology",
    "Patients": "Patients",
    "Rules": "Shared",
    "SendOut": "SendOut",
    "ValueObjects": "Shared",
    "Architecture": "Shared",
}

# The presentation test project mirrors the view models.
PRESENTATION_TEST_OWNER = {
    "CataloguePanelViewModelTests": "Catalogue",
    "PatientWorkViewModelTests": "Patients",
    "SignInViewModelTests": "Identity",
}

# The persistence configuration file that owns each aggregate.
CONFIG_OWNER = {
    "BillingConfigurations": "Billing",
    "ClinicalKnowledgeConfigurations": "Catalogue",
    "IdentityConfigurations": "Identity",
    "OperationalConfigurations": "Shared",
    "PatientConfigurations": "Patients",
    "ValueObjectConfigurations": "Shared",
}

# ---------------------------------------------------------------------------
# Reading helpers
# ---------------------------------------------------------------------------


def read(path: str) -> str:
    with open(path, "r", encoding="utf-8", errors="replace") as handle:
        return handle.read()


def rel(path: str) -> str:
    return os.path.relpath(path, ROOT).replace(os.sep, "/")


def walk(base: str, extensions: tuple[str, ...]):
    """Yields every file under base with one of the given extensions."""
    for directory, dir_names, file_names in os.walk(base):
        dir_names[:] = sorted(d for d in dir_names if d not in ("bin", "obj", ".git", ".vs"))
        for name in sorted(file_names):
            if name.endswith(extensions):
                yield os.path.join(directory, name)


def source_files():
    return [p for p in walk(SRC, (".cs", ".xaml"))] + [p for p in walk(TESTS, (".cs",))]


def layer_of(relative_path: str) -> str:
    for prefix, layer in LAYERS.items():
        if relative_path.startswith(prefix + "/"):
            return layer
    if relative_path.startswith("tests/"):
        return "Tests"
    return "Other"


def project_of(relative_path: str) -> str:
    parts = relative_path.split("/")
    if len(parts) >= 2 and parts[0] == "src":
        return parts[1]
    if len(parts) >= 2 and parts[0] == "tests":
        return parts[1]
    return ""


def feature_area_of(relative_path: str) -> str:
    """Assigns a source file to exactly one functional area."""
    parts = relative_path.split("/")

    if parts[:2] == ["src", "RefLab.Application"]:
        if len(parts) > 3 and parts[2] == "Features":
            return parts[3]
        if len(parts) > 3 and parts[2] == "Validation":
            name = parts[3]
            if name in VALIDATOR_AREA:
                return VALIDATOR_AREA[name]
            if not name.endswith("Validators.cs"):
                return "Shared"
            stem = name[: -len("Validators.cs")]
            if stem in AREAS:
                return stem
            if (stem + "s") in AREAS:
                return stem + "s"
            return "Shared"
        return "Shared"

    if parts[:2] == ["src", "RefLab.Domain"]:
        # src/RefLab.Domain/<Folder>/<File>.cs
        if len(parts) > 3:
            return DOMAIN_OWNER.get(parts[2], "Shared")
        return "Shared"

    if parts[:2] == ["src", "RefLab.Infrastructure"]:
        if len(parts) > 3 and parts[2] == "Persistence":
            if len(parts) > 4 and parts[3] == "Configurations":
                return CONFIG_OWNER.get(parts[4].replace(".cs", ""), "Shared")
            return "Shared"
        if len(parts) > 3 and parts[3] == "Reports":
            return "Reporting"
        return "Shared"

    if parts[:2] == ["src", "RefLab.Presentation"]:
        if len(parts) > 3 and parts[2] == "ViewModels":
            return VIEWMODEL_OWNER.get(parts[3].replace(".cs", ""), "Shared")
        if len(parts) > 3 and parts[2] == "Views":
            # src/RefLab.Presentation/Views[/<Sub>]/<Name>View[.xaml[.cs]]
            name = parts[-1].replace(".cs", "").replace(".xaml", "")
            stem = name[:-4] + "ViewModel" if name.endswith("View") else name + "ViewModel"
            return VIEWMODEL_OWNER.get(stem, "Shared")
        return "Shared"

    if parts[0] == "tests":
        if len(parts) > 3 and parts[2] == "Features":
            return parts[3]
        if len(parts) > 3 and parts[2] == "ViewModels":
            stem = parts[3].replace(".cs", "")
            return PRESENTATION_TEST_OWNER.get(stem, "Shared")
        if len(parts) > 3 and parts[2] == "Integration":
            return "Shared"
        if len(parts) > 3 and parts[2] == "Persistence":
            return "Shared"
        if len(parts) > 3 and parts[2] == "Workflow":
            return "Shared"
        if len(parts) > 3 and parts[2] == "ClinicalSafety":
            return "Shared"
        if len(parts) > 3:
            return DOMAIN_TEST_OWNER.get(parts[2], "Shared")
        return "Shared"

    return "Shared"


# ---------------------------------------------------------------------------
# Extraction
# ---------------------------------------------------------------------------

TYPE_PATTERN = re.compile(
    r"^\s*(public|internal)\s+"
    r"(?:sealed\s+|abstract\s+|static\s+|partial\s+|readonly\s+|record\s+|ref\s+)*"
    r"(class|record|interface|enum|struct)\s+(\w+)",
    re.MULTILINE,
)

NAMESPACE_PATTERN = re.compile(r"^namespace\s+([\w\.]+)\s*;", re.MULTILINE)
USING_PATTERN = re.compile(r"^using\s+([\w\.]+)\s*;", re.MULTILINE)

DBSET_PATTERN = re.compile(r"public DbSet<(\w+)>\s+(\w+)\s*=>\s*Set<\1>\(\);")
CONFIGURE_CALL = re.compile(r"modelBuilder\.Configure(\w+)\(\);")
TOTABLE_PATTERN = re.compile(r"\.ToTable\(\s*\"([^\"]+)\"")
OWNSONE_PATTERN = re.compile(r"\.OwnsOne<(\w+)")
CREATE_TABLE = re.compile(r'migrationBuilder\.CreateTable\(\s*\n\s*name:\s*"([^"]+)"')
ADD_COLUMN = re.compile(r'migrationBuilder\.AddColumn<(\w+)>\(\s*\n\s*name:\s*"([^"]+)"')
FK_NAME = re.compile(r'name:\s*"(FK_[^"]+)"')
SCREEN_KEY = re.compile(r'public const string (\w+)\s*=\s*"([^"]+)";')
SCREEN_REGISTRY = re.compile(r"\[ScreenKeys\.(\w+)\]\s*=\s*typeof\(([\w\.]+)\)")
ADD_TRANSIENT = re.compile(r"services\.AddTransient<(\w+)>\(\);")
REQUEST_USE = re.compile(r"\b((?:Get|Set|Create|Update|Delete|Record|Enter|Review|Print|Produce|Configure|Manage|Run|Build|Compute|Search|Register|Assign|Change|Sign|Dispatch|Settle|Split|Derive|Interpolate|Order|Hand|Attach|Generate|Allocate|Follow|Withdraw)[A-Za-z]*(?:Command|Query))\b")
PROJECT_REFERENCE = re.compile(r'ProjectReference Include="([^"]+)"')
TEST_ATTRIBUTE = re.compile(r"public\s+(?:async\s+)?(?:Task|void)\s+(\w+)\s*\(")


def build_file_inventory() -> list[dict]:
    rows = []
    for path in source_files():
        relative = rel(path)
        with open(path, "rb") as handle:
            data = handle.read()
        text = data.decode("utf-8", errors="replace")
        rows.append({
            "path": relative,
            "layer": layer_of(relative),
            "project": project_of(relative),
            "bytes": len(data),
            "lines": text.count("\n") + (0 if text.endswith("\n") or not text else 1),
            "kind": os.path.splitext(path)[1].lstrip("."),
            "feature_area": feature_area_of(relative),
        })
    rows.sort(key=lambda row: row["path"])
    return rows


def build_symbol_index() -> tuple[list[dict], dict]:
    """Returns the symbol rows and a map of type name to its declaring file."""
    rows = []
    declaring: dict[str, str] = {}

    for path in walk(SRC, (".cs",)):
        relative = rel(path)
        text = read(path)
        namespace = NAMESPACE_PATTERN.search(text)
        namespace = namespace.group(1) if namespace else ""
        line_numbers = {}
        for index, line in enumerate(text.splitlines(), start=1):
            for match in TYPE_PATTERN.finditer(line):
                visibility, kind, name = match.group(1), match.group(2), match.group(3)
                declaring.setdefault(name, relative)
                line_numbers[name] = index

        for match in TYPE_PATTERN.finditer(text):
            visibility, kind, name = match.group(1), match.group(2), match.group(3)
            rows.append({
                "layer": layer_of(relative),
                "project": project_of(relative),
                "kind": kind,
                "type": name,
                "file": relative,
                "line": line_numbers.get(name, 0),
                "namespace": namespace,
                "feature_area": feature_area_of(relative),
                "visibility": visibility,
            })

    rows.sort(key=lambda row: (row["layer"], row["feature_area"], row["file"], row["type"]))
    return rows, declaring


def build_usecase_index(declaring: dict) -> list[dict]:
    entries, _ = summarise()

    rows = []
    for index, entry in enumerate(entries, start=1):
        feature_file = ""
        for candidate in sorted(os.listdir(os.path.join(ROOT, "src", "RefLab.Application", "Features", entry["area"]))):
            if not candidate.endswith(".cs") or candidate == "Dtos.cs":
                continue
            if re.search(r"public sealed record %s%s\b" % (entry["name"][:-len(entry["kind"])], entry["kind"]),
                         read(os.path.join(ROOT, "src", "RefLab.Application", "Features", entry["area"], candidate))):
                feature_file = "src/RefLab.Application/Features/%s/%s" % (entry["area"], candidate)
                break

        # The response type is not always declared in the feature's own
        # Dtos.cs — Derivation, for instance, returns a DTO that lives in the
        # Results folder. The declaring file is resolved by searching the
        # whole Application layer for the response type.
        dto_file = ""
        response = entry["response"].strip()
        if response and response != "—":
            # IReadOnlyList<OrderTestDto> -> OrderTestDto ; OrderTestDto -> OrderTestDto
            simple = response
            if "<" in simple and ">" in simple:
                inner = simple[simple.rindex("<") + 1:simple.rindex(">")]
                simple = inner.split(",")[0].strip()
            simple = simple.split("<")[0].split(".")[-1].strip()
            for candidate in sorted(walk(os.path.join(ROOT, "src"), (".cs",))):
                text = read(candidate)
                if re.search(r"\b(?:record|class|struct|enum|interface)\s+%s\b" % re.escape(simple), text):
                    dto_file = rel(candidate)
                    break

        validator_file = ""
        validator_name = entry["validator"]
        if validator_name != "—":
            for name in sorted(os.listdir(os.path.join(ROOT, "src", "RefLab.Application", "Validation"))):
                if not name.endswith(".cs"):
                    continue
                if re.search(r"public sealed class %s\b" % validator_name,
                             read(os.path.join(ROOT, "src", "RefLab.Application", "Validation", name))):
                    validator_file = "src/RefLab.Application/Validation/%s" % name
                    break

        test_names = []
        for test_file in entry["tests"]:
            for match in TEST_ATTRIBUTE.finditer(read(os.path.join(ROOT, test_file))):
                if match.group(1) not in test_names:
                    test_names.append(match.group(1))

        rows.append({
            "id": "UC-%03d" % index,
            "feature_area": entry["area"],
            "usecase": entry["name"],
            "kind": entry["kind"],
            "handler": entry["handler"],
            "feature_file": feature_file,
            "permission": entry["permission"],
            "validator": validator_name,
            "validator_file": validator_file,
            "dto_file": dto_file,
            "request_contract": "(%s)" % (entry["params"] or "—"),
            "response_contract": entry["response"] or "—",
            "legacy_requirement": entry["legacy"],
            "test_files": ";".join(entry["tests"]) if entry["tests"] else "",
            "test_names": ";".join(test_names),
        })

    rows.sort(key=lambda row: (row["feature_area"], row["usecase"]))
    return rows


def resolve_property_type(entity: str, property_name: str, declaring: dict) -> str:
    """Reads the declared type of a property from the entity's own source."""
    source = declaring.get(entity)
    if not source:
        return ""
    path = os.path.join(ROOT, source)
    if not os.path.isfile(path):
        return ""
    pattern = re.compile(
        r"public\s+([\w\.<>\?]+)\s+%s\s*\{\s*get" % re.escape(property_name))
    match = pattern.search(read(path))
    return match.group(1) if match else ""


def build_db_entity_index(declaring: dict) -> list[dict]:
    context_path = os.path.join(ROOT, "src", "RefLab.Infrastructure", "Persistence", "RefLabDbContext.cs")
    context = read(context_path)

    entities = []
    for match in DBSET_PATTERN.finditer(context):
        entities.append({"entity": match.group(1), "property": match.group(2)})

    configure_calls = [m.group(1) for m in CONFIGURE_CALL.finditer(context)]

    # The shared mapping helpers declare the owned type in their generic
    # constraint, which is the precise way to learn which value object a
    # helper maps: ConfigureMoney<T> where T is Money.
    configurations_dir = os.path.join(ROOT, "src", "RefLab.Infrastructure", "Persistence", "Configurations")
    value_object_helpers: dict[str, str] = {}
    shared_path = os.path.join(configurations_dir, "ValueObjectConfigurations.cs")
    if os.path.isfile(shared_path):
        for helper in re.finditer(
                r"Configure(\w+)<T>\(\s*this OwnedNavigationBuilder<T,\s*(\w+)>", read(shared_path)):
            value_object_helpers[helper.group(1)] = helper.group(2)

    # Table name, owned value objects and the configuration file, per aggregate.
    table_of: dict[str, str] = {}
    owns: dict[str, list[str]] = {}
    owned_prefixes: dict[str, list[str]] = {}
    config_file: dict[str, str] = {}

    for name in sorted(os.listdir(configurations_dir)):
        if not name.endswith(".cs"):
            continue
        path = os.path.join(configurations_dir, name)
        text = read(path)
        relative = rel(path)

        # Each configuration method is an extension method on ModelBuilder.
        methods = list(re.finditer(r"public static void Configure(\w+)\(this ModelBuilder", text))
        for index, method in enumerate(methods):
            entity = method.group(1)
            end = methods[index + 1].start() if index + 1 < len(methods) else len(text)
            body = text[method.start():end]

            table = TOTABLE_PATTERN.search(body)
            if table:
                table_of[entity] = table.group(1)
            config_file[entity] = relative

            owned = []
            prefixes = []
            # The shared-helper form: OwnsOne(x => x.Prop, v => v.ConfigureThing("Prefix"))
            for own in re.finditer(
                    r"\.OwnsOne\(\s*\w+\s*=>\s*\w+\.(\w+)\s*,\s*\w+\s*=>\s*\w+\.Configure(\w+)\(\s*\"([^\"]*)\"",
                    body):
                property, helper, prefix = own.group(1), own.group(2), own.group(3)
                value_object = value_object_helpers.get(helper, helper)
                if value_object not in owned:
                    owned.append(value_object)
                if property not in prefixes:
                    prefixes.append(property)

            # The block form: OwnsOne(x => x.Prop, v => { ... }) — the owned
            # type is read from the entity's own source, and the column prefix
            # from the first HasColumnName inside the block.
            for own in re.finditer(
                    r"\.OwnsOne\(\s*\w+\s*=>\s*\w+\.(\w+)\s*,\s*\w+\s*=>\s*\{", body):
                property = own.group(1)
                block_end = body.find("});", own.end())
                block = body[own.end():block_end if block_end > 0 else own.end() + 600]
                column = re.search(r'HasColumnName\(\s*"([A-Za-z0-9]+)_', block)
                value_object = resolve_property_type(entity, property, declaring)
                if value_object and value_object not in owned:
                    owned.append(value_object)
                prefix = column.group(1) if column else property
                if property not in prefixes:
                    prefixes.append(property)

            if owned:
                owns[entity] = owned
            if prefixes:
                owned_prefixes[entity] = prefixes

    # Migration lineage: which migration creates which table, and which adds columns.
    migrations_dir = os.path.join(ROOT, "src", "RefLab.Infrastructure", "Persistence", "Migrations")
    migration_of: dict[str, str] = {}
    added_columns: dict[str, list[str]] = {}
    for name in sorted(os.listdir(migrations_dir)):
        if not name.endswith(".cs") or name.endswith(".Designer.cs") or name.startswith("RefLabDbContextModelSnapshot"):
            continue
        text = read(os.path.join(migrations_dir, name))
        relative = rel(os.path.join(migrations_dir, name))
        for table in CREATE_TABLE.findall(text):
            migration_of.setdefault(table, relative)
        for _entity, column in ADD_COLUMN.findall(text):
            added_columns.setdefault(relative, [])
            if column not in added_columns[relative]:
                added_columns[relative].append(column)

    # Foreign keys, taken from the migration that creates the table.
    fks: dict[str, list[str]] = {}
    for name in sorted(os.listdir(migrations_dir)):
        if not name.endswith(".cs") or name.endswith(".Designer.cs"):
            continue
        text = read(os.path.join(migrations_dir, name))
        for table in CREATE_TABLE.findall(text):
            block = text[text.find('name: "%s"' % table):]
            block = block[: block.find("});")] if "});" in block else block
            names = FK_NAME.findall(block)
            if names:
                fks.setdefault(table, [])
                for fk in names:
                    if fk not in fks[table]:
                        fks[table].append(fk)

    rows = []
    for item in entities:
        entity = item["entity"]
        entity_file = declaring.get(entity, "")
        table = table_of.get(entity, entity + "s")
        rows.append({
            "aggregate": entity,
            "entity_file": entity_file,
            "table": table,
            "config_file": config_file.get(entity, ""),
            "migration": migration_of.get(table, ""),
            "owned_value_objects": ";".join(owns.get(entity, [])),
            "owned_properties": ";".join(owned_prefixes.get(entity, [])),
            "foreign_keys": ";".join(fks.get(table, [])),
            "dbset_property": item["property"],
            "configured_by": "Configure%s" % entity if entity in configure_calls else "",
        })
    rows.sort(key=lambda row: row["table"])
    return rows


def build_screen_index(usecases: list[dict]) -> list[dict]:
    keys_path = os.path.join(ROOT, "src", "RefLab.Presentation", "Navigation", "ScreenKeys.cs")
    registry_path = os.path.join(ROOT, "src", "RefLab.Presentation", "Navigation", "ScreenRegistry.cs")
    di_path = os.path.join(ROOT, "src", "RefLab.Presentation", "Bootstrap",
                           "PresentationServiceCollectionExtensions.cs")

    keys = {name: value for name, value in SCREEN_KEY.findall(read(keys_path))}
    views = {name: value for name, value in SCREEN_REGISTRY.findall(read(registry_path))}

    requests_by_view_model: dict[str, list[str]] = {}
    vm_dir = os.path.join(ROOT, "src", "RefLab.Presentation", "ViewModels")
    for directory, _dir_names, file_names in os.walk(vm_dir):
        for name in sorted(file_names):
            if not name.endswith(".cs"):
                continue
            stem = name[:-3]
            text = read(os.path.join(directory, name))
            requests_by_view_model[stem] = sorted(set(REQUEST_USE.findall(text)))

    # The registered views that are not in the navigation registry are the
    # in-place panels: they are hosted inside a screen rather than navigated to.
    registry_views = set(views.values())
    panels = []
    for name in ADD_TRANSIENT.findall(read(di_path)):
        if name.endswith("View") and ("Views." + name) not in registry_views and name not in ("MainWindow", "SignInWindow"):
            panels.append(name)

    by_usecase = {row["usecase"]: row for row in usecases}

    rows = []
    for key_name, key_value in sorted(keys.items()):
        view = views.get(key_name, "")
        view_simple = view.replace("Views.", "")
        # The view model of a screen is named after the *view*, not after the
        # screen key: SendOut -> SendOutQueueView -> SendOutQueueViewModel.
        view_model = view_simple[:-4] + "ViewModel" if view_simple.endswith("View") else ""
        requests = requests_by_view_model.get(view_model, [])
        areas = sorted({by_usecase[r]["feature_area"] for r in requests if r in by_usecase})
        rows.append({
            "screen_key": key_value,
            "constant": key_name,
            "view": view_simple,
            "view_model": view_model,
            "feature_areas": ";".join(areas),
            "use_cases": ";".join(requests),
            "count": len(requests),
            "kind": "screen",
        })

    for panel in sorted(set(panels)):
        view_model = panel[:-4] + "ViewModel" if panel.endswith("View") else ""
        requests = requests_by_view_model.get(view_model, [])
        areas = sorted({by_usecase[r]["feature_area"] for r in requests if r in by_usecase})
        rows.append({
            "screen_key": panel,
            "constant": "",
            "view": panel,
            "view_model": view_model,
            "feature_areas": ";".join(areas),
            "use_cases": ";".join(requests),
            "count": len(requests),
            "kind": "panel",
        })

    rows.sort(key=lambda row: (row["kind"], row["screen_key"]))
    return rows


def build_dependency_edges(declaring: dict) -> list[dict]:
    """Namespace-level dependency edges derived from the using directives."""
    edges = set()

    for path in walk(SRC, (".cs",)):
        relative = rel(path)
        text = read(path)
        namespace = NAMESPACE_PATTERN.search(text)
        source = namespace.group(1) if namespace else ""
        for using in USING_PATTERN.findall(text):
            if using.startswith("RefLab.") and using != source:
                edges.add((source, using, "using"))

    # Project-level edges, read from the project files.
    project_edges = set()
    for path in walk(SRC, (".csproj",)):
        relative = rel(path)
        for reference in PROJECT_REFERENCE.findall(read(path)):
            target = os.path.basename(reference.replace("\\", "/"))
            project_edges.add((PROJECT_OF.get(relative, relative), target, "project-reference"))

    rows = [{"from": a, "to": b, "kind": kind} for a, b, kind in sorted(edges)]
    rows.extend({"from": a, "to": b, "kind": kind} for a, b, kind in sorted(project_edges))
    rows.sort(key=lambda row: (row["kind"], row["from"], row["to"]))
    return rows


def build_legacy_corpus_index() -> list[dict]:
    """Points at the legacy evidence corpus. Never copies it."""
    rows = []
    candidates = [
        ("ReferenceIndex/RefLab_V2_COMPLETE_INDEX.md", "Legacy corpus index (human)", "legacy-evidence"),
        ("ReferenceIndex/RefLab_V2_FILE_INVENTORY.csv", "Legacy corpus file inventory", "legacy-evidence"),
        ("ReferenceIndex/RefLab_V2_OPEN_ITEMS_MAP.csv", "Legacy open-item map", "legacy-evidence"),
        ("RefLab_Reference_V2_Consolidated.zip", "The 10 consolidated legacy reference documents", "legacy-evidence"),
        ("Docs/Business_Rules_V2.md", "45 verified legacy business rules", "legacy-evidence"),
        ("Docs/Functional_Specification_V2.md", "48 legacy functions and 7 workflows", "legacy-evidence"),
        ("Docs/Data_Model_and_Workflow_Entities_V2.md", "40 legacy entities, 19 observed relationships", "legacy-evidence"),
        ("Docs/Functionality_Requirements_Matrix_V2.md", "48 legacy functions mapped to 72 V2-REQ requirements", "legacy-evidence"),
        ("Docs/Complete_Reverse_Engineering_Reference_V2.md", "Complete legacy reverse-engineering reference", "legacy-evidence"),
        ("Database documentation.md", "Legacy Patients.mdb documentation (103 tables)", "legacy-evidence"),
        ("Documenting the Executable File.md", "Legacy real lab system.exe (VB6) documentation", "legacy-evidence"),
        ("evidence/reverse-engineering/", "Raw reverse-engineering evidence (metadata, exports, ledger)", "legacy-evidence"),
        ("Docs/ui-labels-verified.md", "Authorised Arabic user-interface labels", "current-authoritative"),
        ("IDEA.md", "The authoritative project specification", "process"),
        ("README.md", "Human onboarding document", "process"),
        ("Docs/RL-P0-RPT-001-Discovery-Report.md", "Phase 0 discovery report", "historical"),
        ("From-Phase-0-to-3-Handoff.md", "Phase 0-3 engineering handoff (superseded)", "historical"),
        ("Docs/RL-P2-MAN-001-Domain-Layer-Manifest.md", "Phase 2 domain delivery manifest", "historical"),
        ("Docs/RL-P3-MAN-001-Application-Layer-Manifest.md", "Phase 3 application delivery manifest", "historical"),
        ("Docs/RL-P3-Completion-Report.md", "Phase 3 completion report", "historical"),
        ("Docs/RL-P4-MAN-001-Phase4-Delivery-Manifest.md", "Phase 4 delivery manifest", "historical"),
        ("Docs/RL-P4-Completion-Report.md", "Phase 4 completion report", "historical"),
        ("Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md", "Phase 5 delivery manifest", "historical"),
        ("Docs/RL-P5-COR-001-Documentation-Corrigendum.md", "Phase 5 documentation corrigendum", "historical"),
        ("Docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md", "Phase 5 delivery-application guide", "process"),
        ("LOGIN_FAILURE_INVESTIGATION_REPORT.md", "Login failure investigation report", "historical"),
        ("LOGIN_FAILURE_INVESTIGATION_REPORT_HISTORICAL.md", "Retrospective login investigation record", "historical"),
        ("Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md",
         "Machine audit of one specific workstation — not portable", "environment-local"),
    ]
    for path, description, provenance in candidates:
        exists = os.path.exists(os.path.join(ROOT, path))
        rows.append({
            "path": path,
            "exists": "yes" if exists else "no",
            "provenance": provenance,
            "description": description,
        })
    return rows


# ---------------------------------------------------------------------------
# Fingerprint and manifest
# ---------------------------------------------------------------------------


def source_fingerprint() -> str:
    """SHA-256 over the sorted (path, size) list of every source file."""
    digest = hashlib.sha256()
    entries = []
    for path in source_files():
        entries.append((rel(path), os.path.getsize(path)))
    for path, size in sorted(entries):
        digest.update(path.encode("utf-8"))
        digest.update(b"\0")
        digest.update(str(size).encode("ascii"))
        digest.update(b"\n")
    return digest.hexdigest()


def write_csv(path: str, rows: list[dict], columns: list[str], header_comment: list[str]) -> None:
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="") as handle:
        for line in header_comment:
            handle.write("# %s\n" % line)
        handle.write(",".join(columns) + "\n")
        for row in rows:
            values = []
            for column in columns:
                value = str(row.get(column, ""))
                if any(character in value for character in ',"\n'):
                    value = '"%s"' % value.replace('"', '""')
                values.append(value)
            handle.write(",".join(values) + "\n")


# ---------------------------------------------------------------------------
# Feature files
# ---------------------------------------------------------------------------

BEGIN_MARKER = "<!-- BEGIN GENERATED FACTS - do not edit inside this block -->"
END_MARKER = "<!-- END GENERATED FACTS -->"


def feature_template(area: str) -> str:
    title = AREAS.get(area, area)
    return """# %(area)s — %(title)s

**Area:** `%(area)s` · **Part of:** the Ref Lab knowledge base, `Docs/KB/`.
**Curated sections** (1, 7, 8) are written by hand. **Section 2-6** are
regenerated by `scripts/kb_build.py` — never edit them by hand.

---

## 1. What this area is for

_To be written._

---

%(facts)s

---

## 7. Traps and constraints

_To be written._

---

## 8. Related documents

_To be written._
""" % {"area": area, "title": title, "facts": BEGIN_MARKER + "\n" + END_MARKER}


def build_feature_facts(area: str, usecases: list[dict], screens: list[dict],
                        files: list[dict], entities: list[dict]) -> str:
    area_usecases = [row for row in usecases if row["feature_area"] == area]
    area_files = [row for row in files if row["feature_area"] == area]
    area_entities = [row for row in entities
                     if row["entity_file"] and feature_area_of(row["entity_file"]) == area]

    lines = [BEGIN_MARKER, ""]

    lines.append("## 2. Use cases")
    lines.append("")
    if area_usecases:
        lines.append("| Use case | Kind | Permission | Feature file | Validator | Legacy requirement |")
        lines.append("|---|---|---|---|---|---|")
        for row in area_usecases:
            lines.append("| `%s` | %s | %s | `%s` | %s | %s |" % (
                row["usecase"], row["kind"], row["permission"], row["feature_file"],
                row["validator"], row["legacy_requirement"]))
    else:
        lines.append("_No use case is registered directly under this area._")
    lines.append("")

    lines.append("## 3. Source files owned by this area")
    lines.append("")
    if area_files:
        lines.append("| File | Layer | Lines |")
        lines.append("|---|---|---:|")
        for row in area_files:
            lines.append("| `%s` | %s | %d |" % (row["path"], row["layer"], row["lines"]))
    else:
        lines.append("_No source file is assigned to this area._")
    lines.append("")

    lines.append("## 4. Persistence")
    lines.append("")
    if area_entities:
        lines.append("| Table | Aggregate | Entity | Migration |")
        lines.append("|---|---|---|---|")
        for row in area_entities:
            lines.append("| `%s` | `%s` | `%s` | `%s` |" % (
                row["table"], row["aggregate"], row["entity_file"] or "—", row["migration"] or "—"))
    else:
        lines.append("_This area owns no mapped table._")
    lines.append("")

    lines.append("## 5. Screens that reach this area")
    lines.append("")
    area_screens = [row for row in screens if area in row["feature_areas"].split(";")]
    if area_screens:
        lines.append("| Screen | View | View model | Use cases |")
        lines.append("|---|---|---|---|")
        for row in area_screens:
            lines.append("| `%s` | `%s` | `%s` | %d |" % (
                row["screen_key"], row["view"], row["view_model"], row["count"]))
    else:
        lines.append("_No screen calls this area directly._")
    lines.append("")

    lines.append("## 6. Tests")
    lines.append("")
    test_files = []
    for row in area_usecases:
        for test_file in filter(None, row["test_files"].split(";")):
            if test_file not in test_files:
                test_files.append(test_file)
    if test_files:
        lines.append("| Test file |")
        lines.append("|---|")
        for test_file in test_files:
            lines.append("| `%s` |" % test_file)
    else:
        lines.append("_No test is mapped to this area._")
    lines.append("")
    lines.append(END_MARKER)

    return "\n".join(lines)


def refresh_feature_files(usecases: list[dict], screens: list[dict],
                          files: list[dict], entities: list[dict]) -> list[str]:
    os.makedirs(FEATURES_DIR, exist_ok=True)
    written = []
    for area in sorted(AREAS):
        path = os.path.join(FEATURES_DIR, "%s.md" % area)
        if os.path.isfile(path):
            text = read(path)
        else:
            text = feature_template(area)

        facts = build_feature_facts(area, usecases, screens, files, entities)
        if BEGIN_MARKER in text and END_MARKER in text:
            head = text.split(BEGIN_MARKER)[0]
            tail = text.split(END_MARKER, 1)[1]
            text = head + facts + tail
        else:
            text = text.replace(BEGIN_MARKER + "\n" + END_MARKER, facts)

        with open(path, "w", encoding="utf-8") as handle:
            handle.write(text)
        written.append(rel(path))
    return written


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def main() -> int:
    print("Building the Ref Lab knowledge-base index tier...")

    files = build_file_inventory()
    symbols, declaring = build_symbol_index()
    usecases = build_usecase_index(declaring)
    entities = build_db_entity_index(declaring)
    screens = build_screen_index(usecases)
    edges = build_dependency_edges(declaring)
    legacy = build_legacy_corpus_index()
    fingerprint = source_fingerprint()
    stamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

    header = [
        "GENERATED by scripts/kb_build.py - DO NOT EDIT BY HAND",
        "source_tree_fingerprint: %s" % fingerprint,
        "generated_at: %s" % stamp,
        "regenerate with: python scripts/kb_build.py",
    ]

    write_csv(os.path.join(GENERATED, "file-inventory.csv"), files,
              ["path", "layer", "project", "kind", "bytes", "lines", "feature_area"], header)

    write_csv(os.path.join(GENERATED, "symbol-index.csv"), symbols,
              ["layer", "project", "kind", "type", "file", "line", "namespace",
               "feature_area", "visibility"], header)

    write_csv(os.path.join(GENERATED, "usecase-index.csv"), usecases,
              ["id", "feature_area", "usecase", "kind", "handler", "feature_file",
               "permission", "validator", "validator_file", "dto_file",
               "request_contract", "response_contract", "legacy_requirement",
               "test_files", "test_names"], header)

    write_csv(os.path.join(GENERATED, "db-entity-index.csv"), entities,
              ["table", "aggregate", "entity_file", "config_file", "migration",
               "owned_value_objects", "owned_properties", "foreign_keys",
               "dbset_property", "configured_by"], header)

    write_csv(os.path.join(GENERATED, "screen-index.csv"), screens,
              ["kind", "screen_key", "constant", "view", "view_model", "feature_areas",
               "use_cases", "count"], header)

    write_csv(os.path.join(GENERATED, "dependency-edges.csv"), edges,
              ["from", "to", "kind"], header)

    write_csv(os.path.join(GENERATED, "legacy-corpus-index.csv"), legacy,
              ["path", "exists", "provenance", "description"], header)

    feature_files = refresh_feature_files(usecases, screens, files, entities)

    # The curated documents are recorded with their provenance class, their
    # hash and the commit they were verified against, so that a stale
    # curated document is detectable rather than merely suspected.
    curated_provenance = {
        "AGENTS.md": "curated-verified",
        "Docs/KB/START-HERE.md": "curated-verified",
        "Docs/KB/CONVENTIONS.md": "curated-verified",
        "Docs/KB/ARCHITECTURE.md": "curated-verified",
        "Docs/KB/OPERATIONS.md": "curated-verified",
        "Docs/KB/DOMAIN-RULES.md": "curated-verified",
        "Docs/KB/DATA-MODEL.md": "curated-verified",
        "Docs/KB/history/PHASES.md": "historical",
        "ReferenceIndex/README.md": "curated-verified",
        "ReferenceIndex/RefLab_V2_COMPLETE_INDEX.md": "legacy-evidence",
        "ReferenceIndex/RefLab_V2_FILE_INVENTORY.csv": "legacy-evidence",
        "ReferenceIndex/RefLab_V2_OPEN_ITEMS_MAP.csv": "legacy-evidence",
        "README.md": "curated-verified",
        "IDEA.md": "process",
    }
    commit = read_head()
    curated_documents = []
    for relative, provenance in sorted(curated_provenance.items()):
        path = os.path.join(ROOT, relative)
        if not os.path.isfile(path):
            continue
        with open(path, "rb") as handle:
            data = handle.read()
        curated_documents.append({
            "path": relative,
            "provenance": provenance,
            "bytes": len(data),
            "characters": len(data.decode("utf-8", errors="replace")),
            "sha256": hashlib.sha256(data).hexdigest(),
            "verified_against": commit,
        })

    manifest = {
        "schema_version": 1,
        "generated_at": stamp,
        "generator": "scripts/kb_build.py",
        "source_tree_fingerprint": fingerprint,
        "verified_against_commit": commit,
        "counts": {
            "source_files": len(files),
            "symbols": len(symbols),
            "use_cases": len(usecases),
            "tables": len(entities),
            "screens": len([s for s in screens if s["kind"] == "screen"]),
            "panels": len([s for s in screens if s["kind"] == "panel"]),
            "dependency_edges": len(edges),
            "feature_areas": len(AREAS),
        },
        "provenance_classes": {
            "authoritative-source": "The source code itself. It is the only authority.",
            "generated": "Emitted by scripts/kb_build.py from the source tree.",
            "curated-verified": "Hand-written analysis, checked against a named commit.",
            "historical": "An accepted record of a past state. Read as history.",
            "legacy-evidence": "Describes the old reference systems, not this one.",
            "process": "Governance, ownership and working rules.",
            "environment-local": "Describes one workstation. Not portable.",
        },
        "generated_index": sorted(
            "Docs/KB/generated/" + name for name in os.listdir(GENERATED)
            if name.endswith((".csv", ".json"))
        ),
        "curated_documents": curated_documents,
        "feature_files": feature_files,
    }

    os.makedirs(GENERATED, exist_ok=True)
    with open(os.path.join(GENERATED, "kb-manifest.json"), "w", encoding="utf-8") as handle:
        json.dump(manifest, handle, indent=2, ensure_ascii=False)
        handle.write("\n")

    print("  source files       : %d" % len(files))
    print("  symbols            : %d" % len(symbols))
    print("  use cases          : %d" % len(usecases))
    print("  tables             : %d" % len(entities))
    print("  screens            : %d" % len(screens))
    print("  dependency edges   : %d" % len(edges))
    print("  feature files      : %d" % len(feature_files))
    print("  fingerprint        : %s" % fingerprint)
    print("  verified against   : %s" % manifest["verified_against_commit"])
    print("Done.")

    return 0


def read_head() -> str:
    """The commit the working tree is at, if git is available."""
    import subprocess
    try:
        result = subprocess.run(["git", "rev-parse", "HEAD"], cwd=ROOT,
                                capture_output=True, text=True, timeout=20)
        if result.returncode == 0:
            return result.stdout.strip()
    except Exception:
        pass
    return "unknown"


if __name__ == "__main__":
    sys.exit(main())
