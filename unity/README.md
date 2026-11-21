# Steel Tycoon 3D — HD city + pikap slice

Unity **2022.3.62f1**, Test Framework 1.1.33, Input System 1.7.0.
URP manifest14.0.11 resolves to14.0.12 in this editor (packages-lock.json).
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
ProjectSettings, Scenes/City.unity and Generated/* are generator outputs, not authored code.

## Generate, play, build
Open Assets/_Project/Scenes/City.unity and press Play.
WASD / on-screen joystick walks; hold mouse-right / drag upper-right screen to orbit.
E / bottom-right touch button: inspect, then pick up. Return to lapak to sort.
Walk to Pabrik AI, preview price/margin, then confirm the sale (or cancel).
All HUD/dialog text is Indonesian. Cash, carried kg, IBN and game clock are live.
Near the pikap: E / MASUK PIKAP enters; WASD / joystick drives; release to brake.
E exits when no pile/station takes priority; KELUAR PIKAP always offers exit when stopped.
No Android build, save/load, weather or caste progression in this slice.

Menu: Steel > Generate City. Headless: use the Docker prefix above, replace
the test arguments (after -projectPath /repo/unity) with:
```sh
-executeMethod Steel.Editor.CityBuilder.Generate -quit -logFile /repo/unity/Logs/generate-city.log
-runTests -testPlatform PlayMode -testResults /repo/unity/test-results-play.xml
-executeMethod Steel.Editor.CityBuilder.BuildLinux -quit -logFile /repo/unity/Logs/build-linux.log
```
These are three separate invocations. Generation replaces City deterministically.
If the image lacks /root/.cache, mount a writable cache there (not the license directory).
Linux build: unity/Build/Linux/SteelTycoon.x86_64 (generated, ignored).
Run with DISPLAY=:0 and optional -screenshot /home/ubuntu/shots/hd-walking.png.
The screenshot argument captures the actual player after four seconds.
See Assets/ThirdParty/PolyHaven/LICENSES.md for CC0 sources and checksum provenance.
Downloads are1K diffuse/normal/roughness + HDRI, 11.76MB (<150MB).
Poly Haven model refresh: python unity/Tools/download_models.py from the repo root (~30MB).
Combined new source downloads, including the Quaternius archive/pikap, total78.67MB.
See ThirdParty/PolyHavenModels/LICENSES.md and ThirdParty/Quaternius/LICENSES.md for
CC0 models and SHA256 manifests. glTFast6.11 imports glTF; the worker uses FBX idle/walk clips.
The generator adds gabled roofs, corrugated awnings, scanned shutters, fences and cables.

## Boundaries
- Steel.Core: pure C#, no engine references, deterministic elapsed-time APIs.
- Services: resource loading and local composition behind IGameService/IConfigSource.
- Presentation → Services → Core; no presentation assembly references Steel.Core.
- UI/World/Player/Input/Vehicles are active; Core/Fleet owns pikap capacity and trip time.
- Core/World/Equipment/Progression folders remain reserved.
- Editor scripts generate the detailed block, textured PBR, HDRI, shadows, ACES, bloom and SSAO.
- CharacterController movement and orbit collision are presentation; interaction range,
  capacity, inspection, sorting, cash and confirmed sales live in Core/Services.

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
City.json configures start cash13m, carry cap50kg, interaction3.5m, stations and controls.
It also configures invisible city boundaries, spawnY and respawnBelowY (-15).
FallRecovery returns characters or rigidbodies to spawn without changing cash/inventory.
Vehicle.json: 1000kg, 3600-real-second city trip, PRD price Rp85m; physical arcade speed
12m/s (reverse4m/s) is a prototype scale choice, not a speed specified in the PRD.
The demo provides a loaner, not ownership/purchase or a cash grant. Entering transfers
the backpack into its cargo; exit leaves cargo in the pikap. Sorting/sales use the
active container. Trip time counts occupied time, pauses outside, and clamps at zero;
the next entry after completion begins another trip. No automated delivery reward exists.
Piles.json configures eight finite world piles; no replenishment/faucet or NPC stock purchases.
Factory prices apply K1 AI buy spread and grade factors; reuse uses its reference multiplier.
Free collection costs Rp0; margin equals proceeds. NPC sales have no player transaction tax.
Preview does not pay; confirmation rechecks location, inventory revision and quote bucket.
Margin includes supplied loss, transaction tax and operating cost, not future income tax.
Scrap starts uninspected. Factory sales require inspected, collected and sorted inventory.
Inspection is free and reveals grade; Sort is the successful sorting outcome, not a minigame.
Sorting applies §6.1 multipliers and removes dirt weight once; reuse quality interpolates
160–250%. Legacy Quote uses reference price before buyer spread, not a completed sale.
Weather/rust (§6.4), vehicle purchases, persistence and other MVP criteria are intentionally
outside this handoff. No Phase0 all-seed balance claim is made.
