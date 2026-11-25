# Kalibrasi ekonomi karier mobile

**0 dari 10 seed uji lolos seluruh kriteria.** Pada kandidat terpilih, median laba bandar negatif di tujuh kota, dua kota menghasilkan laba bandar positif dan satu kota tidak mencapai bandar. Hanya satu kota mencapai smelter. Ambang ketahanan 8/10 belum tercapai.

## Harga dan alur pembukaan

Profil `mobile` membaca harga dari sumber Unity di `unity/Assets/_Project/Resources/Config`. Model kota, retensi, pajak dan asumsi K3 mengikuti skenario `fixed`. Angka berikut berlaku pada basis `f30ac574`.

| Komponen | Perhitungan simulator |
|---|---|
| Kas dan tutorial | Pemain mulai dengan Rp500 ribu. Agen menawarkan tumpukan awal 12 kg. Penjualan pertama memungkinkan klaim Rp12 juta satu kali di dealer. Hadiah masuk kas saja. |
| Armada dan rute | Gerobak 50 kg, triseda 300 kg/Rp12 juta, pikap 1.000 kg/Rp85 juta. Rute pungut membawa 40% kapasitas. Rute beli membayar berat penuh. |
| Harga lokal | Agen membandingkan pabrik AI, bandar pemain dan loket Pasar/Bengkel. Loket HMS1 membayar 86%/83% acuan dikurangi ongkir dari rumah Rp3/kg/meter. |
| Reputasi | Setelah penjualan, minimum antara 100 dan 1 + lantai(total kg terjual / 100). Belum menjual berarti reputasi nol. |
| Izin dan buka bandar | Izin Rp15 juta, proses lengkap 24 jam game. Sewa Rp20 juta berlaku 720 jam game (5 hari nyata). Perlu reputasi 30 dan kas Rp250 juta sesudah membayar sewa. |
| Alat dan modal kerja | Timbangan + loader Rp175 juta. Membuka pada ambang minimum menyisakan Rp75 juta. Cor + hanggar Rp260 juta. |
| Operasi bandar | Agen memperpanjang sewa jika kas cukup. Bandar yang sewanya berakhir masih bisa mengirim stok lama. Sewa dibayar di muka dan dibebankan sepanjang masa sewa. Satu kiriman 15 ton mentah membayar Rp3 juta. Ambang pemasok NPC HMS1 sebesar 86% acuan mengikuti `LocalGameService.CounterRate`. |

Pada acuan Rp5.500/kg, pikap yang membeli HMS1 di NPC dan mengirim ke loket terbaik mendapat margin sekitar **Rp458/kg** setelah BBM dan ongkir, sebelum biaya harian. Pengukuran pada pembeli pabrik saja melewatkan peluang ini.

## Kriteria tetap dan denominator

- Satu run mengikuti 1.000 agen awal selama 90 hari nyata dengan 20 pendatang per hari sejak H2. Populasi H90 mencakup semua pemain yang masih aktif.
- Laba harus positif pada semua kasta. Ukurannya median laba agen-hari per kasta setelah biaya, pajak dan bunga, termasuk hari menganggur. Belanja modal dicatat di aset. Kasta tanpa observasi menghasilkan N/A dan gagal.
- Pasokan AI harus <50% setiap hari berinput pada H1–60. Denominator adalah seluruh kg input K2/K3 menurut penjual langsung. Kg bisa berpindah dua tahap. Seluruh periode tanpa input gagal.
- Inflasi harus <5% pada setiap minggu penuh setelah H14 (H21–84). Ukurannya pertumbuhan jumlah kas positif seluruh identitas termasuk kas churn yang dibekukan. Perubahan harga IBN dilaporkan terpisah.
- Progresi mengharuskan H90 berisi 15–25% K2 dan 3–7% K3, serta median waktu ke K2 aktif H10–14 dan santai H40–50. Kohor waktu berisi akun awal per tipe yang bertahan sampai H90. Yang belum mencapai dihitung +∞. Median tak teridentifikasi ditulis N/A.
- Target memakai revisi Putaran 2 di `sim/config/baseline_policy.json`. Riwayat keputusan mengganti target asli Lampiran I PRD dan pengujian seed 73–82 tersedia di [arsip Putaran 2](../sim/docs/putaran-2.md).

## Eksperimen dan pemilihan

Seed 73 dipakai untuk pemeriksaan awal. Grid melatih 48 kombinasi dan satu kontrol pada seed 73–78. Lima sumbu grid adalah tawaran bandar (83/86% acuan), plafon pembelian NPC K1 per kecamatan mulai H15 (8/32/64 ton), porsi perjalanan pungut mulai H15 (30/80%), tambahan cadangan kas sebelum membuka bandar (Rp0/25 juta), serta biaya omzet K1/K2/K3 (1/8/12% atau 1/3/7%). Harga mobile dan batas penerimaan dikunci pada semua kombinasi.

Pemilihan mengutamakan jumlah seed lolos gabungan, kemudian jumlah pemeriksaan utama yang lolos, lalu jumlah komponen laba dan progresi yang lolos. Seri mengikuti urutan grid. Hanya kontrol dan pemenang latih yang dijalankan pada sepuluh seed terpisah 173–182. Total 314 pasangan kebijakan–seed.

`policy-18` memilih tawaran 83%, plafon K1 64 ton, perjalanan pungut 30%, cadangan tambahan Rp0 dan biaya omzet 1/3/7%. Tidak ada kandidat yang lolos gabungan pada satu pun seed latih.

| Skenario | Laba | AI | Inflasi | Progresi | Gabungan |
|---|---:|---:|---:|---:|---:|
| Kontrol mobile, latih | 0/6 | 0/6 | 6/6 | 0/6 | 0/6 |
| Kandidat, latih | 0/6 | 6/6 | 6/6 | 0/6 | 0/6 |
| Kontrol mobile, uji | 0/10 | 0/10 | 10/10 | 0/10 | 0/10 |
| Kandidat, uji | 1/10 | 7/10 | 6/10 | 0/10 | 0/10 |

| Metrik uji kandidat | Hasil pada seed 173–182 |
|---|---|
| Laba median K1 | Rp1,704 juta/agen-hari (median dari 10 median seed) |
| Laba median K2 | −Rp1,855 juta/agen-hari (9 seed teramati, rentang −Rp1,908 juta hingga +Rp2,087 juta) |
| Laba median K3 | +Rp34,958 juta/agen-hari pada seed 174, N/A di sembilan seed lainnya |
| Mix H90 | Median K2 12,26% (0–18,65%), K3 0% (0–1,75%) |
| Waktu ke K2 | Aktif H15 di tujuh seed, N/A di tiga. Santai N/A di semua seed. |
| Pasokan AI maksimum | 0% di tujuh seed berinput. Tiga seed tanpa input H1–60 gagal. |
| Pertumbuhan uang mingguan maksimum | Median maksimum 4,14%, rentang 3,29–8,07%. Empat seed melampaui batas. |

Pada kontrol seed 73, kas beredar naik dari Rp500 juta saat pembukaan menjadi Rp21,27 miliar pada akhir H7 (+4.154%). Hadiah tutorial dan arus kas transaksi sudah masuk dalam angka tersebut. Kriteria inflasi mingguan mulai dinilai setelah H14.

**Sensitivitas.** Dengan sumbu lain tetap pada `policy-18`, menurunkan plafon K1 dari 64 ke 32 lalu 8 ton menurunkan median maksimum inflasi latih dari 4,06% ke 2,02% lalu 0,95%. Ketiganya masih gagal laba dan progresi di 6/6 seed. Menaikkan porsi perjalanan pungut dari 30% ke 80% membuat median K2 santai masuk H48–49,5 pada lima seed, tetapi porsi median K2 H90 menjadi 43,95% dan inflasi gagal di seluruh seed (median maksimum 16,39%).

## Reproduksi dan batas model

Jalankan dari akar repositori dengan Python 3.10 (pengukuran menggunakan 3.10.12).

```sh
cd sim
python -m unittest discover -s tests
python main.py
python tools/mobile_calibration.py
```

- `config/mobile_calibration.json` memuat grid dan pembagian seed. `output/mobile_trials.jsonl` menyimpan semua observasi termasuk median null serta pembilang/denominator kohor. `output/mobile_calibration.json` menyimpan skor, parameter dan hitungan kelulusan. CSV harian/mingguan profil mobile dari `main.py` memakai kontrol seed 73.
- Resolusi model satu hari nyata, dengan transaksi tutorial di lima menit pertama. Agen memilih mengurus izin setelah membeli pikap. Izin subhari siap pada langkah harian berikutnya. Waktu bermain atau perjalanan dealer di perangkat belum diukur.
- Benchmark memakai harga satu grade HMS1 untuk seluruh stok. Di mobile, rute gratis menghasilkan besi campur. Lot per grade, kuota rute per pemain, pelapukan, cuaca, antre bongkar, tender serta kejadian karier belum dimodelkan. Sumber rongsok dan pembeli NPC dibatasi per kecamatan. K2 mengalokasikan lot penuh menurut urutan penjual yang diacak per hari.
- K3 dan transaksi antaragen memakai asumsi ekonomi kota asinkron. Eksperimen ini mengukur perilaku model pada harga mobile. Validasi sesi permainan nyata dan biaya dari integrasi karier perlu pengukuran terpisah.
