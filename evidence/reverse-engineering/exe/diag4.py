import json
t=open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\rea_analyze_raw3.txt",encoding="utf-8-sig").read()
d=json.loads(t)
det=d.get("details",{})
print(json.dumps({k:v for k,v in det.items() if k!="diagnostics"}, indent=1)[:3000])
diag=det.get("diagnostics",{})
for k,v in diag.items():
    if k in ("failure_output","stdout","stderr"): continue
    print(k, "=", str(v)[:1500])
