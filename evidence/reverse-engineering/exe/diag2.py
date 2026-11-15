import json
t=open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\rea_analyze_raw2.txt",encoding="utf-8-sig").read()
d=json.loads(t)
det=d["details"]
print("failure_kind:", det.get("failure_kind"))
print("failure_message:", det.get("failure_message"))
diag=det.get("diagnostics",{})
for k in ("exit_code","exit_signal","diagnostic_truncated","output_incomplete"):
    print(k, diag.get(k))
fo=diag.get("failure_output") or {}
print("stdout tail:", (fo.get("stdout") or {}).get("text","")[-2500:])
print("stderr tail:", (fo.get("stderr") or {}).get("text","")[-1500:])
