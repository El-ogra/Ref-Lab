import json
t=open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\rea_analyze_raw3.txt",encoding="utf-8-sig").read()
d=json.loads(t)
det=d.get("details",{})
print(d.get("code"), d.get("message"))
diag=det.get("diagnostics",{})
fo=diag.get("failure_output") or {}
print((fo.get("stdout") or {}).get("text","")[-3000:])
