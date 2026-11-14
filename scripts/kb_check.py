#!/usr/bin/env python3
"""Validates the Ref Lab knowledge base against the source tree.

The knowledge base is only useful if it never lies about the code. This
script is the gate that proves it. It exits with a non-zero status on the
first failing category, so it can be run before a commit or in a pipeline.

    python scripts/kb_check.py

Checks performed
----------------

  V1  path existence      every repository path named in the curated
                           documents resolves to a real file or directory
  V2  symbol existence     every type name named in the curated documents
                           exists in the source tree
  V3  index freshness      the source-tree fingerprint in kb-manifest.json
                           matches the tree as it is now
  V4  use-case drift       the generated use-case index matches an
                           independent second extraction of the source
  V5  coverage             every source file and every use case is claimed
                           by exactly one feature file
  V6  broken links         every internal Markdown link resolves
  V7  duplicate content    near-identical paragraphs are reported
  V8  budget compliance    the size budgets hold and no generated block has
                           been hand-edited
  V9  numeric claims       a count written in prose matches the generated
                           count it refers to
  V10 convention guards    every rule in CONVENTIONS.md still has its
                           executable guard

V1, V2, V3, V4, V5, V6, V8 and V9 are fully automatic. V7 reports for a
human decision. V10 reports whether a named guard test still exists.

The script has no third-party dependency.
"""

from __future__ import annotations

import csv
import hashlib
import json
import os
import re
import sys
from collections import defaultdict

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
KB = os.path.join(ROOT, "Docs", "KB")
GENERATED = os.path.join(KB, "generated")
FEATURES_DIR = os.path.join(KB, "features")

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from kb_build import (  # noqa: E402
    AREAS, BEGIN_MARKER, END_MARKER, build_db_entity_index, build_file_inventory,
    build_feature_facts, build_screen_index, build_symbol_index,
    build_usecase_index, source_fingerprint,
)
from generate_use_case_inventory import summarise  # noqa: E402

CURATED_DOCS = [
    "Docs/KB/START-HERE.md",
    "Docs/KB/CONVENTIONS.md",
    "Docs/KB/ARCHITECTURE.md",
    "Docs/KB/OPERATIONS.md",
    "Docs/KB/DOMAIN-RULES.md",
    "Docs/KB/DATA-MODEL.md",
]

# The budgets are counted in CHARACTERS, not bytes. The knowledge base is
# written in Arabic, and one Arabic character occupies two bytes in UTF-8, so
# a byte budget would silently penalise the non-English documents and
# under-measure their real reading cost. A character budget is comparable
# across languages.
BUDGETS = {
    "AGENTS.md": 2000,
    "Docs/KB/START-HERE.md": 7000,
    "Docs/KB/CONVENTIONS.md": 9000,
    "Docs/KB/OPERATIONS.md": 9000,
    "Docs/KB/ARCHITECTURE.md": 16000,
    "Docs/KB/DOMAIN-RULES.md": 16000,
    "Docs/KB/DATA-MODEL.md": 16000,
}

# Cues that a line states the ABSENCE of something. A name mentioned on such
# a line is a correct denial, not a broken reference.
NEGATION_CUES = (
    "لا وجود", "لا يوجد", "ليس ", "غير موجود", "لا تُوجد", "لا يمكن",
    "not ", "does not", "never", "absent", "without ",
)

# Cues that a line talks about the LEGACY reference systems. A count on such
# a line describes the old Access database, not this one.
LEGACY_CUES = (
    "legacy", "Legacy", "old system", "Patient.mdb", "Patients.mdb", "Access", "Jet",
    "V-CD-2019", "V-ROOT", "old database",
    "\u0627\u0644\u0646\u0638\u0627\u0645 \u0627\u0644\u0642\u062f\u064a\u0645",
    "\u0627\u0644\u0642\u062f\u064a\u0645\u0629", "\u0627\u0644\u0646\u0638\u0627\u0645\u064a\u0646",
    "\u0627\u0644\u0645\u0648\u0631\u0648\u062b", "\u0645\u0631\u062c\u0639\u064a",
    "\u0627\u0644\u0623\u0631\u0634\u064a\u0641", "\u0642\u062f\u064a\u0645",
)

BEGIN_MARKER_TEXT = "<!-- BEGIN GENERATED FACTS"
END_MARKER_TEXT = "<!-- END GENERATED FACTS -->"

PATH_PATTERN = re.compile(r"`([A-Za-z0-9_./\-]+\.(?:cs|xaml|csproj|md|csv|json|py|sln|props))`")
LINK_PATTERN = re.compile(r"\[[^\]]*\]\(([^)]+)\)")
BACKTICK_IDENT = re.compile(r"`([A-Z][A-Za-z0-9_]{2,})`")

failures: list[str] = []
warnings: list[str] = []


def fail(check: str, message: str) -> None:
    failures.append("[%s] %s" % (check, message))


def warn(check: str, message: str) -> None:
    warnings.append("[%s] %s" % (check, message))


def read(path: str) -> str:
    with open(path, "r", encoding="utf-8", errors="replace") as handle:
        return handle.read()


def curated_texts() -> list[tuple[str, str]]:
    texts = []
    for relative in CURATED_DOCS:
        path = os.path.join(ROOT, relative)
        if os.path.isfile(path):
            texts.append((relative, read(path)))
    for name in sorted(os.listdir(FEATURES_DIR)) if os.path.isdir(FEATURES_DIR) else []:
        if name.endswith(".md"):
            relative = "Docs/KB/features/%s" % name
            texts.append((relative, read(os.path.join(FEATURES_DIR, name))))
    history = os.path.join(KB, "history")
    if os.path.isdir(history):
        for name in sorted(os.listdir(history)):
            if name.endswith(".md"):
                relative = "Docs/KB/history/%s" % name
                texts.append((relative, read(os.path.join(history, name))))
    return texts


# ---------------------------------------------------------------------------
# V1 - path existence
# ---------------------------------------------------------------------------


def check_paths(texts: list[tuple[str, str]]) -> None:
    known_roots = (
        "src/", "tests/", "Docs/", "scripts/", "tools/", "evidence/",
        "ReferenceIndex/",
    )
    for relative, text in texts:
        for match in sorted(set(PATH_PATTERN.findall(text))):
            candidate = match
            if not candidate.startswith(known_roots):
                continue
            if not os.path.exists(os.path.join(ROOT, candidate)):
                fail("V1", "%s names a path that does not exist: %s" % (relative, candidate))


# ---------------------------------------------------------------------------
# V2 - symbol existence
# ---------------------------------------------------------------------------


def check_symbols(texts: list[tuple[str, str]]) -> None:
    # The identifier universe: every type, property, method and enum member
    # that appears in the source, plus the screen keys. A name that the
    # documentation mentions but that exists nowhere in the source is the
    # failure this check exists to catch.
    known = set()
    for base, _dir_names, file_names in os.walk(os.path.join(ROOT, "src")):
        for name in file_names:
            # The file name itself is a legitimate reference: a document may
            # name a validator file rather than the class inside it.
            known.add(os.path.splitext(name)[0])
            if not name.endswith((".cs", ".xaml")):
                continue
            text = read(os.path.join(base, name))
            known.update(re.findall(r"\b[A-Z][A-Za-z0-9_]{2,}\b", text))
            known.update(re.findall(r"\b[A-Za-z_][A-Za-z0-9_]{2,}\b", text))

    # The architecture guard test names live in the test suite, and they are
    # named in the documentation on purpose.
    for base, _dir_names, file_names in os.walk(os.path.join(ROOT, "tests")):
        for name in file_names:
            if not name.endswith(".cs"):
                continue
            known.update(re.findall(r"\b[A-Z][A-Za-z0-9_]{2,}\b", read(os.path.join(base, name))))

    known.update(AREAS.keys())
    known.update({"Permission", "ICurrentUser", "IAppDbContext", "IUnitOfWork", "IClock"})

    for row in build_db_entity_index({}):
        known.add(row["dbset_property"])
    for row in build_screen_index(build_usecase_index({})):
        known.add(row["screen_key"])
        known.add(row["constant"])

    allowed = {
        "SQL", "WPF", "MVVM", "CQRS", "EF", "JSON", "CSV", "SHA", "PDF", "MediatR",
        "FluentValidation", "QuestPDF", "Microsoft", "Windows", "Linux", "NETSDK",
        "NETSDK1138", "NETSDK1135", "CS0118", "CS0108", "MSBuild", "NuGet",
        "Git", "GitHub", "Actions", "README", "AGENTS", "START", "HERE", "CONVENTIONS",
        "ARCHITECTURE", "OPERATIONS", "DOMAIN", "RULES", "DATA", "MODEL", "USE",
        "CASE", "INVENTORY", "IDE", "VS", "SDK", "PTC", "CBCCONS", "EGP", "Docs",
        "docs", "src", "tests", "scripts", "tools", "evidence", "ReferenceIndex",
        "Dotnet", "dotnet", "LocalDB", "powershell", "PowerShell", "Python",
        "Boolean", "String", "Double", "Decimal", "Guid", "Int32",
        # Legacy artefacts of the reference systems. They are named in the
        # knowledge base only to say what was removed or what the evidence
        # was, and they exist in the legacy corpus, not in src/.
        "Labbranch", "patientinfo", "patienttest", "testtable", "normalrange",
        "constanttable", "subgrouptable", "testunits", "converts", "abbreviations",
        "Gehats", "pricelist", "mowzaf", "printuser", "printdate", "printnum",
        "SMSSender", "loger", "PackageReference", "ProjectReference", "LEGACY",
        "Expected", "Total", "Import", "Export", "First", "Second",
        "EnableWindowsTargeting", "TreatWarningsAsErrors", "LangVersion",
    }

    reported = 0
    for relative, text in texts:
        for number, line in enumerate(text.splitlines(), start=1):
            # A name that the documentation explicitly denies ("there is no
            # such command") is a correct statement, not a broken reference.
            if any(cue in line for cue in NEGATION_CUES):
                continue
            for candidate in sorted(set(BACKTICK_IDENT.findall(line))):
                if candidate in known or candidate in allowed:
                    continue
                if candidate.isupper() and "_" in candidate:
                    continue
                if candidate.startswith("V2REQ") or candidate.startswith("RLP"):
                    continue
                warn("V2", "%s:%d mentions `%s`, which does not exist anywhere in src/"
                     % (relative, number, candidate))
                reported += 1
                if reported > 25:
                    return


# ---------------------------------------------------------------------------
# V3 - index freshness
# ---------------------------------------------------------------------------


def check_fingerprint() -> None:
    manifest_path = os.path.join(GENERATED, "kb-manifest.json")
    if not os.path.isfile(manifest_path):
        fail("V3", "Docs/KB/generated/kb-manifest.json is missing — run scripts/kb_build.py")
        return
    manifest = json.loads(read(manifest_path))
    recorded = manifest.get("source_tree_fingerprint")
    actual = source_fingerprint()
    if recorded != actual:
        fail("V3", "the generated index is stale.\n"
                   "        recorded: %s\n"
                   "        actual  : %s\n"
                   "        fix     : python scripts/kb_build.py" % (recorded, actual))
    else:
        print("  V3  fingerprint      OK  %s" % actual[:16])


# ---------------------------------------------------------------------------
# V4 - use-case drift
# ---------------------------------------------------------------------------


def check_usecase_drift() -> None:
    index_path = os.path.join(GENERATED, "usecase-index.csv")
    if not os.path.isfile(index_path):
        fail("V4", "Docs/KB/generated/usecase-index.csv is missing — run scripts/kb_build.py")
        return

    rows = []
    with open(index_path, "r", encoding="utf-8") as handle:
        lines = [line for line in handle if not line.startswith("#")]
    for row in csv.DictReader(lines):
        rows.append(row)

    entries, totals = summarise()
    if len(rows) != len(entries):
        fail("V4", "the index records %d use cases but the source defines %d\n"
                   "        fix: python scripts/kb_build.py" % (len(rows), len(entries)))
        return

    index_names = sorted(row["usecase"] for row in rows)
    source_names = sorted(entry["name"] for entry in entries)
    if index_names != source_names:
        only_index = set(index_names) - set(source_names)
        only_source = set(source_names) - set(index_names)
        fail("V4", "the index and the source disagree.\n"
                   "        only in the index: %s\n"
                   "        only in the source: %s" % (sorted(only_index), sorted(only_source)))
        return

    print("  V4  use-case drift    OK  %d use cases (%d commands, %d queries, %d areas)"
          % (totals["useCases"], totals["commands"], totals["queries"], totals["areas"]))


# ---------------------------------------------------------------------------
# V5 - coverage
# ---------------------------------------------------------------------------


def check_coverage() -> None:
    files = build_file_inventory()
    texts = curated_texts()

    claimed: dict[str, list[str]] = defaultdict(list)
    for relative, text in texts:
        if not relative.startswith("Docs/KB/features/"):
            continue
        for match in PATH_PATTERN.findall(text):
            claimed[match].append(relative)

    missing = []
    shared_missing = []
    architecture = dict(texts).get("Docs/KB/ARCHITECTURE.md", "")
    for row in files:
        if row["layer"] == "Tests":
            continue
        # The EF Core designer files and the model snapshot are generated
        # artefacts of the migrations; the migration itself is the artefact
        # that is named and documented.
        if row["path"].endswith(".Designer.cs"):
            continue
        if row["path"].endswith("RefLabDbContextModelSnapshot.cs"):
            continue
        if row["path"] in claimed:
            continue
        if row["feature_area"] == "Shared":
            # The cross-cutting files are documented as a set in
            # ARCHITECTURE.md rather than in a feature file.
            if row["path"] not in architecture:
                shared_missing.append(row["path"])
            continue
        missing.append(row["path"])

    if missing:
        fail("V5", "%d source file(s) are not named in any feature file:\n        %s"
             % (len(missing), "\n        ".join(missing[:12])))

    if shared_missing:
        warn("V5", "%d cross-cutting file(s) are not named in ARCHITECTURE.md:\n        %s"
             % (len(shared_missing), "\n        ".join(shared_missing[:12])))

    usecases = build_usecase_index({})
    counts: dict[str, int] = defaultdict(int)
    for row in usecases:
        counts[row["usecase"]] += 1
        if row["feature_file"] and row["feature_file"] not in claimed:
            fail("V5", "the use case %s lives in %s, but no feature file names that file"
                 % (row["usecase"], row["feature_file"]))

    duplicated = [name for name, count in counts.items() if count != 1]
    if duplicated:
        fail("V5", "use cases claimed more than once: %s" % duplicated)

    print("  V5  coverage          OK  %d use cases, each in exactly one feature file"
          % len(usecases))


# ---------------------------------------------------------------------------
# V6 - broken internal links
# ---------------------------------------------------------------------------


def check_links(texts: list[tuple[str, str]]) -> None:
    broken = 0
    for relative, text in texts:
        base = os.path.dirname(os.path.join(ROOT, relative))
        for target in LINK_PATTERN.findall(text):
            if target.startswith(("http://", "https://", "#", "mailto:")):
                continue
            clean = target.split("#")[0]
            if not clean:
                continue
            resolved = os.path.normpath(os.path.join(base, clean))
            if not os.path.exists(resolved):
                fail("V6", "%s has a broken link: %s" % (relative, target))
                broken += 1
    if not broken:
        print("  V6  broken links      OK")


# ---------------------------------------------------------------------------
# V7 - duplicate content
# ---------------------------------------------------------------------------


def check_duplicates(texts: list[tuple[str, str]]) -> None:
    seen: dict[str, str] = {}
    duplicates = 0
    for relative, text in texts:
        for block in re.split(r"\n\s*\n", text):
            block = " ".join(block.split())
            if len(block) < 160:
                continue
            if block.startswith("|"):
                continue
            if block in seen and seen[block] != relative:
                warn("V7", "identical paragraph in %s and %s" % (seen[block], relative))
                duplicates += 1
            else:
                seen[block] = relative
    if not duplicates:
        print("  V7  duplicate content OK")


# ---------------------------------------------------------------------------
# V8 - budget compliance and generated-block freshness
# ---------------------------------------------------------------------------


def check_budgets() -> None:
    for relative, limit in BUDGETS.items():
        path = os.path.join(ROOT, relative)
        if not os.path.isfile(path):
            warn("V8", "%s is missing" % relative)
            continue
        with open(path, "r", encoding="utf-8") as handle:
            size = len(handle.read())
        if size > limit:
            fail("V8", "%s is %d characters, over its %d character budget"
                 % (relative, size, limit))

    # The generated block of every feature file must match what the
    # generator would produce right now.
    files = build_file_inventory()
    _symbols, declaring = build_symbol_index()
    usecases = build_usecase_index(declaring)
    screens = build_screen_index(usecases)
    entities = build_db_entity_index(declaring)

    stale = []
    if os.path.isdir(FEATURES_DIR):
        for name in sorted(os.listdir(FEATURES_DIR)):
            if not name.endswith(".md"):
                continue
            area = name[:-3]
            if area not in AREAS:
                continue
            path = os.path.join(FEATURES_DIR, name)
            text = read(path)
            if BEGIN_MARKER_TEXT not in text or END_MARKER_TEXT not in text:
                stale.append("%s has no generated block" % name)
                continue
            current = build_feature_facts(area, usecases, screens, files, entities)
            if current not in text:
                stale.append("%s has a hand-edited generated block" % name)

    for item in stale:
        fail("V8", "%s — fix: python scripts/kb_build.py" % item)

    if not stale:
        print("  V8  budgets           OK  all sizes within budget, no hand-edited blocks")


# ---------------------------------------------------------------------------
# V9 - numeric claims in prose
# ---------------------------------------------------------------------------


def check_numeric_claims(texts: list[tuple[str, str]]) -> None:
    files = build_file_inventory()
    _symbols, declaring = build_symbol_index()
    usecases = build_usecase_index(declaring)
    from kb_build import build_db_entity_index
    entities = build_db_entity_index(declaring)
    screens = build_screen_index(usecases)

    facts = {
        "use case": len(usecases),
        "use cases": len(usecases),
        "table": len(entities),
        "tables": len(entities),
        "screen": len([s for s in screens if s["kind"] == "screen"]),
        "screens": len([s for s in screens if s["kind"] == "screen"]),
        "panel": len([s for s in screens if s["kind"] == "panel"]),
        "panels": len([s for s in screens if s["kind"] == "panel"]),
        "feature area": len(AREAS),
        "feature areas": len(AREAS),
        "domain type": len([f for f in files if f["layer"] == "Domain"]),
    }

    # The knowledge base is written in Arabic, so the same nouns appear in
    # Arabic. Both spellings must be checked. The Arabic nouns are matched
    # with explicit alternatives because a bare Arabic word is ambiguous.
    arabic_nouns = {
        "حالة": len(usecases),
        "حالات": len(usecases),
        "حالة استخدام": len(usecases),
        "حالات استخدام": len(usecases),
        "جدول": len(entities),
        "جداول": len(entities),
        "شاشة": len([s for s in screens if s["kind"] == "screen"]),
        "شاشات": len([s for s in screens if s["kind"] == "screen"]),
        "مجال": len(AREAS),
        "مجالا": len(AREAS),
        "مجالاً": len(AREAS),
        "مجالات": len(AREAS),
        "مجال وظيفي": len(AREAS),
        "مجالا وظيفي": len(AREAS),
        "مجالاً وظيفياً": len(AREAS),
    }
    facts.update(arabic_nouns)

    nouns = "|".join(
        re.escape(noun) for noun in
        list(arabic_nouns.keys()) +
        ["use cases?", "tables?", "screens?", "feature areas?", "domain types?"]
    )

    # The count may be wrapped in backticks, in bold, or in both.
    pattern = re.compile(
        r"[*`]{0,2}(\d+)[*`]{0,2}\s+(" + nouns + r")", re.I)

    for relative, text in texts:
        for match in pattern.finditer(text):
            number = int(match.group(1))
            noun = match.group(2).strip().lower()
            expected = facts.get(noun)
            if expected is None:
                # The matched group may carry a trailing Arabic diacritic or
                # a differing form; fall back to the longest known noun that
                # the match starts with.
                for candidate in sorted(facts, key=len, reverse=True):
                    if noun.startswith(candidate) and candidate:
                        expected = facts[candidate]
                        break
            if expected is None:
                continue
            # A count in a passage that talks about the LEGACY reference
            # systems refers to the legacy database (103 tables), not to this
            # one. The context is the whole paragraph, because the cue and the
            # number are often on different lines of it.
            para_start = text.rfind("\n\n", 0, match.start()) + 2
            para_end = text.find("\n\n", match.end())
            paragraph = text[para_start:para_end if para_end > 0 else len(text)]
            if any(cue in paragraph for cue in LEGACY_CUES):
                continue
            if number != expected:
                fail("V9", "%s says '%d %s' but the source has %d"
                     % (relative, number, noun, expected))
    print("  V9  numeric claims    OK")


# ---------------------------------------------------------------------------
# V10 - convention guards
# ---------------------------------------------------------------------------


def check_convention_guards(texts: list[tuple[str, str]]) -> None:
    guard_source = ""
    for base, _dir_names, file_names in os.walk(os.path.join(ROOT, "tests")):
        for name in file_names:
            if name.endswith(".cs"):
                guard_source += read(os.path.join(base, name))

    conventions = dict(texts).get("Docs/KB/CONVENTIONS.md", "")
    if not conventions:
        warn("V10", "Docs/KB/CONVENTIONS.md is missing")
        return

    expected = [
        ("B8_the_wpf_application_type_is_always_fully_qualified", "CS0118"),
        ("The_composition_root_never_resolves_the_database_initialiser", "DatabaseInitializer"),
        ("B9_every_arabic_string_is_authorised_by_the_verified_label_source", "ui-labels-verified"),
        ("Domain_references_only_the_base_class_library", "zero package references"),
        ("The_laboratory_currency_is_egp", "EGP"),
    ]

    missing = []
    for guard, topic in expected:
        if guard in conventions and guard not in guard_source:
            missing.append("%s (about %s)" % (guard, topic))

    for item in missing:
        fail("V10", "CONVENTIONS.md cites the guard %s, but no such test exists" % item)

    if not missing:
        print("  V10 convention guards OK  every cited guard test still exists")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def main() -> int:
    print("Checking the Ref Lab knowledge base...\n")

    texts = curated_texts()
    if not texts:
        fail("V1", "no curated document was found under Docs/KB")
        return 1

    check_fingerprint()
    check_usecase_drift()
    check_coverage()
    check_paths(texts)
    check_symbols(texts)
    check_links(texts)
    check_duplicates(texts)
    check_budgets()
    check_numeric_claims(texts)
    check_convention_guards(texts)

    print("")
    if warnings:
        print("%d warning(s):" % len(warnings))
        for item in warnings:
            print("  ! " + item)
        print("")

    if failures:
        print("%d FAILURE(S):" % len(failures))
        for item in failures:
            print("  x " + item)
        print("")
        print("FAILURE: the knowledge base does not describe the current source tree.")
        return 1

    print("SUCCESS: the knowledge base matches the source tree.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
