import json, os, re
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
for pat in ("paste","Paste Errors","Pasterror","paste error"):
    hits=[s for s in sc["strings"] if pat.lower() in s["t"].lower()]
    print(pat, len(hits), [(h["h"],h["t"][:60]) for h in hits[:8]])
