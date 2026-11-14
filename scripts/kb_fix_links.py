#!/usr/bin/env python3
"""Repairs root-relative Markdown links inside Docs/KB into file-relative ones.

A Markdown link is resolved relative to the file that contains it. A link
written as ``Docs/KB/CONVENTIONS.md`` inside ``Docs/KB/features/Patients.md``
therefore points at ``Docs/KB/features/Docs/KB/CONVENTIONS.md`` and is broken
for every reader, even though the path looks right when read against the
repository root.

This script rewrites such links to the form that actually resolves, and
reports every link it could not repair.
"""
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
KB = os.path.join(ROOT, "Docs", "KB")
LINK = re.compile(r"\[([^\]]*)\]\(([^)#]+?)\)")


def main() -> int:
    state = {"changed": 0}
    unresolved = []
    for directory, _dir_names, file_names in os.walk(KB):
        for name in sorted(file_names):
            if not name.endswith(".md"):
                continue
            path = os.path.join(directory, name)
            text = open(path, encoding="utf-8").read()
            original = text

            def rewrite(match):
                label, target = match.group(1), match.group(2)
                if target.startswith(("http://", "https://", "mailto:")):
                    return match.group(0)
                if os.path.exists(os.path.join(directory, target)):
                    return match.group(0)
                if os.path.exists(os.path.join(ROOT, target)):
                    relative = os.path.relpath(os.path.join(ROOT, target), directory)
                    relative = relative.replace(os.sep, "/")
                    state["changed"] += 1
                    return "[%s](%s)" % (label, relative)
                unresolved.append("%s -> %s" % (os.path.relpath(path, ROOT), target))
                return match.group(0)

            text = LINK.sub(rewrite, text)
            if text != original:
                with open(path, "w", encoding="utf-8") as handle:
                    handle.write(text)

    print("repaired %d link(s)" % state["changed"])
    for item in unresolved:
        print("  unresolved: %s" % item)
    return 1 if unresolved else 0


if __name__ == "__main__":
    sys.exit(main())
