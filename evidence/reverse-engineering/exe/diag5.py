import json
t=open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\rea_analyze_raw4.txt",encoding="utf-8-sig").read()
d=json.loads(t)
print(d.get("code"), "|", d.get("message"))
det=d.get("details",{})
for r in det.get("rejections",[]) or []:
    print("REJECT", r.get("provider_id"), r.get("code"), str(r.get("reason"))[:200])
diag=det.get("diagnostics",{})
fo=diag.get("failure_output") or {}
print("STDOUT TAIL:", (fo.get("stdout") or {}).get("text","")[-2500:])
