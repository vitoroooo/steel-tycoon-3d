# Steel Tycoon 3D — simulator ekonomi Fase 0

Python 3.10+, tanpa dependency eksternal. Dari root repository:

```sh
cd sim
python -m unittest discover -s tests
python main.py
```

Hasil deterministik seed 73: `output/report.md`, `summary.json`, serta
`{baseline,fixed,stress}_{daily,weekly}.csv`. Semua output dibuat ulang.
**Usulan `fixed` belum lolos semua kriteria Lampiran I.** Jangan gunakan
nama berkas sebagai klaim bahwa ekonomi sudah tervalidasi.

## Struktur dan konfigurasi

- `config/baseline_*.json`: populasi, harga/IBN, produksi, progresi, keuangan.
- `config/fixed.json`: override rekursif usulan, bukan mengganti PRD diam-diam.
- `config/stress.json`: override populasi awal 60% K1 / 20% K2 / 20% K3.
- `agents/`: state, izin, pembelian paket, fixture populasi.
- `market/`: urutan pembeli menurut harga bersih, persediaan dan kas terbatas.
- `model/`: scheduler, produksi, formula, pajak, utang dan sitaan agregat.
- `report/`: metrik, stress test analitis, CSV/Markdown; `tests/`: unittest.

Ubah JSON lalu jalankan ulang. Tidak ada dependency numpy. Semua berkas
Python di bawah 100 baris. `main.py` dapat dijalankan dari direktori mana pun.

## Asumsi yang tidak ditetapkan PRD

1. Modal Rp1 juta + hadiah tutorial Rp12 juta; hadiah ini sengaja ditandai
   asumsi, karena PRD tidak menetapkan nominal hadiah. Kedua skenario sama.
2. 1.000 agen awal tepat 20/50/30%; masuk 20/hari (2% populasi awal),
   total 2.780 identitas. Retensi log-linear antar D1 .35/D7 .12/D30 .05,
   datar setelah D30; churn permanen, tanpa reaktivasi. Ini bukan selalu
   1.000 pemain serentak: batas kota PRD dan target retensi perlu klarifikasi.
3. Sesi tersebar merata, antrean maksimal 8 jam/sesi: aktif dan sedang
   maksimal 24 jam/hari, santai 8. Armada awal satu, trip berjalan otomatis.
   Fraksi barang gratis, kecepatannya dan reputasi per setoran adalah asumsi.
4. Semua barang HMS1; campuran gratis/NPC dihitung sebagai rata-rata biaya.
   Tidak ada event cuaca, reuse, K3 safety, pelindung barang gelap atau veteran.
   Barang gelap diuji sebagai EV terpisah, bukan kebijakan agen utama.
5. Waktu agregasi sehari nyata; IBN dihitung 144 tick/hari lalu harga rata-rata
   dipakai transaksi. Acak harian sama untuk satu hari nyata; efek pasokan
   memakai output kemarin terhadap target 1.000 ton, bukan harga endogen.
6. Agen beli armada berurutan, renovasi saat cukup kas plus cadangan kerja,
   izin dibayar terpisah sebelum paket. Izin subhari efektif hari berikutnya.
   Paket K2 sudah memuat sewa lahan; K3 memuat lahan industri dan jembatan.
7. Pekerja dibayar 6 hari game/hari nyata, satu shift; belum ada Mandor.
   Susut bandar 3%, EAF 10%. Transport trailer Rp6 juta termasuk sewa,
   diprorata kapasitas20t. Biaya jarak seller linear selisih distrik.
8. AI bandar menjual ke smelter pada 110% acuan, maksimal60t/distrik/hari:
   ini asumsi tambahan untuk jaring pengaman setiap kasta (harga tidak di PRD).
   Agen K1 tidak menjual langsung ke smelter; lapak satelit belum disimulasikan.
9. Sitaan mengurangi buku aset agregat; semua aset habis menghapus armada,
   lalu downgrade jika tetap berutang. Bukan lelang individual alat/bangunan.
   B2G pertama berarti izin K3 dibayar, akses terbuka setelah15hari, dan ada
   produksi aktual; kompetisi tender/asosiasi tidak dimodelkan.

## Definisi metrik dan batas bukti

- `daily.csv`: pemain aktif per kasta, IBN, pasokan AI/pemain (kg), ingot,
  uang beredar (kas positif seluruh identitas termasuk churn) dan utang.
- Pasokan adalah **transaksi input K2/K3 menurut penjual langsung**, bukan
  asal geologis material. Pengumpulan K1 dari NPC tidak masuk denominator.
  Satu kg bisa tercatat di dua tingkat; hasil ini bukan pangsa asal kota.
- Hari tanpa permintaan adalah N/A, tidak diberi nilai 0% untuk meluluskan AI;
  kriteria AI ketat tidak lolos jika ada satu hari N/A.
- Laba median per kasta = median agen-hari setelah OpEx, pajak dan bunga,
  sebelum belanja modal. Hari menganggur tetap masuk (penting untuk bandar).
- Tunggu15t = interval pengiriman yang selesai, resolusi1hari; bandar yang
  tidak pernah mencapai batch tersensor dan tidak masuk median ini.
- Tonggak = umur sejak bergabung, median yang mencapai plus pembilang/penyebut
  kohor aktif awal. Churn tidak dianggap sukses; median bukan seluruh kohor.
- `weekly.csv`: pertumbuhan kas positif dan perubahan IBN terpisah. Minggu
  pertama membandingkan akhirH1→H7 (6interval); berikutnya 7interval.
  Hari85–90 tetap ada di daily tetapi belum menjadi minggu penuh.
- Stress 20%K3 memberi modal/izin awal untuk isolasi rantai pasok; **bukan**
  bukti bahwa progresi alamiah mencapai20%K3. Analisis piramida mengasumsikan
  produktivitas per peran sama; bukan otomatis prediksi persentase tonase.

Lihat `../docs/ECONOMY_SPEC.md` untuk angka dan masalah yang masih terbuka.
