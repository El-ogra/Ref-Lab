#!/usr/bin/env python3
"""Verifies the Ref Lab Phase 5 delivery against its manifest.

The tool reads the delivery manifest, recomputes the SHA-256 of every file it
lists over the extracted tree, and compares the two. It exits with a non-zero
status on the first mismatch, so it can be used as a gate before the delivery
is integrated.

It also reports the files that are present in the tree but absent from the
manifest, so that an incomplete or an unexpected delivery is caught as well.

Usage
-----
    python tools/verify_manifest.py --root <extracted-root> \
                                    --manifest <path-to-manifest>

The extracted root is the folder that holds ``src``, ``Docs`` and ``tools`` —
that is, the ``RefLab-Phase5-Presentation`` folder itself, not the folder that
contains it.

The manifest is the Markdown file
``Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md``. The file table is read
from the Markdown rows of its manifest section, so the manifest stays the
single human-readable record of the delivery.
"""

from __future__ import annotations

import argparse
import hashlib
import os
import re
import sys

ROW_PATTERN = re.compile(
    r"^\|\s*\d+\s*\|\s*`(?P<path>[^`]+)`\s*\|\s*(?P<change>Added|Replaced)\s*\|"
    r"\s*(?P<bytes>\d+)\s*\|\s*`(?P<sha>[0-9a-f]{64})`\s*\|"
)


def sha256_of(path: str) -> str:
    """Computes the SHA-256 of a file."""
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 16), b""):
            digest.update(chunk)
    return digest.hexdigest()


def read_manifest_rows(manifest_path: str) -> list[tuple[str, str, int, str]]:
    """Reads the file rows of the manifest."""
    rows: list[tuple[str, str, int, str]] = []

    with open(manifest_path, "r", encoding="utf-8") as handle:
        for line in handle:
            match = ROW_PATTERN.match(line.strip())
            if match is None:
                continue
            rows.append(
                (
                    match.group("path").replace("\\", "/"),
                    match.group("change"),
                    int(match.group("bytes")),
                    match.group("sha").lower(),
                )
            )

    return rows


def collect_tree_files(root: str) -> set[str]:
    """Collects every file of the extracted tree, as forward-slash relative paths."""
    collected: set[str] = set()

    for directory, directory_names, file_names in os.walk(root):
        directory_names[:] = [name for name in directory_names if name not in ("bin", "obj", ".git", ".vs")]

        for name in file_names:
            full = os.path.join(directory, name)
            relative = os.path.relpath(full, root).replace("\\", "/")
            collected.add(relative)

    return collected


def main() -> int:
    parser = argparse.ArgumentParser(description="Verify a Ref Lab delivery against its manifest.")
    parser.add_argument("--root", required=True, help="The extracted delivery root.")
    parser.add_argument("--manifest", required=True, help="The path of the delivery manifest.")
    parser.add_argument("--quiet", action="store_true", help="Only report the failures.")
    arguments = parser.parse_args()

    root = os.path.abspath(arguments.root)
    manifest = os.path.abspath(arguments.manifest)

    if not os.path.isdir(root):
        print(f"FAIL  the delivery root '{root}' does not exist.")
        return 2

    if not os.path.isfile(manifest):
        print(f"FAIL  the manifest '{manifest}' does not exist.")
        return 2

    rows = read_manifest_rows(manifest)

    if not rows:
        print("FAIL  the manifest contains no file rows.")
        return 2

    failures = 0
    verified = 0

    for relative, change, expected_bytes, expected_sha in rows:
        full = os.path.join(root, relative.replace("/", os.sep))

        if not os.path.isfile(full):
            print(f"FAIL  missing   {relative}")
            failures += 1
            continue

        actual_bytes = os.path.getsize(full)

        if actual_bytes != expected_bytes:
            print(f"FAIL  size     {relative}: expected {expected_bytes} bytes, found {actual_bytes}")
            failures += 1
            continue

        actual_sha = sha256_of(full)

        if actual_sha != expected_sha:
            print(f"FAIL  hash     {relative}")
            print(f"        expected {expected_sha}")
            print(f"        found    {actual_sha}")
            failures += 1
            continue

        verified += 1

        if not arguments.quiet:
            print(f"ok    {change:<8} {relative}")

    manifest_relative = os.path.relpath(manifest, root).replace("\\", "/")
    tree_files = collect_tree_files(root)
    listed = {row[0] for row in rows}

    for extra in sorted(tree_files - listed - {manifest_relative}):
        print(f"WARN  unlisted {extra}")

    if not arguments.quiet:
        print()
        print(f"verified {verified} of {len(rows)} file(s)")
        print(f"manifest sha256 {sha256_of(manifest)}")

    if failures:
        print()
        print(f"FAILURE: {failures} file(s) did not match the manifest.")
        return 1

    print()
    print("SUCCESS: every file matches the manifest.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
