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
| `Host/Careers` | karier per akun, jamnya, penyimpanannya (tabel `careers`), dan dompet bersama pasar (`Wallet.cs`) |
| `Host/City` | kota bersama: siapa yang ada di kota (`TownSquare`), papan harga lapak (`TownYards`), periksa posisi (`MoveGuard`) |
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

## Satu dompet

Kas dan stok HMS 1 akun pasar (`traders.cash`, `traders.stock`) adalah kas karier dan HMS 1 di lapak karier.

- Tiap batch karier berjalan dalam satu transaksi SQLite: `Wallet.Bring` membawa apa yang diubah pasar (lot, kontrak,
  biaya asosiasi) ke karier, lalu `Wallet.Leave` menulis kas, stok, dan kasta karier ke `traders` dan `standing`.
- Tabel `wallets` mencatat angka terakhir yang ditulis karier; selisihnya dengan `traders` adalah perbuatan pasar.
- Akun yang punya baris `wallets` tidak lagi mendapat stok gratis per jam dari pasar dan tidak bisa membeli kasta di
  pasar; kastanya mengikuti karier.
- Tutup season di pasar mengosongkan `careers` dan `wallets`; karier baru dibuka pada panggilan berikutnya.
- Di game, halaman pasar, kontrak, asosiasi, dan season tersambung sendiri dengan akun **Main online** (`GameHost.Market`).

## Kota bersama

Tiap jawaban server juga membawa `others` (pemain lain) dan `yards` (papan harga lapak pemain lain).

- **Pemain lain.** Paling banyak 12 pemain terdekat yang masih mengirim dalam 15 detik terakhir; game menampilkan
  mereka berjalan dengan namanya (`Presentation/World/Town/Ghost.cs`).
- **Papan harga (PRD 5.2).** Lapak bandar yang aktif dan punya harga beli tampil di kavling 2 sampai 6 kota pemain lain.
  Pemain berdiri di pos kavling dengan muatan tersortir dan menyetor (`SellToPlayerYard`): server memeriksa posisi,
  harga, sisa kapasitas, dan kas pembeli, lalu dalam satu transaksi memindahkan kas ke penjual dan besi ke lapak
  pembeli (`CareerBook.Turn.cs`), juga saat pembeli sedang tidak bermain.
- **Posisi.** `MoveGuard` menolak posisi yang tidak mungkin dicapai: pejalan dibatasi laju jalan, kendaraan yang
  dikemudikan dibatasi laju puncaknya (keduanya dengan kelonggaran 30% dan 10 m), kendaraan parkir tidak berpindah.
  Aksi yang bergantung pada posisi palsu lalu ditolak aturan game di server, dan HP menerima karier versi server.
  Argumen `roam` mematikan pemeriksaan ini; hanya untuk tes yang memindahkan pemain seketika.

## Belum ada

- Kavling 1 selalu lapak pemain itu sendiri, jadi lapak yang sama tampil di kavling berbeda bagi pemain berbeda, dan
  pemain lain yang berdiri di lapaknya sendiri tampak berdiri di kavling 1 Anda. Kota hanya punya 6 kavling lapak.
- Semua karier dimuat ke memori saat server menyala dan batch dijalankan satu per satu; cukup untuk uji kecil,
  belum untuk ribuan pemain.

- Lot yang dibeli tidak diperiksa terhadap kapasitas lapak, dan pemain Kasta 1 bisa menyimpan HMS 1 dari pasar.
- Nilai usaha saat tutup season hanya menghitung kas dan HMS 1, belum truk, alat, dan pabrik.
- Stok kejar ketinggalan pemain yang masuk di tengah season tidak terbawa ke karier.
- Season baru ditutup saat ada yang membuka pasar, bukan oleh jam server karier.
- Pemeriksaan posisi hanya soal jarak dan laju; menembus tembok atau bangunan tidak diperiksa.
- Saat server tidak terjangkau, pemain masih bisa bermain di salinan HP; hasilnya baru sah setelah server menjawab.
