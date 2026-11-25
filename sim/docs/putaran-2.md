# Arsip simulasi Fase 0 — Putaran 2

## Status dan perubahan target PRD
**Seed73 lolos empat pemeriksaan; BELUM robust untuk rilis: hanya1/10 seed lolos seluruhnya.**
Sumber: PRD§4/5/6.7/7/8.2/9.1/11.1 dan Lampiran A/B/I; PRD asli tidak diubah.
Keputusan baru menggantikan median aktif K3 H45–55 dan sasaran20%K3:
- H90: sekitar20%K2 dan5%K3, sisanyaK1. Toleransi eksplisit15–25% dan3–7%.
- Aktif K2 H10–14; santai sekitarH45 (toleransiH40–50); tidak ada deadline K3.
- Pertumbuhan uang <5%/minggu setelah minggu2; pasokan AI <50% tiap hari berinput H1–60.
- Median laba positif di setiap kasta; bunga2% per hari NYATA, bukan hari game.
Batas penerimaan ada di `config/baseline_policy.json`, tidak diubah oleh sweep.

## Denominator dan akuntansi
Populasi H90 = semua agen yang masih aktif, termasuk pendatang; bukan1000 akun awal.
Seed73:184 pemain aktif, K1/K2/K3=137/39/8 (74,46%/21,20%/4,35%).
Tonggak = umur hari sejak bergabung, kohor lahirH1 yang tetap aktif sampaiH90.
Aktif berarti tipe `aktif`, bukan semua tipe yang online. Denominator11 survivor dari200
akun aktif awal; santai11 survivor dari300. Masing-masing11/11 mencapaiK2 (H11/H44).
Pendatang dan churn dikeluarkan; survivor belum mencapai bernilai+∞, tidak dibuang.
Median null bila tidak teridentifikasi. Ini median survivor, BUKAN median seluruh akun.
Uang = jumlah kas positif semua identitas termasuk kas churn yang dibekukan; utang terpisah.
Tidak ada tambahan kas beku untuk memperbesar denominator. Minggu1 pembukaanH1→H7,
berikutnya7hari; pemeriksaan H21,28,…,84. H85–90 bukan minggu lengkap.
Laba = median agen-hari per kasta termasuk menganggur, sesudah OpEx/pajak/bunga,
sebelum capex; kehilangan likuiditas karena pembelian paket bukan biaya laba sekaligus.
Pasokan AI = input K2/K3 menurut penjual langsung, bukan asal geologis; kg bisa dihitung
di dua tahap. Pungut K1 tidak masuk denominator. Hari tanpa input N/A, bukan0%; tidak ada
ketergantungan AI hari itu. Semua-N/A atau kurang60hari observasi tidak bisa lolos.

## Parameter akhir (override fixed.json; Rp, kg, hari nyata)
| Parameter | PRD/baseline | Fixed |
|---|---:|---:|
| NPC jual / AI beli K1, fraksi acuan | 0,60 / 0,70 | 0,72 / 0,70 |
| Bandar / AI beli K2 / smelter / AI industrial | 0,855 / 0,90 / 1 / 1,10 | 0,70 / 1 / 1 / 1,06 |
| Respons stok / biaya jarak per distrik | 0,03 / 0,008 | 0,03 / 0,002 |
| EAF input per hari game (6/hari nyata) | 30t | 10t |
| Kapasitas bandar / hari nyata | 40t | 100t |
| Sumber K1 / kecamatan / hari | praktis tak dibatasi | 500t |
| NPC beli K1/K2/K3 per kecamatan H1–14 | praktis tak dibatasi | 500t / 100t / 120t |
| NPC beli K1/K2/K3 per kecamatan H15+ | praktis tak dibatasi | 8t / 40t / 60t |
| AI suplai K2 / K3 per kecamatan / hari | 40t / 60t | 10t / 60t |
| Maintenance nilai buku aset / hari | 0 | 0,3% |
| Biaya layanan omzet K1/K2/K3 | 0 | 1% / 8% / 12% |
| Sewa bandar per5hari | 20juta | 25juta |
| Staf | penuh | proporsional utilisasi, maksimum1shift |
| Gerbang kas K2 / K3 | 250juta / 3miliar | 190juta / 250juta |
| Paket K2 / K3; izin K2 / K3 | 210juta / 2,95miliar;15/750juta | 100/150juta;15/50juta |
| Reputasi K2/K3; cor+hanggar | 30/70;680juta | 42/70;120juta |
| Fraksi pungut gratis H1–14 / H15+; kecepatan | 30%;50% | 90% / 30%;100% |
| Trailer20t termasukBBM | 6juta | 3juta |
| Bunga per hari nyata | 1,02^6−1 =12,616% | 2% |
Tetap: transaksi pemain2%, pajak laba5/10/15% tiap15hari, susut3%/10%, EAF OpEx2000/kg.
Reputasi per sesi setoran1 (awal0,5). Ambisi aktif100%K3; sedang30%K1/70%K2;
santai100%K2. Ini preferensi maksimum, bukan promosi timer atau pemaksaan kuota H90.
Paket murah adalah perubahan besar/asumsi mikro-EAF/sewa, bukan harga penuh pabrik PRD;
tidak ada utang tersembunyi. Persetujuan desain dan playtest tetap diperlukan.
Pembelian NPC berbagi plafon kasta/kecamatan/hari, alokasi sama per penjual aktif.
Persediaan tak terjual dan basis biaya disimpan, bukan dimusnahkan atau dibayar dua kali.
Sumber, input AI, output NPC dan hadiah tutorial per akun semuanya terbatas kapasitas.
Modal13juta/akun,20 pendatang/hari, retensi dan asumsi agregasi lain lihat README.

## Sebelum / sesudah — seed73, evaluator revisi yang sama
Baseline dijalankan ulang dengan mesin sekarang, bukan salinan hasil commit Putaran1.
| Metrik | Baseline | Fixed |
|---|---:|---:|
| K2 / K3 H90 | 27,72% / 0% | 21,20% / 4,35% |
| Median survivor aktif / santai K2 | H28 / H64 | H11 / H44 |
| Median laba K1 / K2 / K3, juta/hari | 2,360 / −16,425 / N/A | 0,691 / 0,548 / 7,126 |
| Maksimum pasokan AI H1–60 | 35,19% | 7,06% |
| Maksimum pertumbuhan uang setelahH14 | 90,92% | 3,83% |
| Arbitrase pikap NPC→AI, Rp/kg setelahBBM | +400 | −260 |
| Utang tanpa pembayaran H7 / H30 | 2,2972× / 35,3208× | 1,1487× / 1,8114× |
Rasio lama20:8:1 tetap hanya0,592%K3 jika produktivitas sama; bukan bukti tonase AI.
EV barang gelap tetap uji analitis: pengali3,541475 dan risiko dasar2% memberi1,3×
laba legal padaheat15, negatif padaheat50; hasil bergantung urutan penambahanheat.

## Robustness: sepuluh seed tetap dan tiga perubahan kecil
Daftar tetap: **73,74,75,76,77,78,79,80,81,82**; masing-masing1000agen/90hari.
Reproduksi: `python sim/tools/sweep.py`; `python sim/tools/robustness.py`.
Grid dan seed di calibration.json; pengukuran mentah di output/{sweep,robustness}.jsonl.
| Varian | Profit | AI | Inflasi | Progresi gabungan | Semua |
|---|---:|---:|---:|---:|---:|
| Kandidat dipertahankan | 2/10 | 10/10 | 10/10 | 1/10 | 1/10 |
| Biaya omzet K2/K3 6%/10% | 6/10 | 10/10 | 7/10 | 3/10 | 2/10 |
| Ambisi sedang60%K2 | 3/10 | 10/10 | 10/10 | 3/10 | 3/10 |
| Kedua perubahan sekaligus | 8/10 | 10/10 | 5/10 | 5/10 | 1/10 |
Tidak ada tweak mencapai≥8/10 tanpa merusak kriteria lain; kandidat lama dipertahankan.
AI/inflasi lolos10/10; laba dan komposisi belum robust. Seed74: laba K2−0,383juta,
K3−0,716juta dan K3H90=1,87% (<3%). Seed81 tidak pernah menghasilkanK3 (laba N/A).
Seluruh seed mencapai median aktifH11 dan santaiH44–47; kegagalan progresi ada di mix.
Bukti ini menunjukkan sensitivitas populasi/likuiditas, bukan diagnosis kausal tuntas.
Jangan menyebut konfigurasi ini lolos8/10 atau siap produksi; perlu desain/calibrasi lanjutan.
