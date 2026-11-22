# Server karier

Karier tiap akun dihitung dan disimpan di sini, bukan di HP (PRD 12: server otoritatif).
Aturannya adalah kode game itu sendiri: proyek ini mengompilasi `unity/Assets/_Project/Core` dan `Services`
apa adanya, jadi aturan hanya dirawat di satu tempat.

## Menjalankan

Butuh .NET 8 SDK dan basis data pasar (`server/market`), karena akun dan tokennya dipakai bersama.

```bash
python3 -m server.market --db pasar.db trader Budi      # menerbitkan token akun
dotnet run --project server/career -- pasar.db unity/Assets/_Project/Resources 8766
dotnet test server/career/Tests
# berkas mandiri untuk tes Unity (editor di dalam kontainer tidak punya dotnet):
dotnet publish server/career -c Release -r linux-x64 --self-contained -o server/career/bin/selfhost
# setelah IGameService atau RemoteCalls berubah, tulis ulang separuh RemoteGameService yang dibuat mesin:
dotnet run --project server/career -- generate unity/Assets/_Project/Services/Remote/Generated
```

Server hanya mendengar di `127.0.0.1`; untuk alamat publik pasang di belakang reverse proxy HTTPS,
sama seperti server pasar.

## Protokol

Semua permintaan `POST` dengan `Authorization: Bearer <token>`. Bentuk pesannya ada di
`unity/Assets/_Project/Services/Remote/RemoteMessages.cs`.

| Alamat | Isi | Jawaban |
|---|---|---|
| `/career/open` | kosong | karier lengkap (`save`), setelah waktu pergi dihitung sebagai waktu offline |
| `/career/act` | `RemoteBatch`: nomor urut, daftar panggilan, ringkasan (`digest`) versi HP | hasil tiap panggilan; `save` hanya jika ringkasan HP berbeda dari server |

- **Panggilan** adalah nama metode `IGameService` dan argumennya sebagai teks (`CallText`). Metode mana yang
  dikirim, mana yang dijawab HP sendiri, dan mana yang dilarang diatur di `Services/Remote/RemoteCalls.cs`.
- **Waktu milik server.** HP menyebut jam game tiap panggilan supaya perkiraannya sama dengan hasil server,
  tetapi hanya diterima dalam rentang 10 detik di belakang sampai 2 detik di depan jam server.
  `Advance` dan `CatchUp` tidak bisa dikirim. Tanpa panggilan selama 90 detik, pemain dianggap pergi.
- **Nomor urut.** Kiriman yang sama dua kali dijawab dua kali tetapi dijalankan sekali; nomor yang melompat
  tidak dijalankan dan dijawab dengan karier untuk memulai lagi.

## Susunan

| Folder | Isi |
|---|---|
| `Host/Accounts` | token akun dari basis data pasar |
| `Host/Careers` | karier per akun, jamnya, dan penyimpanannya (tabel `careers`) |
| `Host/Http` | alamat HTTP |
| `Host/Generate` | menulis `RemoteGameService` milik game dari `IGameService` |
| `Host/Unity` | pengganti kecil `UnityEngine` (JSON, Resources, Vector2) supaya kode game terkompilasi di server |
| `Tests` | tes xUnit; `Tests/Support/Phone.cs` meniru HP yang menebak lalu mengikuti server |

## Sisi game

| Berkas (`unity/Assets/_Project/Services/Remote`) | Tugas |
|---|---|
| `RemoteGameService.cs` + `Generated/` | `IGameService` versi online: tiap aksi dijalankan dulu di salinan HP, lalu dikirim |
| `RemoteLink.cs` | antrean, nomor urut, kirim ulang, laporan posisi tiap 3 detik |
| `RemoteRunner.cs` | menjalankan satu panggilan; dipakai server dan HP (saat mengulang antrean setelah koreksi) |
| `Client/` | alamat server, HTTP, akun tersimpan, dan formulir **Main online** di menu awal |

Menu awal → **Main online** → alamat dan token → karier dibuka dari server → kota dimuat dengan `RemoteGameService`
(`GameHost.Online`). Karier di berkas HP tidak disentuh.

## Belum ada

- Halaman pasar, kontrak, asosiasi, dan season masih masuk sendiri dengan token yang sama, dan kas/stok akun pasar
  (`traders`) belum digabung dengan kas/stok karier.
- Posisi pemain dan truk dipercaya dari HP; kecepatan gerak belum diperiksa.
- Saat server tidak terjangkau, pemain masih bisa bermain di salinan HP; hasilnya baru sah setelah server menjawab.
