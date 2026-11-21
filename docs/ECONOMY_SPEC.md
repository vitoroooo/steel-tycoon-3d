# Spesifikasi ekonomi Fase 0 — Steel Tycoon 3D

## Status keputusan

**BELUM LOLOS Lampiran I.** Parameter usulan di bawah adalah hasil eksperimen,
bukan angka rilis final tervalidasi. Tidak ditemukan set parameter yang memenuhi
semua syarat dalam iterasi ini; kegagalan dipertahankan di laporan, bukan ditutupi
dengan grant besar, menghapus pemain miskin dari median, atau menyebut IBN sebagai
inflasi uang. `sim/main.py` menjalankan baseline, usulan dan stress20%K3.
Sumber persyaratan: PRD§4/5/6.7/7/8.2/9.1/11.1 dan Lampiran A/B/I.

## Angka usulan yang diterapkan (rupiah, kg, hari nyata kecuali disebut)

| Parameter | Awal | Usulan |
|---|---:|---:|
| Beli NPC / acuan | 60% | 72% |
| Beli Pabrik AI dari K1 | 70% | 70% |
| Beli bandar dasar / respons stok | 85,5% / 3% | 82% / 3% |
| Beli Pabrik AI dari bandar | 90% | 115% |
| Beli smelter pemain | 100% | 120% |
| AI bandar → smelter | 110% | 125% |
| Kuota pengepul AI / distrik / hari | 40.000 | 10.000 |
| Kapasitas permintaan bandar / hari | 40.000 | 100.000 |
| Trailer20t, sewa+BBM | 6 juta | 3 juta |
| Fraksi barang gratis / kecepatan relatif | 30% / 50% | 90% / 100% |
| Gerbang kas K2 / K3 (izin dibayar terpisah) | 250 juta / 3 miliar | 500 juta / 3,6 miliar |
| Bunga per hari game | 2% | 0,2% |
| Barang gelap: pengali laba / risiko dasar | 3–5 / 10% | 3,541475 / 2% |

Usulan sengaja mengubah beberapa angka besar, bukan mengklaim ini perubahan kecil:
barang gratis cepat dan floor AI115% adalah subsidi kuat. Belum layak produksi.
Tetap: pajak laba5/10/15% tiap15hari, transaksi2%, IBN basis10.000,
siklus±15%/60hari, acak±3%, efek pasokan±5%, acuan55%, susut3%/10%,
OpEx EAF2.000/kg, gaji Lampiran B, paket210juta dan2,95miliar, AMDAL750juta.
Modal/tutorial dan kapasitas dasar yang tidak ada di PRD dijelaskan di README.

## Bukti numerik sebelum / sesudah (seed73)

| Metrik | Baseline | Usulan |
|---|---:|---:|
| Arbitrase pikap NPC→AI sesudah BBM, Rp/kg | +400 | -260 |
| Median aktif: triseda | H1 | H1 |
| Median aktif: pikap (yang mencapai) | H13 (21/200) | H5 (46/200) |
| Median aktif: K2 (yang mencapai) | H28 (12/200) | H14 (20/200) |
| Median aktif: cor+hanggar | Tidak tercapai | H17 (16/200) |
| Median aktif: K3 / B2G | Tidak tercapai | Tidak tercapai |
| Median laba K1 / hari | 2,360 juta | 33,689 juta |
| Median laba K2 / hari | -16,425 juta | +7,972 juta |
| Median laba K3, season alami | Tidak teramati | Tidak teramati |
| Pangsa AI maksimum H1–60, jika ada permintaan | 35,19% | 0% |
| Minggu terburuk: pertumbuhan uang | 287,81% | 746,97% |
| Utang tanpa pembayaran selama7hari | 2,2972× | 1,0875× |
| Utang tanpa pembayaran selama30hari | 35,3208× | 1,4328× |

Angka tabel harus dibaca bersama denominator dan asumsi `sim/output/report.md`.
Median tonggak teramati tidak memenuhi makna median seluruh kohor yang churn.

## Lima masalah PRD

1. **Arbitrase positif, bukan uang tak terbatas per waktu.** 5500×(0,70−0,60)
   −150=400/kg sebelum pajak laba/gaji. Jalur benar-benar dipakai agen K1 saat
   tidak ada bandar (semua pemain mulaiK1). Satu pikap maksimal24trip/hari:
   laba spread9,6juta/hari, sebelum gaji1,2juta dan pajak. Akumulasi tak terbatas
   hanya jika waktu tak terbatas; kapasitas, antrean dan modal membatasi laju.
   Usulan −260/kg menghapus loop beli-jual itu, bukan keuntungan pungut gratis.
2. **Piramida 20:8:1 berarti160K1+8K2+1K3**, jadi K3 hanya1/169=0,592%.
   Untuk200smelter dibutuhkan33.600peran pemasok, sementara tersisa800pemain:
   kekurangan97,62%peran jika produktivitas sama, bukan bukti tonase97,62%AI.
   Stress terpisah60/20/20% memberi AI maksimum63,63%, benar melewati50%;
   laba K3 median62,672juta/hari. Season alami belum mencapai keadaan itu:
   kesimpulan sebab hanya terverifikasi di fixture20%K3, bukan di season alami.
3. **EV perlu definisi nilai muatan dan urutan heat.** Model memakai laba legal
   L=4700−3300=1400, omzet gelapR=3300+mL; EV=(1−p)(R−3300)
   −p(3300+3R). Padaheat0, EV awal3/4/5× adalah0,857/1,457/2,057×L.
   Jika +15heat diterapkan sebelum razia pertama: −0,75/−0,45/−0,15×L.
   Usulan risiko2%+heat/200 dan m3,541475 memberi1,3×L padaheat15,
   −2,829×L padaheat50; heat0masih3,070×L. Pilih urutan heat sebelum rilis.
4. **Utang2%/hari game adalah12,616%/hari nyata**, bukan2%/hari nyata.
   Santai hanya mendapat8jam antrean vs24jam aktif, tetapi bunga tetap6kali.
   Stress utang tabel mengasumsikan tidak dibayar, bukan prediksi semua santai
   bangkrut. Turun0,2% memberi1,206%/hari nyata; lebih aman namun masih perlu
   masa tenggang, batas bunga, dan uji shock kas dalam playtest.
5. **Target progresi gagal.** Usulan memindahkan K2 teramatiH28→H14, tetapi
   hanya20/200aktif awal yang mencapai; tidak seorang pun mencapaiK3 di90hari.
   Klaim median aktifK3H45–55 belum dapat dibuat. RetensiD1=35% sendiri
   membuat median kohor awal tidak bisa mencapai tonggak lanjut tanpa reaktivasi.

## Keputusan lanjutan yang wajib sebelum angka final

- Tentukan apakah20%K3 target populasi serentak atau kohor; PRD§17 malah10%H60.
- Definisikan median pemain aktif sebagai survivor/engaged, bukan semua akun.
- Tentukan sistem treasury/escrow dan batas faucet: menaikkan margin mempercepat
  progresi tetapi memperburuk pertumbuhan kas. Jangan tambahkan kas beku besar
  hanya untuk mempercantik denominator inflasi5%.
- Uji lapak satelit, staf sesuai shift, reuse dan kontrak sebelum menyimpulkan
  target mustahil; model sekarang sengaja membatasi cakupan, bukan bukti mustahil.
- Butuh kalibrasi lanjutan untuk K3H45–55, median laba semua kasta, dan inflasi.
  Status ini berarti permintaan set parameter yang lolos **belum selesai**.
