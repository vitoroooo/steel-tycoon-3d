# Arsitektur Fase 1 (Prototipe Single-Player K1 → K2)

Sumber kebutuhan: PRD §17.1, angka dari docs/ECONOMY_SPEC.md (hasil Fase 0).

## Prinsip
- Maksimal 100 baris per file; satu tanggung jawab per file; folder per sistem.
- Logika game = C# murni (tanpa UnityEngine) di `Steel.Core`, bisa dites cepat.
- Unity hanya presentasi (scene, 3D, UI) dan memanggil Core lewat interface.
- Dunia terbuka third-person ala GTA: berjalan dan mengemudi gerobak/triseda/pikap/truk.
- Visual 3D HD detail (stylized boleh), **bukan low-poly**; gunakan URP + aset HD.
- Mobile memakai touch joystick; keyboard untuk editor. Menu manajemen sebagai overlay.
- Semua angka balancing di data (JSON di `Resources/Config`), bukan di kode.
- Waktu game deterministik: 1 jam game = 10 menit nyata; offline maks 8 jam nyata.
- Fase 2 (server otoritatif) cukup mengganti implementasi interface `Services`.

## Struktur `unity/Assets/_Project/`
| Folder | Assembly | Isi |
|---|---|---|
| `Core/Economy` | Steel.Core | IBN, harga AI, margin, pajak |
| `Core/Scrap` | Steel.Core | Tumpukan rongsok, grade tersembunyi, inspeksi, sortir (§6.1) |
| `Core/Fleet` | Steel.Core | Gerobak/triseda/pikap, kapasitas, waktu trip (§5.1) |
| `Core/World` | Steel.Core | Cuaca, status lahan, karat stok terbuka (§6.4) |
| `Core/Equipment` | Steel.Core | Keausan alat, 3 pilihan perbaikan (§6.5) |
| `Core/Progression` | Steel.Core | Syarat & buka Kasta 2, papan harga beli, setoran AI |
| `Core/Time` | Steel.Core | Jam game, tick, simulasi offline |
| `Core/State` | Steel.Core | GameState + save/load (JSON) |
| `Services` | Steel.Services | Interface + implementasi lokal (pengganti backend) |
| `Services/Launch` | Steel.Services | Alur layar awal (splash → menu → konfirmasi → muat), teks menu, ringkasan & mulai karier |
| `Presentation/UI/Boot` | Steel.UI | Scene Boot: splash, menu awal dua jempol, layar muat; tema di `Resources/Theme/Boot.json` |
| `Presentation/UI` | Steel.UI | HUD (§12.1), nav bawah (§12.2), dialog konfirmasi |
| `Presentation/World` | Steel.World | Kota 3D HD (URP), kamera third-person |
| `Presentation/Player` | Steel.Player | Character controller dan interaksi tumpukan di dunia |
| `Presentation/Vehicles` | Steel.Vehicles | Kendaraan yang bisa dikemudikan |
| `Presentation/Input` | Steel.Input | Touch joystick mobile dan keyboard |
| `Tests/EditMode` | Steel.Tests | Unit test Core (NUnit) |
| `Tests/PlayMode` | Steel.PlayTests | Smoke test scene City dan siklus transaksi |
| `Editor` | Steel.Editor | Generator City deterministik + build Linux |

## Aturan dependensi
`Presentation → Services → Core`. Core tidak boleh referensi Unity atau layer lain.
Slice: City memakai CharacterController, orbit camera, Input System, joystick dan HUD Indonesia.
Core/State/Slice menyimpan posisi interaksi, world piles, inventori dan kas.
Services memberi DTO untuk presentasi; inspeksi/pungut/sortir/konfirmasi jual lewat IGameService.
Harga AI diperiksa lagi saat konfirmasi; penawaran kedaluwarsa harus diperbarui.
City.json/Piles.json adalah data bersama untuk Core dan generator scene.
Piles.json `restockHours`: tiap jam game satu tumpukan kosong terisi lagi (urutan kosong tersimpan di save).
Services/Guide/IncomeView merangkum sumber uang pemula (tumpukan, isi ulang, rute gerobak gratis, kiriman)
untuk Presentation/UI/Guide (IncomeHint, DartoLetter); tes di Tests/EditMode/Income dan PlayMode/StreetIncomeTests.
Editor membangun geometri detail, PBR CC0, HDRI, URP, ACES, bloom dan SSAO.
Scenes/City.unity, Generated/* serta ProjectSettings adalah keluaran generator (boleh >100 baris).
Input/Player/UI/World/Vehicles aktif; Core/Fleet menyimpan kapasitas pikap dan waktu trip.
Vehicle.json: 1 ton, 60 menit nyata/trip; pikap pinjaman demo, bukan pembelian gratis.
Masuk memindahkan tas ke bak; keluar meninggalkan muatan di bak. DTO VehicleView via IGameService.
Fisika kinematik di Presentation/Vehicles; kapasitas, validasi masuk/keluar dan timer di Core.
glTFast mengimpor model CC0 Poly Haven/Quaternius; FBX worker memakai animasi idle/walk.
Generator menambah atap pelana, kanopi, shutter, pagar dan kabel; HUD mengikuti safe area.
Belum ada save/load, cuaca, simulasi supply kota, pembelian armada atau progresi kasta.

## Build & tes (headless, Docker game-ci)
- Tes: `unity-editor -batchmode -runTests -testPlatform EditMode`
- Build: Android APK via image `unityci/editor:*-android-*`.
