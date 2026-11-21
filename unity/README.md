# Steel Tycoon 3D — Fase 1 skeleton

Unity **2022.3.62f1**, Test Framework 1.1.33, URP 14.0.11.
See ../docs/ARCHITECTURE.md and ../docs/ECONOMY_SPEC.md.

## Headless EditMode tests (from repository root)
```sh
docker run --rm --network host \
  -v ~/unity-lic/machine-id:/etc/machine-id:ro \
  -v ~/unity-lic/config/unity3d:/root/.config/unity3d \
  -v /home/ubuntu/repos/steel-tycoon-3d:/repo \
  unityci/editor:ubuntu-2022.3.62f1-base-3 \
  unity-editor -batchmode -nographics -projectPath /repo/unity \
  -runTests -testPlatform EditMode -testResults /repo/unity/test-results.xml \
  -logFile /repo/unity/Logs/editmode.log
```
Do not add -quit: the test runner exits after completion. Results and logs are ignored.
Never modify unity-lic or touch the unity-box container.
Generated project/cache files may be root-owned after Docker writes them.
ProjectSettings are Unity-generated defaults, not hand-authored balancing data.

## Boundaries
- Steel.Core: pure C#, no engine references, deterministic elapsed-time APIs.
- Services: resource loading and local composition behind IGameService/IConfigSource.
- Presentation → Services → Core; UI/World/Player/Vehicles/Input are asmdef placeholders.
- Fleet/World/Equipment/Progression folders are reserved, not implemented.
- Third-person open-world HD 3D with walking/driving and mobile joystick is the direction.
  URP is installed; render-pipeline assets, HD art, scenes and gameplay come later.
- No visual surface is implemented or visually verified in this skeleton.

## Data and semantics
JSON is in Assets/_Project/Resources/Config. Core parses supplied text without Unity.
The deliberately small format accepts a flat object of finite numbers with identifier
keys, no strings, arrays, nested objects or duplicate keys; invalid/missing data throws.
Time:600 real seconds/game hour; HourTick fires at each crossed boundary; offline
CatchUp caps each supplied absence at28800 real seconds. Caller persists last-seen
time and must not replay the same absence; online Advance is uncapped.
IBN:60-real-day cycle, seeded daily noise, bounded supply effect; quotes cache by
600-second bucket. Input is city ingot kg for the supply window (same as simulator).
Local service uses neutral supply until city simulation exists; Core accepts actual supply.
AI buy/sell names are from the AI's perspective. Config uses the Phase0 fixed coefficients.
Quotes are per kg, not purchase authorization: money, inventory and NPC caps are future work.
Margin includes supplied loss, transaction tax and operating cost, not future income tax.
Scrap starts uninspected and sells at mixed valuation until inspected AND sorted.
Inspection is free and reveals grade; Sort is the successful sorting outcome, not a minigame.
Sorting applies §6.1 multipliers and removes dirt weight once; reuse quality interpolates
160–250%. Quote uses reference price (before buyer spread), not a completed sale.
Weather/rust (§6.4), vehicles (§5.1), persistence and other MVP criteria are intentionally
outside this handoff. No Phase0 all-seed balance claim is made.
