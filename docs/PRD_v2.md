# PRODUCT REQUIREMENTS DOCUMENT (PRD) v2
**Game Title:** Steel Tycoon 3D: The Real Syndicate
**Versi dokumen:** 2.1 (pematangan konsep dari Master PRD v1, termasuk season Ekspansi Kota dan lampiran desain lengkap)
**Genre:** Simulasi Manajemen Ekonomi Multiplayer (asinkron)
**Platform:** Mobile-first (Android lalu iOS). PC menyusul setelah mobile terbukti.
**Kamera:** 3D isometrik/free-cam gaya city-builder, landscape, penempatan grid
**Gaya visual:** Stylized industrial yang ringan dulu (low-poly bertekstur, nuansa gritty). Fotorealistik jadi target jangka panjang.

---

## 0. Cara membaca dokumen ini

Dokumen ini menggantikan PRD v1. Isinya tetap visi yang sama (dari gerobak sampai smelter), tetapi setiap sistem sekarang punya angka, aturan, dan batas cakupan supaya bisa dikerjakan tim.

### 0.1 Asumsi dasar (berlaku sampai Anda koreksi)

| # | Asumsi | Alasan |
|---|---|---|
| A1 | Pasar pertama Indonesia, tim kecil (indie, 3 sampai 8 orang) | Tema sangat lokal; cakupan harus muat untuk tim kecil |
| A2 | Pemain mengatur dari atas, tidak menyetir kendaraan | Menghapus kebutuhan fisika kendaraan dan kontrol menyetir di HP |
| A3 | Multiplayer asinkron dulu; harga baja global ditentukan server | Hampir semua interaksi (pasang harga, setor, lelang) tidak butuh real-time |
| A4 | Grafis 3D ringan dulu, fotorealistik belakangan | Mayoritas HP di Indonesia kelas menengah |

### 0.2 Ringkasan perubahan dari v1

| Topik | v1 | v2 |
|---|---|---|
| Ekonomi | "Sepenuhnya player-driven" | Ekonomi hibrida: harga indeks dari server, harga lokal dari pemain, pembeli/penjual AI sebagai jaring pengaman |
| Peran pemain | Campuran SnowRunner dan city-builder | Manajer dari atas; kendaraan dijalankan AI |
| Waktu | Tidak didefinisikan | 1 hari game = 4 jam nyata (Bagian 4) |
| Multiplayer | MMO real-time | Asinkron; satu kota per season |
| Kasta 1 ke smelter | Kontradiktif | Kasta 1 tidak bisa jual langsung ke smelter; smelter menyerap Kasta 1 lewat lapak satelit miliknya |
| Contoh reuse | 15 ton baut oleh pengepul kecil | Temuan reuse diskalakan per kasta |
| Margin | Satu contoh tanpa susut | Tabel margin per kasta, termasuk susut, angkut, pajak |
| Lumpur | Fisika sungguhan | Status lahan (kering/becek/lumpur) dengan efek angka |
| Season | (tidak ada) | Season 90 hari = "Ekspansi ke Kota Baru". Usaha di kota lama tidak dihapus, menjadi bagian Grup Usaha permanen (Bagian 11.3) |
| Monetisasi | VIP bocoran harga 24 jam, skip timer izin | Dihapus. Diganti analitik historis, kosmetik, season pass kosmetik |
| Nama institusi | Nama asli (AHU, OSS, PLN, Polisi, Ormas) | Nama fiktif (Bagian 15) |
| Cakupan | Semua sekaligus | 4 fase, MVP jelas (Bagian 17) |

---

## 1. Visi dan pilar desain

**Hook:** Mulai dari gerobak dorong di gang sempit, bangun kerajaan besi tua sampai punya smelter sendiri, sambil berhadapan dengan izin, preman, hujan, dan pesaing yang juga pemain sungguhan.

**Pilar desain** (setiap fitur baru harus mendukung minimal satu):
1. **Keputusan ekonomi yang terasa nyata.** Setiap kilo punya harga, setiap jalan pintas punya risiko.
2. **Saling butuh antar pemain.** Kasta bawah memasok, kasta atas membeli. Tidak ada yang bisa naik sendirian secepat yang bekerja sama.
3. **Dunia yang membalas.** Cuaca, warga, kelompok preman, dan aparat bereaksi terhadap keputusan pemain.
4. **Ramah sesi pendek.** Satu sesi 5 menit selalu menghasilkan kemajuan; sesi panjang memberi optimasi.

**Target pemain:** Pria dan wanita 18 sampai 35 tahun di Indonesia, pemain game manajemen/tycoon di HP, suka angka dan strategi, akrab dengan budaya lapak dan usaha kecil.

**Pembanding dan pembeda:**
| Game | Yang diambil | Pembeda Steel Tycoon |
|---|---|---|
| Game tycoon idle mobile | Kemajuan saat offline, sesi pendek | Ekonomi antar pemain sungguhan, bukan angka naik sendiri |
| Simulasi logistik/industri PC | Kedalaman rantai produksi | Bisa dimainkan di HP, tema lokal |
| Game kota multiplayer asinkron | Server bersama, perdagangan | Hierarki kasta yang memaksa transaksi |

---

## 2. Keputusan desain inti (menyelesaikan kontradiksi v1)

1. **Ekonomi hibrida.** Server menetapkan **Indeks Baja Nasional (IBN)**, harga acuan baja ingot per kg. Pemain menetapkan harga beli/jual lokal di atas acuan itu. NPC menjadi sumber uang (faucet) dan pembuangan uang (sink) yang tercatat jelas (Bagian 7.4).
2. **Pemain adalah manajer.** Pemain memberi perintah (rute, muatan, harga, konstruksi). Kendaraan dan pekerja dijalankan AI. Tidak ada mode menyetir.
3. **Satu jam dunia per kota.** 1 hari game = 4 jam nyata. Semua timer memakai satuan ini.
4. **Satu kota per season, asinkron.** Satu kota fiktif berisi maksimal 1.000 pemain aktif (kalau pemain lebih banyak, dibuka beberapa kota paralel), dibagi 6 sampai 10 kecamatan. Interaksi antar pemain lewat papan harga, setoran, kontrak, dan kejadian; tidak perlu online bersamaan.
5. **Season = Ekspansi ke Kota Baru.** Satu season = 90 hari nyata. Di akhir season, usaha pemain di kota itu "tutup buku" dan menjadi bagian **Grup Usaha** permanen; season berikutnya pemain membuka cabang baru dari Kasta 1 di kota baru dengan bekal warisan. Tujuannya: lapisan bawah rantai pasok selalu terisi lagi tanpa ada yang dihapus (Bagian 11.3).
6. **Akses smelter.** Smelter hanya menerima setoran minimal 5 ton dari pemegang PT. Kasta 1 tidak bisa setor langsung. Smelter yang ingin menyerap Kasta 1 harus membuka **lapak satelit** (bangunan Kasta 2 milik smelter).
7. **Jaring pengaman AI di setiap kasta.** Selalu ada pembeli AI dengan harga di bawah pemain dan penjual AI dengan harga di atas pemain. Pemain selalu punya jalan keluar, tetapi pemain lain selalu lebih menguntungkan.

---

## 3. Core loop

### 3.1 Loop per level waktu

| Level | Durasi | Isi |
|---|---|---|
| Detik | 5 sampai 30 detik | Ketuk tumpukan untuk cek grade, kirim armada, terima setoran, tanggapi kejadian |
| Sesi | 3 sampai 10 menit | Kumpulkan hasil saat offline, sortir, jual, atur harga, antre pesanan untuk 8 jam ke depan |
| Harian (nyata) | 3 sampai 5 sesi | Ambil kontrak, upgrade alat, urus izin, hadapi 1 sampai 2 kejadian besar |
| Mingguan | | Naik kasta atau naik level usaha, pajak, peringkat kecamatan |
| Season | 90 hari nyata | Ekspansi ke kota baru, peringkat per kasta, hadiah kosmetik, Grup Usaha bertambah besar |

### 3.2 Sesi 5 menit per kasta

**Kasta 1 (Pengepul):**
1. Buka game, lihat ringkasan offline: berapa kg terkumpul, berapa uang.
2. Sortir muatan pikap: pisahkan besi, seng, tembaga, barang reuse (60 detik, mini-game seret-lepas).
3. Lihat papan harga lokal: bandar mana yang beli paling tinggi untuk tiap grade, berapa jarak dan ongkos jalan.
4. Kirim muatan ke bandar terpilih (atau ke Pabrik AI kalau terdesak).
5. Antre rute pengumpulan untuk 8 jam ke depan (pilih kecamatan, durasi).

**Kasta 2 (Bandar):**
1. Lihat setoran yang masuk dari pemain Kasta 1 dan pengepul AI.
2. Ubah harga beli per grade (naikkan kalau stok kurang, turunkan kalau gudang penuh).
3. Cek tumpukan: ada yang terkena hujan? Pindahkan ke hanggar.
4. Kalau stok sudah 15 ton atau lebih: ambil SPK dari smelter (pemain atau AI), kirim trailer.
5. Tanggapi kejadian (kelompok preman di gerbang, alat rusak).

**Kasta 3 (Smelter):**
1. Lihat antrean truk di gerbang dan stok bahan baku per grade.
2. Atur jadwal lebur EAF (siang/malam, kapasitas, biaya listrik).
3. Pasang harga beli smelter dan SPK terbuka untuk bandar.
4. Ambil atau kirim mega-kontrak pemerintah (B2G).
5. Urus asosiasi, keamanan, dan izin.

---

## 4. Waktu dan aturan offline

### 4.1 Skala waktu
| Satuan game | Nyata |
|---|---|
| 1 jam game | 10 menit |
| 1 hari game | 4 jam |
| 1 bulan game (30 hari) | 5 hari |
| 1 tahun game | 60 hari |

Siang game = 06.00 sampai 18.00 (2 jam nyata), malam = 18.00 sampai 06.00 (2 jam nyata).

### 4.2 Aturan saat pemain offline
| Sistem | Aturan |
|---|---|
| Produksi/pengumpulan | Berjalan sesuai antrean pesanan, maksimal 8 jam nyata (bisa naik ke 12 jam dengan Mandor senior) |
| Penjualan | Pesanan jual yang sudah dipasang tetap berjalan |
| Cuaca | Tetap terjadi. Kerugian stok saat offline dibatasi maksimal 10% nilai stok terbuka per hari game |
| Kelompok preman | Diselesaikan otomatis memakai pengaturan keamanan terakhir (bayar otomatis atau bertahan) |
| Pajak dan gaji | Ditagih otomatis; jika kas kurang, masuk utang (Bagian 11) |
| Kecelakaan K3 | Tidak bisa terjadi saat offline (pekerja hanya memakai alat yang statusnya aman) |

Notifikasi push hanya untuk: cuaca buruk dalam 1 jam nyata, gerbang diblokir, setoran besar masuk, kontrak hampir habis waktu.

---

## 5. Kasta rantai pasok (detail)

### 5.1 Kasta 1: Pengepul (Fase Jalanan)

| Armada | Kapasitas | Harga | Jangkauan per trip |
|---|---|---|---|
| Gerobak dorong | 50 kg | Gratis (awal) | 1 kecamatan, 30 menit nyata |
| Motor triseda | 300 kg | Rp 12 juta | 2 kecamatan, 45 menit nyata |
| Pikap bekas | 1 ton | Rp 85 juta | Seluruh kota, 60 menit nyata |

- **Sumber barang:** rumah tangga, bengkel, proyek bongkaran, pasar (NPC). Pemain membeli rongsok dari NPC atau memungut gratis (lebih lambat, grade lebih rendah).
- **Jual ke:** pemain Kasta 2, lapak satelit smelter, atau Pabrik AI (harga terendah).
- **Temuan reuse Kasta 1:** maksimal 50 kg per temuan (contoh: sekarung baut grade 8.8 utuh).
- **Syarat naik ke Kasta 2:** kas Rp 250 juta, reputasi 30, PT Perorangan + izin usaha jual-beli besi tua, sewa atau beli lahan lapak.

### 5.2 Kasta 2: Bandar Lapak

- **Infrastruktur awal:** lahan terbuka 1,6 ha (tanah), timbangan lantai 3 ton, loader kecil.
- **Papan Harga Beli:** bandar memasang harga beli per grade. Pemain Kasta 1 dan pengepul AI di kecamatan yang sama melihat dan memilih.
- **Gudang dan hoarding:** kuota kirim minimal 15 ton per trailer. Kapasitas lahan awal 60 ton, bisa diperluas.
- **Armada ekspedisi:** truk engkel 8 ton (beli Rp 320 juta atau sewa Rp 1,8 juta/trip), trailer flat-deck 20 feet 20 ton (sewa Rp 3 juta/trip, beli Rp 900 juta).
- **Jual ke:** smelter pemain lewat SPK, Pabrik AI, atau Toko Konstruksi NPC (barang reuse).
- **Temuan reuse Kasta 2:** sampai 2 ton per temuan, dari setoran yang disortir.
- **Syarat naik ke Kasta 3:** kas Rp 3 miliar (cukup untuk paket awal smelter, Lampiran A), reputasi 70, AMDAL disetujui, lahan industri, jembatan timbang truk.

### 5.3 Kasta 3: Smelter

- **Infrastruktur:** pabrik tertutup, EAF (kapasitas awal 30 ton/hari game), hydraulic shredder, jembatan timbang otomatis, gerbang RFID (upgrade).
- **Disrupsi:** smelter pemain memasang harga beli lebih tinggi dari Pabrik AI dan menarik pasokan bandar.
- **Lapak satelit:** smelter boleh membuka maksimal 2 lapak Kasta 2 untuk menyerap Kasta 1 langsung (dijalankan dengan aturan Kasta 2).
- **Produk:** ingot, H-beam, baut industri. Dijual ke pasar NPC (harga = IBN) atau kontrak B2G.
- **Temuan reuse Kasta 3:** sampai 15 ton per temuan (di sinilah contoh 15 ton baut dari v1 masuk akal).

---

## 6. Mekanika inti (revisi)

### 6.1 Grading dan sortir (Sort and Reuse)
Grade material:
| Grade | Contoh | Harga relatif terhadap acuan rongsok |
|---|---|---|
| HMS 1 | Plat/besi tebal bersih | 100% |
| HMS 2 | Besi tipis, campuran | 85% |
| Besi cor | Blok mesin, pipa cor | 90% |
| Rongsok campur | Belum disortir | 70% |
| Reuse | Baut grade 8.8/10.9 utuh, H-beam bekas lurus | 160 sampai 250% (dijual ke Toko Konstruksi NPC) |
| Kotoran | Tanah, plastik, karat berat | 0% (memotong berat bersih) |

- Setiap tumpukan baru masuk sebagai **"belum diperiksa"** dengan grade tersembunyi.
- **Cara menentukan grade:** periksa manual (mini-game seret-lepas, gratis, makan waktu), magnet tangan (Kasta 1, murah), alat XRF/spektrometer (Kasta 2 ke atas, cepat dan akurat).
- **Kotoran** di setoran mengurangi berat bersih. Bandar yang tidak memeriksa setoran bisa tertipu pemain lain yang mencampur kotoran (lihat 8.3).

### 6.2 Birokrasi dan izin
Izin adalah gerbang progresi, bukan skill tree:
| Izin (nama fiktif) | Syarat untuk | Biaya | Waktu proses |
|---|---|---|---|
| PT Perorangan + Kode Usaha | Kasta 2, akses SPK | Rp 15 juta | 1 hari game (4 jam nyata) |
| Izin Lingkungan Ringan | Lapak di atas 1 ha | Rp 50 juta | 2 hari game |
| AMDAL | EAF / Kasta 3 | Rp 750 juta | 3 hari game, ada mini-game kelengkapan dokumen |
| Sertifikat K3 | Kontrak B2G | Rp 200 juta | 2 hari game |

- **Mini-game portal perizinan:** teka-teki kelengkapan dokumen (cocokkan syarat dengan dokumen yang dimiliki), bukan isian formulir. Gagal = proses mundur 1 hari game.
- **Pajak:** dibayar per kuartal game (15 hari nyata). Basis = laba kuartal (bukan aset), tarif progresif 5% / 10% / 15% per kasta. Ini tidak menghukum investasi.
- **Pabrik tanpa AMDAL:** EAF tidak bisa dibangun sama sekali (bukan disegel belakangan), supaya pemain tidak kehilangan investasi besar karena lupa.

### 6.3 Faksi, kelompok preman, dan keamanan
- **Warga sekitar:** meteran **Kesabaran Warga** (0 sampai 100) per lokasi. Turun karena operasi malam (-2/jam game), antrean truk di jalan umum (-1 per truk per jam), debu shredder. Naik karena CSR (sumbangan, perbaikan jalan) dan gerbang RFID. Di bawah 20: blokade jalan 1 hari game.
- **Kelompok preman (fiktif):** kejadian pemerasan acak, peluang naik sesuai kekayaan dan rendahnya keamanan. Pilihan:
  1. **Bayar** "uang keamanan" bulanan: 2% omzet, kejadian berhenti 1 bulan game.
  2. **Negosiasi:** cek reputasi dan relasi; bisa turun ke 1% atau gagal.
  3. **Bertahan:** auto-battler di grid gerbang 5x4. Pemain menempatkan satpam (unit tank/jarak dekat/penjinak) sebelumnya. Hasil dihitung otomatis, juga saat offline.
- **Asosiasi dan sabotase:** asosiasi Kasta 3 (maksimal 10 anggota) bisa menyewa blokade rute ke pesaing. Batasan anti-perundungan:
  - Target harus Kasta 3 juga (Kasta 1 dan 2 tidak bisa jadi target).
  - Biaya blokade Rp 500 juta per hari game, durasi maksimal 1 hari game, jeda 3 hari game per target.
  - Target bisa membalas atau menyewa pengawalan.
  - Pemain baru Kasta 3 dilindungi 7 hari nyata.

### 6.4 Cuaca dan lahan
- Cuaca per kecamatan, diumumkan **prakiraan 1 hari game ke depan** untuk semua pemain (adil, tidak bisa dibeli).
- **Status lahan:** kering / becek / lumpur. Tanah berubah ke lumpur setelah 6 jam game hujan.
  | Status | Efek |
  |---|---|
  | Becek | Bongkar muat +30% lebih lama |
  | Lumpur | Trailer 20 ton punya 40% peluang terjebak per masuk; keluar dengan loader (1 jam game) atau derek (Rp 2,5 juta) |
  | Cor beton | Kebal; biaya Rp 350 ribu/m² |
- **Oksidasi:** tumpukan grade HMS 1 dan reuse di lahan terbuka turun 1 tingkat kualitas per 2 hari game hujan (harga -15% per tingkat). Hanggar atap menghentikan oksidasi.

### 6.5 Keausan dan downtime mesin
Setiap alat punya **Kondisi** 0 sampai 100%, turun 0,5% per jam game operasi (2% jika menahan beban di atas 80% kapasitas).
| Pilihan saat rusak | Biaya | Waktu | Risiko |
|---|---|---|---|
| Suku cadang asli | 8% harga alat | 2 hari game (8 jam nyata) | Tidak ada |
| Kanibalisasi | Gratis | 1 jam game | Alat donor turun ke Kondisi 0%, perlu suku cadang asli |
| Perbaikan darurat (lakban/kawat) | 1% harga alat | 15 menit game | 15% peluang breakdown **per muatan berat** selama 1 hari game; breakdown = 50% peluang kecelakaan K3 |

### 6.6 Keselamatan kerja (K3)
- Pekerja dikelompokkan dalam **regu** dipimpin Mandor (bukan diatur satu per satu).
- Regu punya **Stamina** dan **Moral**. Dua shift (siang/malam) diatur otomatis oleh Mandor; pemain hanya memilih kebijakan (hemat, normal, lembur).
- **Kecelakaan K3** (dari breakdown, lembur berlebihan, alat di bawah 20%): investigasi aparat, operasional area itu berhenti 1 hari game, moral regu -30, reputasi -10, denda sesuai kasta.

### 6.7 Barang gelap (contraband)
- Tawaran muncul acak: kabel listrik curian, rel bekas, aset sitaan bermasalah (nama fiktif).
- **Margin:** 3x sampai 5x muatan legal sejenis.
- **Meteran Sorotan (heat):** setiap muatan gelap +15 sorotan; turun 5 per hari game.
- **Peluang razia per muatan** = 10% + (sorotan / 2)%. Contoh: sorotan 40 berarti 30%.
- **Jika tertangkap:** muatan disita, denda 3x nilai muatan, reputasi -25, operasional disita 2 hari game untuk Kasta 3.
- **Relasi pelindung** (pengganti "bekingan aparat"): biaya bulanan tinggi, mengurangi peluang razia setengahnya, tetapi bisa "terbongkar" (5% per bulan game) dan menggandakan hukuman.
- **Target balancing:** nilai harapan barang gelap kira-kira 1,3x jalur legal di sorotan rendah, lalu negatif di atas sorotan 50. Jadi menggoda sesekali, merugikan kalau jadi strategi utama.

---

## 7. Ekonomi dan matematika

### 7.1 Indeks Baja Nasional (IBN)
- Ditentukan server, diperbarui setiap 1 jam game (10 menit nyata).
- Rumus: IBN = Basis x Siklus Pasar x Acak Harian x (1 + Efek Pasokan Kota)
  - Basis awal: Rp 10.000/kg ingot.
  - Siklus pasar: gelombang naik-turun ±15% dengan periode sekitar 1 tahun game (60 hari nyata); pola umumnya diketahui, angka pastinya tidak.
  - Acak harian: ±3%.
  - Efek pasokan kota: -5% sampai +5% tergantung total ingot yang dijual pemain di kota (pemain besar bisa menekan harga).
- **Acuan Rongsok HMS 1** = 55% x IBN (Rp 5.500 saat IBN Rp 10.000).
- Semua pemain melihat data yang sama. Tidak ada yang bisa membeli data masa depan.

### 7.2 Harga lokal
- Harga beli/jual pemain bebas dalam batas **50% sampai 130% dari acuan** untuk mencegah pemindahan uang antar akun (Bagian 8.3).
- Pembeli AI (jaring pengaman):
  | Pembeli AI | Membeli dari | Harga |
  |---|---|---|
  | Pabrik AI Monopoli | Kasta 1 | 70% acuan |
  | Pabrik AI Monopoli | Kasta 2 (min 5 ton) | 90% acuan |
  | Toko Konstruksi NPC | Semua kasta, barang reuse | 160 sampai 250% acuan |
  | Pasar baja NPC | Kasta 3, ingot | 100% IBN |
- Penjual AI (pengepul AI) menyetor ke bandar pemain jika harga beli bandar minimal 85% acuan, dengan volume terbatas per kecamatan. Ini mengisi kekurangan pemain Kasta 1 tanpa menggantikan mereka.

### 7.3 Tabel margin per kasta (IBN Rp 10.000, grade HMS 1)
| Kasta | Beli | Biaya tambahan | Jual | Laba bersih per kg | Laba per trip penuh |
|---|---|---|---|---|---|
| 1 (pikap 1 ton) | Rp 3.300 dari NPC (60%) | BBM Rp 150 | Rp 4.700 ke bandar (85%) | Rp 1.250 | Rp 1,25 juta |
| 2 (trailer 20 ton) | Rp 4.700 | Susut kotoran 3%, angkut Rp 150 | Rp 5.500 ke smelter | Rp 485 | Rp 9,7 juta |
| 3 (per kg ingot) | Rp 5.500 rongsok, susut lebur 10% (jadi Rp 6.111) | OpEx lebur Rp 2.000 | Rp 10.000 | Rp 1.889 | Rp 56,7 juta per 30 ton/hari game |

Catatan:
- Contoh v1 (laba Rp 2.500/kg) dikoreksi karena belum memasukkan susut lebur.
- Pajak dipotong per kuartal dari laba, tidak dari tabel di atas.
- Sortir menaikkan margin: rongsok campur yang disortir dengan benar bisa naik 15 sampai 30%.

### 7.4 Sumber dan pembuangan uang (faucet dan sink)
| Sumber uang masuk (faucet) | Pembuangan uang keluar (sink) |
|---|---|
| Penjualan ke Pabrik AI dan pasar baja NPC | Pembelian rongsok dari NPC |
| Toko Konstruksi NPC (reuse) | Pajak kuartal |
| Kontrak B2G | Gaji, BBM, listrik EAF |
| Hadiah tutorial dan misi | Suku cadang, derek, perbaikan |
| | Konstruksi (cor, hanggar, bangunan) |
| | Uang keamanan, denda, sitaan |
| | Biaya izin |

Transaksi antar pemain hanya memindahkan uang, tidak menciptakan uang. Target tim ekonomi: total uang beredar per kota naik maksimal 5% per minggu nyata setelah dikurangi sink. Dipantau di dasbor live-ops.

### 7.5 Skala hukuman per kasta
Biaya kejadian dihitung sebagai persentase omzet harian, bukan angka tetap, supaya terasa di semua kasta:
| Kejadian | Kasta 1 | Kasta 2 | Kasta 3 |
|---|---|---|---|
| Derek/terjebak | Rp 150 ribu | Rp 2,5 juta | Rp 10 juta |
| Denda K3 | Rp 1 juta | Rp 25 juta | Rp 250 juta |

### 7.6 Reputasi
Skala 0 sampai 100, per pemain.
- Naik: SPK selesai tepat waktu (+2), timbangan jujur, kontrak B2G (+5), CSR.
- Turun: SPK gagal (-5), setoran berkotoran tinggi (-3), kecelakaan K3 (-10), tertangkap barang gelap (-25).
- Membuka: SPK besar, syarat naik kasta, kontrak B2G (minimal 60).

### 7.7 Antrean gerbang
- Gerbang manual: 6 truk per jam game. Gerbang RFID: 20 truk per jam game.
- Truk yang antre di jalan umum menurunkan Kesabaran Warga (6.3).

---

## 8. Multiplayer, sosial, dan anti-eksploit

### 8.1 Interaksi antar pemain (semua asinkron)
- **Papan Harga Lokal:** bandar dan lapak satelit memasang harga beli per grade per kecamatan.
- **SPK:** smelter memasang permintaan (tonase, grade, harga, tenggat). Bandar mengambil dan mengirim.
- **Kontrak langsung:** dua pemain menyepakati harga dan volume untuk beberapa hari game.
- **Asosiasi:** maksimal 10 pemain, berbagi chat, kontrak internal, blokade (6.3).
- **Chat:** kecamatan, asosiasi, dan pesan pribadi, dengan filter kata dan laporan.

### 8.2 Siapa yang memasok di awal dan akhir season
| Kondisi | Solusi |
|---|---|
| Awal season: semua pemain di Kasta 1, belum ada bandar/smelter pemain | Pembeli AI di semua kasta; bandar AI mengisi tiap kecamatan sampai ada bandar pemain |
| Bandar menunggu terlalu lama | Pengepul AI menyetor (7.2) |
| Pertengahan sampai akhir season: banyak pemain sudah di kasta atas, Kasta 1 menipis | Pengepul AI menutup kekurangan pasokan; pemain yang bergabung terlambat dapat bonus kejar ketinggalan; peringkat Kasta 1 sendiri agar tetap menarik; smelter boleh punya lapak satelit |
| Season berikutnya | Semua pemain mulai lagi di Kasta 1 di kota baru, jadi lapisan bawah terisi penuh kembali (11.3) |
| Smelter terlalu dominan | Efek pasokan kota menekan IBN; pajak progresif |

Target rasio sehat: sekitar 20 pengepul (pemain + AI) per bandar, sekitar 8 bandar per smelter.

### 8.3 Anti-eksploit
- Batas harga 50 sampai 130% acuan untuk transaksi antar pemain.
- Pajak transaksi antar pemain 2% (sink + mengurangi pencucian uang game).
- Deteksi pola: transaksi berulang ke akun yang sama dengan harga ekstrem, akun baru yang hanya mengirim nilai ke satu akun.
- Satu akun per perangkat/nomor HP per kota.
- Server otoritatif: semua hitungan ekonomi di server, klien hanya tampilan.
- Penipuan setoran (kotoran): bandar dengan alat periksa bisa menolak; penyetor dapat reputasi negatif.

---

## 9. Progresi
- **Level usaha** (1 sampai 50) dari total laba seumur hidup, membuka bangunan dan alat.
- **Kasta** dibuka lewat syarat di Bagian 5 (kas, reputasi, izin, lahan).
- **Pemain boleh tetap di kasta rendah.** Kasta 1 dan 2 punya peringkat season dan hadiah kosmetik sendiri agar tidak terasa sebagai tahap yang harus dilewati saja.

### 9.1 Kurva progresi target (pemain aktif 3 sampai 5 sesi per hari)
| Tonggak | Pemain baru | Veteran (dengan bekal warisan) |
|---|---|---|
| Beli triseda | Hari 1 | Langsung (bekal) atau hari 1 |
| Beli pikap | Hari 3 sampai 5 | Hari 2 |
| Naik Kasta 2 | Hari 10 sampai 14 | Hari 5 sampai 7 |
| Lahan dicor + hanggar | Hari 20 sampai 25 | Hari 12 sampai 15 |
| Naik Kasta 3 | Hari 45 sampai 55 | Hari 30 sampai 40 |
| Kontrak B2G pertama | Hari 60 sampai 70 | Hari 45 sampai 55 |

Dengan season 90 hari, pemain aktif menikmati Kasta 3 sekitar 35 sampai 45 hari. Pemain santai (1 sampai 2 sesi per hari) ditargetkan mencapai Kasta 2 dan menikmati peran bandar. Angka ini divalidasi di Fase 0 (Lampiran I).

### 9.2 Level usaha (contoh pembukaan)
| Level | Membuka |
|---|---|
| 1 | Gerobak, pungut gratis |
| 3 | Triseda, magnet tangan |
| 6 | Pikap, papan harga lokal (lihat) |
| 10 | Syarat level Kasta 2, timbangan lantai |
| 15 | Hanggar, cor beton, truk engkel |
| 20 | Trailer, alat XRF, SPK besar |
| 28 | Syarat level Kasta 3, AMDAL |
| 35 | Shredder, gerbang RFID |
| 40 | Asosiasi (pendiri), tender B2G |
| 50 | Gelar kota, slot dekorasi Kantor Pusat tambahan |

---

## 10. Onboarding: 10 menit pertama
1. **Menit 0 sampai 2:** cerita singkat (2 layar). Pemain dapat gerobak dan Rp 500 ribu.
2. **Menit 2 sampai 4:** pungut rongsok di gang, sortir pertama (tutorial seret-lepas).
3. **Menit 4 sampai 6:** jual ke Pabrik AI, lalu ditunjukkan bandar yang beli lebih mahal (pelajaran harga).
4. **Menit 6 sampai 8:** beli triseda dari hadiah misi, antre rute pertama.
5. **Menit 8 sampai 10:** hujan pertama (prakiraan ditunjukkan), simpan barang di bawah atap. Ajakan kembali: "Rute selesai dalam 45 menit."

Sistem dibuka bertahap: cuaca (hari 1), kerusakan alat (hari 2), izin (saat mendekati Kasta 2), preman (Kasta 2), K3 dan barang gelap (Kasta 2 akhir), asosiasi dan auto-battler (Kasta 3).

---

## 11. Gagal dan pemulihan

### 11.1 Utang
- Kas negatif menjadi utang ke "koperasi" NPC, bunga 2% per hari game.
- Utang di atas 50% nilai aset: alat dan bangunan disita bertahap, mulai dari yang paling tidak dipakai.

### 11.2 Bangkrut
- Semua aset habis dan utang tetap ada: pemain **turun satu kasta** dengan modal awal kasta itu, level usaha dan kosmetik tetap.
- Kasta 1 tidak bisa bangkrut lebih jauh: selalu punya gerobak.

### 11.3 Season: Ekspansi ke Kota Baru

**Masalah yang diselesaikan:** dalam rantai pasok berkasta, kalau semua pemain sudah naik ke atas, tidak ada lagi yang memasok dari bawah. Season dibutuhkan agar lapisan bawah terisi lagi. Tantangannya: pemain tidak boleh merasa kerja kerasnya dihapus.

**Prinsip:** tidak ada yang dihapus. Yang terjadi adalah **ekspansi**: Grup Usaha pemain membuka cabang di kota baru, sementara usaha di kota lama tetap tercatat sebagai milik grup.

**Siklus season (90 hari nyata):**
| Waktu | Yang terjadi |
|---|---|
| Hari 1 | Kota baru dibuka. Semua pemain memulai cabang baru di Kasta 1, membawa bekal warisan. Rantai pasok bawah penuh |
| Hari 1 sampai 30 | Fase rebutan: pemain tercepat membuka lapak, harga beli bersaing ketat |
| Hari 30 sampai 75 | Bandar dan smelter pemain mulai berdiri; pengepul AI mengisi kekurangan Kasta 1 |
| Hari 76 sampai 90 | **Pekan Tutup Buku**: hitung mundur jelas, kontrak baru dibatasi agar tidak ada yang rugi di tengah jalan, peringkat final |
| Akhir | Usaha di kota ini dibekukan menjadi bagian Grup Usaha; kota tetap bisa dikunjungi (hanya lihat) |

**Yang dibawa pemain ke season berikutnya (permanen, tidak pernah hilang):**
| Dibawa | Isi |
|---|---|
| Grup Usaha | Nama dan logo grup, level grup, daftar semua kota yang pernah dikuasai beserta nilai kerajaan dan peringkatnya |
| Kantor Pusat | Ruang 3D pribadi yang tidak pernah direset, berisi trofi, miniatur smelter dari kota lama, dan dekorasi |
| Kosmetik dan gelar | Semua livery, seragam, bingkai, gelar ("Raja Besi Kota Sukabaja Season 1") |
| Bekal warisan | Pemain memilih 3 bekal dari daftar yang dibuka dengan level grup, misalnya: mulai dengan triseda, PT sudah terdaftar, reputasi awal 15, antrean offline +2 jam, sortir 10% lebih cepat |
| Poin Grup | Sebagian nilai kerajaan akhir dikonversi menjadi Poin Grup untuk membuka bekal dan kosmetik baru, bukan uang game |

**Batas bekal warisan:** bekal hanya mempercepat awal permainan (target: veteran mencapai Kasta 2 sekitar 2x lebih cepat dari pemain baru), tidak memberi uang atau alat Kasta 3. Jadi veteran tetap unggul, tetapi pemain baru tidak tertinggal tanpa harapan.

**Kenapa pemain tidak merasa dirampas:**
1. Panjang season dan tanggal tutup buku diketahui sejak hari pertama; tidak ada kejutan.
2. Tidak ada yang dihapus: usaha lama menjadi trofi dan sejarah grup yang bisa dipamerkan.
3. Semua mulai bersama, sehingga awal season adalah momen paling seru dan adil (fase rebutan).
4. Pemain veteran mulai dengan bekal, jadi terasa sebagai babak baru, bukan kembali ke nol.
5. Pemain yang bergabung di tengah season mendapat bonus kejar ketinggalan, atau bisa menunggu kota baru.

**Kenapa ini menjawab "siapa yang supply":**
- Awal season: semua pemain di Kasta 1, pasokan melimpah dan antar pemain.
- Tengah sampai akhir season: pengepul AI menjadi lapisan pasokan dasar, dan pemain Kasta 1 tetap lebih menguntungkan bagi bandar karena barangnya sudah disortir (7.2).
- Season berikutnya: lapisan bawah terisi kembali secara alami.

**Opsi lain yang dipertimbangkan:**
| Opsi | Kenapa tidak dipilih |
|---|---|
| Tanpa season, dunia permanen | Pasokan Kasta 1 terus menipis; pemain awal mendominasi selamanya; pemain baru sulit bersaing |
| Season dengan reset biasa (aset hilang) | Terasa dirampas; pemain marah |
| Season hanya peringkat, tanpa mulai ulang | Tidak menyelesaikan masalah pasokan |
| Kota permanen untuk yang tidak mau mulai ulang | Memecah pemain jadi dua kelompok; bisa dipertimbangkan di Fase 4 jika pemain memintanya |

---

## 12. UI/UX (mobile-first)

### 12.1 HUD
- **Kiri atas:** nama usaha, kas, reputasi, kasta.
- **Kanan atas:** ticker IBN dan acuan rongsok (hijau/merah), cuaca dan prakiraan, jam dan hari game.

### 12.2 Navigasi bawah berubah per kasta
| Menu | Kasta 1 | Kasta 2 | Kasta 3 |
|---|---|---|---|
| Pasar (harga lokal, papan beli, SPK) | Ya | Ya | Ya |
| Gudang dan sortir | Ya | Ya | Ya |
| Armada (kondisi, rute) | Ya | Ya | Ya |
| Konstruksi | | Ya | Ya |
| Pekerja dan Mandor | | Ya | Ya |
| Izin dan pajak | Saat mendekati Kasta 2 | Ya | Ya |
| Keamanan | | Ya | Ya |
| Asosiasi | | | Ya |

Chat dan Toko diakses dari ikon di pojok, tidak memakan slot navigasi.

### 12.3 Prinsip
- Setiap aksi utama selesai dalam maksimal 3 ketukan dari layar utama.
- Angka penting (harga, margin) selalu tampil sebelum pemain mengonfirmasi transaksi.
- Mode "satu tangan" untuk menu (tombol di setengah bawah layar).

---

## 13. Aset 3D (cakupan MVP vs nanti)
| Kategori | MVP (Fase 1 sampai 2) | Nanti |
|---|---|---|
| Kendaraan | Gerobak, triseda, pikap, truk engkel, trailer | Variasi livery, alat berat tambahan |
| Alat | Timbangan lantai, loader kecil, magnet crane | Jembatan timbang, RFID, EAF, shredder |
| Lingkungan | 1 kota, 6 kecamatan, lapak tanah/cor, hanggar | Kota tambahan, pabrik tertutup |
| Material | Tumpukan HMS, seng, drum, baut | Variasi karat dan grade visual |
| Karakter | Kuli, supir, Mandor, warga | Satpam, preman, aparat (Fase 3) |

Gaya visual: low-poly bertekstur dengan pencahayaan hangat dan debu, mudah dibaca dari kamera atas.

---

## 14. Monetisasi (revisi, benar-benar non-pay-to-win)
| Produk | Isi | Status |
|---|---|---|
| Kosmetik | Livery truk, seragam pekerja/satpam, warna mesin, dekorasi kantor | Dipertahankan |
| Season Pass (gratis + premium) | Jalur misi season berhadiah kosmetik, dekorasi Kantor Pusat, bingkai profil, gelar; tidak memberi uang game atau bekal warisan | Baru |
| Analis Pasar (langganan) | Grafik historis IBN, alarm harga, dasbor multi-lapak, laporan margin. **Tanpa data masa depan** | Menggantikan VIP bocoran harga 24 jam |
| Slot antrean tambahan | Antrean pesanan offline 8 jam menjadi 12 jam | Kenyamanan; juga bisa dibuka gratis lewat Mandor senior |
| ~~Skip timer izin~~ | Dihapus. Izin adalah gerbang progresi kompetitif | Dihapus |
| ~~Prediksi harga 24 jam~~ | Dihapus. Melanggar keadilan pasar | Dihapus |

Iklan berhadiah (opsional) hanya memberi kosmetik atau token tutorial di Kasta 1, tidak memberi uang game.

---

## 15. Konten sensitif dan penamaan
| v1 | v2 (fiktif) |
|---|---|
| Ormas | Kelompok pemuda setempat dengan nama fiktif (misalnya "Laskar Gang Baja") |
| Polisi / Aparat | "Petugas Penertiban Kota" (fiktif) |
| Portal AHU/OSS | "Portal SIPADU" (Sistem Perizinan Terpadu, fiktif) |
| Kabel PLN curian | Kabel listrik curian (tanpa nama perusahaan) |
| "Bekingan aparat" | "Relasi pelindung" |

- Kota dan kecamatan fiktif.
- Target rating usia 17+ karena tema pemerasan dan barang gelap. Aturan rating dan regulasi konten yang berlaku belum diverifikasi; perlu dicek dengan ahli hukum sebelum rilis.
- Barang gelap selalu digambarkan berisiko dan merugikan jika dijadikan kebiasaan.

---

## 16. Teknis
| Aspek | Keputusan awal |
|---|---|
| Engine | Unity (URP) atau Godot; Unity disarankan karena ekosistem mobile dan aset |
| Backend | Server otoritatif dengan database relasional; pertimbangkan Nakama atau backend kustom |
| Model jaringan | Asinkron: klien kirim perintah, server simulasi tiap 1 jam game (10 menit nyata) + kejadian |
| Perangkat minimum | Android 10, RAM 4 GB, chipset kelas menengah 2021; target 30 fps |
| Ukuran unduhan awal | Di bawah 300 MB, aset tambahan diunduh bertahap |
| Bahasa | Indonesia dulu, Inggris di Fase 4 |
| Analitik | Event per transaksi, per sesi, per kejadian untuk balancing |

---

## 17. Fase rilis dan MVP

| Fase | Isi | Tujuan | Kriteria lolos |
|---|---|---|---|
| 0. Simulasi ekonomi | Spreadsheet/skrip simulasi pemain per kasta, harga, faucet/sink | Membuktikan piramida bertahan di awal dan akhir season | Simulasi 90 hari: tiap kasta untung, inflasi di bawah 5%/minggu |
| 1. Prototipe single-player | Kasta 1 ke Kasta 2 dengan AI: kumpul, sortir, jual, armada, 1 kejadian hujan-karat, dilema perbaikan | Menguji apakah loop inti seru | Playtest 20 orang: 60% ingin main lagi, sesi rata-rata di atas 5 menit |
| 2. Multiplayer asinkron (soft launch) | Papan harga lokal, setoran antar pemain, SPK, izin, preman sederhana (bayar/tolak) | Ekonomi antar pemain hidup | Retensi D1 35%, D7 12%, D30 5% |
| 3. Kasta 3 | Smelter, B2G, asosiasi, auto-battler, barang gelap | End-game | 10% pemain aktif mencapai Kasta 3 dalam 60 hari nyata |
| 4. Ekspansi | Season kedua dan seterusnya (Ekspansi Kota), event kota, iOS penuh, PC, grafis lebih tinggi | Skala | Pendapatan per pengguna aktif sesuai target bisnis |

### 17.1 Kriteria penerimaan MVP (Fase 1)
- Pemain bisa memungut atau membeli rongsok, dan setiap tumpukan punya grade tersembunyi yang bisa diperiksa.
- Sortir mengubah harga jual sesuai tabel 6.1.
- Pemain bisa membeli triseda dan pikap; kapasitas dan waktu trip sesuai tabel 5.1.
- Harga AI mengikuti IBN dan berubah tiap 10 menit nyata.
- Hujan mengubah status lahan dan menurunkan grade stok terbuka sesuai 6.4.
- Alat rusak menampilkan tiga pilihan perbaikan dengan efek sesuai 6.5.
- Progres offline dihitung maksimal 8 jam nyata.
- Pemain bisa memenuhi syarat dan membuka Kasta 2, memasang harga beli, dan menerima setoran dari pengepul AI.

---

## 18. Metrik sukses
| Metrik | Target soft launch |
|---|---|
| Retensi D1 / D7 / D30 | 35% / 12% / 5% |
| Pemain aktif yang lanjut ke season berikutnya | 60% atau lebih |
| Sesi per hari | 3 atau lebih |
| Lama sesi rata-rata | 5 sampai 8 menit |
| Pemain yang bertransaksi dengan pemain lain di minggu pertama | 50% atau lebih |
| Konversi pembayar | 2 sampai 4% |
| Inflasi uang game per kota | Di bawah 5% per minggu |

---

## 19. Pertanyaan yang masih terbuka
1. Apakah asumsi A1 sampai A4 sudah sesuai?
2. Berapa anggaran dan jumlah orang di tim? Ini menentukan apakah Fase 3 masuk rilis pertama atau tidak.
3. Ditetapkan: season "Ekspansi ke Kota Baru" 90 hari (11.3). Panjangnya masih bisa disesuaikan setelah playtest.
4. Seberapa gelap tema yang Anda inginkan (barang gelap, pemerasan): tetap ada dengan nama fiktif, atau dilunakkan?

---

# LAMPIRAN DESAIN

Semua angka di lampiran adalah nilai awal untuk balancing dan akan disetel di Fase 0 (simulasi) dan Fase 1 (playtest). Harga memakai IBN Rp 10.000/kg.

## Lampiran A. Katalog armada, alat, dan bangunan

### A.1 Armada
| Kendaraan | Kasta | Kapasitas | Harga beli | Sewa | Biaya jalan | Kondisi turun |
|---|---|---|---|---|---|---|
| Gerobak dorong | 1 | 50 kg | Gratis | - | Rp 0 | Tidak aus |
| Motor triseda | 1 | 300 kg | Rp 12 juta | - | Rp 40 ribu/trip | 1% per trip |
| Pikap bekas | 1 | 1 ton | Rp 85 juta | Rp 400 ribu/trip | Rp 150 ribu/trip | 1% per trip |
| Truk engkel | 2 | 8 ton | Rp 320 juta | Rp 1,8 juta/trip | Rp 900 ribu/trip | 1,5% per trip |
| Trailer flat-deck 20 ft | 2 | 20 ton | Rp 900 juta | Rp 3 juta/trip | Rp 3 juta/trip | 2% per trip |
| Armada trailer (3 unit) | 3 | 60 ton | Rp 2,5 miliar | - | Rp 9 juta/trip | 2% per trip |

### A.2 Alat
| Alat | Kasta | Harga | Fungsi |
|---|---|---|---|
| Magnet tangan | 1 | Rp 500 ribu | Cek besi vs non-besi, grade kasar |
| Timbangan lantai 3 ton | 2 | Rp 25 juta | Wajib untuk menerima setoran |
| Loader kecil bekas | 2 | Rp 150 juta | Bongkar muat 10 ton/jam game, menarik truk dari lumpur |
| Alat XRF | 2 | Rp 350 juta | Grade akurat dan instan, mendeteksi kotoran di setoran |
| Magnet crane | 2 dan 3 | Rp 250 juta | Bongkar muat 25 ton/jam game |
| Forklift | 2 dan 3 | Rp 180 juta | Memindahkan barang reuse tanpa merusak grade |
| Jembatan timbang truk | 3 | Rp 300 juta | Syarat Kasta 3, menimbang 60 ton |
| Gerbang RFID | 3 | Rp 400 juta | Antrean 6 menjadi 20 truk/jam game |
| Hydraulic shredder | 3 | Rp 2 miliar | Susut lebur 10% turun ke 7%, debu (-1 Kesabaran Warga/jam game) |
| EAF mini 30 ton/hari game | 3 | Rp 1,2 miliar | Peleburan dasar |
| EAF 60 ton/hari game | 3 | Rp 3 miliar | Upgrade kapasitas |
| Lini rolling H-beam | 3 | Rp 4 miliar | Produk H-beam (+20% di atas harga ingot) |
| Lini baut industri | 3 | Rp 2,5 miliar | Produk baut (+35% di atas harga ingot, volume kecil) |

### A.3 Bangunan dan lahan
| Bangunan | Kasta | Harga | Efek |
|---|---|---|---|
| Sewa lahan lapak 1,6 ha | 2 | Rp 20 juta/bulan game | Kapasitas stok 60 ton |
| Perluasan lahan +1 ha | 2 | Rp 15 juta/bulan game | +40 ton |
| Cor beton per petak 20x20 m | 2 dan 3 | Rp 140 juta | Petak kebal lumpur; jalur truk cukup 3 sampai 5 petak |
| Hanggar atap 50 ton | 2 dan 3 | Rp 120 juta | Menghentikan oksidasi |
| Gudang reuse tertutup 20 ton | 2 dan 3 | Rp 200 juta | Barang reuse naik 1 tingkat kerapian (+10% harga) |
| Pos jaga | 2 dan 3 | Rp 60 juta | Membuka 2 slot satpam auto-battler per level (maks level 3) |
| Mess pekerja | 2 dan 3 | Rp 150 juta | Moral +10, stamina pulih 25% lebih cepat |
| Lahan industri | 3 | Rp 700 juta | Syarat smelter |
| Gedung pabrik tertutup | 3 | Rp 500 juta | Operasi malam tanpa penalti bising sebagian (-50%) |
| Kantor Pusat (permanen) | Semua | Gratis | Ruang trofi Grup Usaha (11.3) |

### A.4 Paket naik kasta
| Paket | Isi | Total |
|---|---|---|
| Naik Kasta 2 | PT Rp 15 juta, sewa lahan bulan pertama Rp 20 juta, timbangan Rp 25 juta, loader bekas Rp 150 juta, modal kerja | Rp 250 juta |
| Naik Kasta 3 | Lahan industri Rp 700 juta, gedung Rp 500 juta, EAF mini Rp 1,2 miliar, jembatan timbang Rp 300 juta, magnet crane Rp 250 juta (AMDAL dibayar sebelumnya) | Rp 2,95 miliar |

## Lampiran B. Pekerja dan Mandor

| Peran | Gaji per hari game | Fungsi |
|---|---|---|
| Kuli | Rp 150 ribu | Sortir dan bongkar manual, 2 ton/jam game per regu 5 orang |
| Supir | Rp 200 ribu | Wajib per kendaraan di atas triseda |
| Operator alat | Rp 300 ribu | Wajib per loader, crane, forklift |
| Mandor | Rp 500 ribu | Memimpin 1 regu (maks 8 orang), mengatur shift otomatis |
| Mandor senior | Rp 900 ribu | Antrean offline 12 jam, 2 regu |
| Satpam | Rp 250 ribu | Unit auto-battler dan penjaga malam |
| Operator EAF | Rp 600 ribu | Wajib 3 orang per tungku per shift |

- **Stamina regu:** turun 10 per jam kerja game, pulih saat istirahat. Di bawah 30: kerja 30% lebih lambat, risiko K3 dobel.
- **Moral regu:** 0 sampai 100. Naik dengan gaji di atas standar (+10%), mess, kontrak sukses. Turun karena kecelakaan, lembur, gaji telat.
- **Kebijakan Mandor:** Hemat (1 shift, gaji standar), Normal (2 shift), Lembur (2 shift panjang, +40% output, moral -5 per hari game).
- **Sifat pekerja** (acak saat rekrut, maksimal 1): Teliti (sortir +10%), Cepat (+10% kecepatan), Pemalas (-10%), Penggerutu (moral regu -5). Pekerja bisa dilatih menjadi Mandor.

## Lampiran C. SPK, kontrak, dan tender

### C.1 Isi SPK
| Kolom | Contoh |
|---|---|
| Penerbit | Smelter pemain atau Pabrik AI |
| Material dan grade minimal | HMS 1, kotoran maksimal 3% |
| Tonase | 20 sampai 200 ton |
| Harga per kg | Rp 5.500 (bisa mengikuti IBN atau tetap) |
| Tenggat | 1 sampai 5 hari game |
| Jaminan | 5% nilai kontrak dari pengambil SPK |
| Syarat | Reputasi minimal, armada kondisi di atas 50% |

- Pengiriman diperiksa di gerbang penerbit. Grade di bawah syarat: harga dipotong sesuai tabel 6.1 atau ditolak (pilihan penerbit diatur sebelumnya).
- Telat: denda 2% per jam game, maksimal jaminan. Gagal: jaminan hangus, reputasi -5.
- Penerbit yang tidak membayar tidak mungkin terjadi: dana SPK dikunci server saat SPK diterbitkan (escrow).

### C.2 Kontrak langsung antar pemain
Volume dan harga per hari game untuk 3 sampai 10 hari game, dana dikunci per pengiriman, bisa dibatalkan dengan penalti 10% sisa kontrak.

### C.3 Tender B2G (pemerintah NPC)
- Diumumkan tiap 5 hari game, contoh: "Jembatan Kali Cempaka, 300 ton H-beam, tenggat 10 hari game".
- Penawaran tertutup: harga per kg dan tanggal kirim. Pemenang = skor harga (60%) + reputasi (25%) + sertifikat K3 (15%).
- Asosiasi boleh menawar bersama dan membagi tonase.
- Kemenangan: reputasi +5, laba besar, nama di monumen kota (kosmetik).

## Lampiran D. Auto-battler gerbang

### D.1 Aturan
- Grid gerbang 5x4, pemain menaruh unit di 2 baris depan. Musuh datang dalam 3 gelombang dari jalan.
- Pertarungan otomatis 60 detik, bisa ditonton atau dilewati. Hasil sama saat pemain offline.
- Menang: pemerasan berhenti 2 bulan game, reputasi warga +5. Kalah: bayar uang keamanan 1 bulan + kerusakan gerbang (Rp 20 sampai 100 juta tergantung kasta).
- Tidak ada cedera permanen atau kematian; unit yang kalah "pingsan" dan pulih 1 hari game.

### D.2 Unit pemain
| Unit | Peran | Darah | Serangan | Kemampuan |
|---|---|---|---|---|
| Satpam Jaga | Tank | 120 | 8 | Menahan 2 musuh sekaligus |
| Satpam Pentung | Petarung | 80 | 18 | Serangan beruntun |
| Anjing Penjaga | Cepat | 50 | 14 | Menyerang barisan belakang musuh |
| Operator CCTV | Pendukung | 40 | 0 | Musuh terlihat 1 gelombang lebih awal, unit lain +10% akurasi |
| Negosiator | Pendukung | 40 | 0 | Moral musuh -20%, peluang musuh mundur |
| Tim Pengawal (sewa) | Elite | 150 | 22 | Disewa per kejadian, Rp 50 juta |

### D.3 Musuh
| Musuh | Darah | Serangan | Catatan |
|---|---|---|---|
| Pemuda Pengacau | 60 | 10 | Jumlah banyak |
| Jawara | 140 | 20 | Muncul di gelombang 2 |
| Provokator | 40 | 5 | Menaikkan moral musuh lain |
| Pemimpin Laskar | 220 | 25 | Gelombang 3, hanya untuk Kasta 3 |

Kekuatan musuh diskalakan dengan kasta dan kekayaan target. Kelompok sewaan asosiasi saingan memakai tabel yang sama, plus 1 Pemimpin Laskar.

## Lampiran E. Katalog kejadian

| Kejadian | Kasta | Pemicu | Pilihan dan efek |
|---|---|---|---|
| Hujan badai | Semua | Cuaca | Pindah stok ke hanggar / biarkan (oksidasi) |
| Truk terjebak lumpur | 2, 3 | Lahan lumpur | Tarik loader (1 jam game) / derek (biaya per kasta) |
| Setoran berkotoran | 2 | Penyetor curang | Tolak (reputasi penyetor turun) / terima dengan potongan |
| Temuan barang reuse | Semua | Acak saat sortir | Jual ke Toko Konstruksi / simpan untuk SPK khusus |
| Pemerasan di gerbang | 2, 3 | Kekayaan, keamanan rendah | Bayar / negosiasi / bertahan (auto-battler) |
| Warga protes | 2, 3 | Kesabaran Warga di bawah 40 | CSR (biaya) / abaikan (blokade di bawah 20) |
| Mesin rusak | 2, 3 | Kondisi 0% | Suku cadang / kanibalisasi / perbaikan darurat |
| Kecelakaan K3 | 2, 3 | Breakdown, stamina rendah | Investigasi otomatis (6.6) |
| Tawaran barang gelap | 1 sampai 3 | Acak | Terima / tolak / laporkan (reputasi +3) |
| Razia | 1 sampai 3 | Meteran sorotan | Otomatis (6.7) |
| Lonjakan harga | Semua | IBN naik di atas 8% sehari | Kesempatan jual; notifikasi |
| Pemogokan pekerja | 2, 3 | Moral di bawah 20 | Naikkan gaji / tunggu (produksi berhenti) |
| Inspeksi lingkungan | 3 | Acak, lebih sering tanpa shredder tertutup | Lulus jika izin lengkap; denda jika tidak |
| Tender kilat | 3 | Acak | Kontrak kecil berbatas waktu 1 hari game |
| Pengepul AI mogok | 2 | Event kota | Pasokan AI turun 50% selama 1 hari game; setoran pemain lebih berharga |

Batas frekuensi: maksimal 2 kejadian negatif per hari nyata per pemain, dan tidak ada kejadian negatif besar di 3 hari pertama pemain baru.

## Lampiran F. Dunia, cerita, dan karakter

- **Setting:** kota-kota industri fiktif di Indonesia. Tiap season membuka kota baru dengan karakter berbeda, misalnya:
  | Kota | Ciri |
  |---|---|
  | Sukabaja | Kota pabrik peleburan besi, banyak bongkaran gedung dan pabrik tua, hujan sedang |
  | Karangbesi | Kota tambang tua, lahan luas, banyak lumpur |
  | Tanjung Rongsok | Kota padat, warga cepat protes, harga reuse tinggi |
  | Cempaka Raya | Kota proyek pemerintah, banyak tender B2G |
- **Mentor:** Pak Darto, pengepul tua yang memandu tutorial dan memberi misi harian.
- **Antagonis AI:** Bos Hartono, pemilik PT Baja Monopoli (Pabrik AI). Tujuan jangka panjang pemain: menjatuhkan monopolinya dengan smelter sendiri. Saat smelter pemain menguasai lebih dari 50% pasokan kota, muncul event cerita "Monopoli Runtuh" dengan hadiah kosmetik untuk seluruh kota.
- **Karakter pendukung:** Mbak Ratna (petugas portal SIPADU, mini-game izin), Bang Jago (pemimpin kelompok pemuda setempat, bisa jadi lawan atau mitra keamanan), Ibu RT (suara warga, meteran Kesabaran Warga).
- **Nada:** satir ringan dan hangat tentang dunia usaha kecil, bukan glorifikasi kejahatan.

## Lampiran G. Arah visual dan audio

- **Visual:** low-poly bertekstur, palet karat oranye, abu beton, biru langit pucat. Debu, asap tipis, dan percikan las sebagai efek murah tapi hidup. Tumpukan barang dibuat dari modul yang bisa digabung agar stok terlihat bertambah.
- **Keterbacaan:** grade terlihat lewat warna tumpukan setelah diperiksa (hijau HMS 1, kuning HMS 2, abu campur, biru reuse).
- **Audio:** dentang besi, mesin diesel, hujan di atap seng, azan dan pasar di kejauhan sebagai suasana kota. Musik santai bernuansa dangdut-elektronik pelan di menu.
- **Siang-malam:** lampu sorot lapak dan percikan EAF menjadi momen visual malam.

## Lampiran H. Daftar layar dan notifikasi

### H.1 Layar utama
| Layar | Isi utama |
|---|---|
| Kota (3D) | Lapak/pabrik pemain, armada bergerak, cuaca |
| Peta kota | Kecamatan, lapak pemain lain dan harganya, rute |
| Pasar | Ticker IBN, papan harga lokal, SPK, kontrak, tender |
| Gudang dan sortir | Tumpukan per grade, mini-game sortir, alat periksa |
| Armada | Kondisi, rute, antrean, perbaikan |
| Konstruksi | Grid bangunan, cor, hanggar |
| Pekerja | Regu, Mandor, kebijakan shift |
| Izin dan pajak | Status izin, mini-game SIPADU, tagihan |
| Keamanan | Grid auto-battler, riwayat kejadian, relasi |
| Asosiasi | Anggota, chat, kontrak bersama, blokade |
| Grup Usaha | Kantor Pusat 3D, sejarah kota, bekal warisan, Poin Grup |
| Toko | Kosmetik, season pass, langganan Analis Pasar |

### H.2 Notifikasi push (maksimal 4 per hari, bisa diatur)
Rute/antrean selesai, cuaca buruk dalam 1 jam nyata, gerbang diserang atau diblokir, setoran besar masuk, SPK hampir tenggat, Pekan Tutup Buku dimulai.

## Lampiran I. Spesifikasi simulasi ekonomi (Fase 0)

- **Alat:** spreadsheet untuk tabel harga, lalu skrip simulasi (misalnya Python) untuk 1.000 agen selama 90 hari nyata.
- **Tipe agen:** aktif (5 sesi/hari), sedang (3), santai (1), dengan proporsi awal 20% / 50% / 30%. Pemain baru masuk 2% per hari, pemain berhenti sesuai kurva retensi Bagian 18.
- **Perilaku agen:** memilih pembeli dengan harga bersih tertinggi (setelah jarak), naik kasta saat syarat terpenuhi, bandar menyesuaikan harga beli sesuai stok.
- **Keluaran yang diukur:** jumlah pemain per kasta per hari, waktu tunggu bandar mencapai 15 ton, porsi pasokan dari AI vs pemain, laba per kasta, total uang beredar, waktu ke tiap tonggak (9.1).
- **Kriteria lolos:** tiap kasta untung di median; porsi pasokan AI di bawah 50% pada hari 1 sampai 60; inflasi di bawah 5% per minggu; median pemain aktif mencapai Kasta 3 di hari 45 sampai 55.

## Lampiran J. Kalender live-ops per season (90 hari)

| Minggu | Kegiatan |
|---|---|
| 1 | Pembukaan kota baru, event "Rebutan Lapak" (peringkat bandar tercepat) |
| 3 | Event cuaca: Musim Hujan Ekstrem (3 hari) |
| 5 | Event harga: Lonjakan Tembaga (reuse dan non-besi naik) |
| 7 | Tender Akbar B2G pertama |
| 9 | Event cerita: Bos Hartono menurunkan harga Pabrik AI (perang harga) |
| 11 | Turnamen auto-battler antar asosiasi (kosmetik) |
| 12 sampai 13 | Pekan Tutup Buku, peringkat final, pengumuman kota berikutnya |

## Lampiran K. Tim dan perkiraan jadwal (tim kecil)

| Peran | Jumlah |
|---|---|
| Game designer / produser (merangkap ekonomi) | 1 |
| Programmer klien (Unity) | 1 sampai 2 |
| Programmer backend | 1 |
| Artis 3D | 1 sampai 2 |
| UI/UX + artis teknis | 1 |
| QA dan komunitas (paruh waktu) | 1 |

| Fase | Perkiraan durasi |
|---|---|
| 0. Simulasi ekonomi | 1 bulan |
| 1. Prototipe single-player | 3 sampai 4 bulan |
| 2. Multiplayer asinkron, soft launch | 5 sampai 6 bulan |
| 3. Kasta 3 dan end-game | 3 sampai 4 bulan |
| 4. Ekspansi | Berkelanjutan |

Perkiraan ini kasar dan bergantung pada pengalaman tim; belum termasuk penggalangan dana dan proses rilis di toko aplikasi.

## Lampiran L. Daftar risiko

| Risiko | Peluang | Dampak | Mitigasi |
|---|---|---|---|
| Ekonomi tidak seimbang (satu kasta selalu rugi) | Sedang | Tinggi | Fase 0 simulasi, parameter bisa diubah dari server tanpa update aplikasi |
| Pemain bingung karena terlalu banyak sistem | Tinggi | Tinggi | Pembukaan bertahap (Bagian 10), Mandor mengotomasi pengaturan mikro |
| Populasi tidak cukup untuk ekonomi antar pemain | Sedang | Tinggi | Jaring pengaman AI di semua kasta; kota berisi 1.000 pemain, gabung kota paralel bila sepi |
| Pemain kecewa saat season berganti | Sedang | Tinggi | Ekspansi Kota tanpa penghapusan, bekal warisan, jadwal jelas (11.3) |
| Keberatan atas konten sensitif | Sedang | Sedang | Nama fiktif, rating 17+, cek hukum (Bagian 15) |
| HP kelas menengah tidak kuat | Sedang | Tinggi | Gaya low-poly, target 30 fps, uji di perangkat minimum sejak Fase 1 |
| Jual-beli uang game dan multi-akun | Tinggi | Sedang | Batas harga, pajak transaksi, deteksi pola (8.3) |
| Cakupan membengkak | Tinggi | Tinggi | Fase dan kriteria lolos (Bagian 17); fitur baru wajib mendukung pilar desain |

## Lampiran M. Glosarium

| Istilah | Arti |
|---|---|
| Kasta | Tingkat usaha pemain: Pengepul, Bandar, Smelter |
| IBN | Indeks Baja Nasional, harga acuan ingot dari server |
| Acuan rongsok | 55% IBN, harga referensi HMS 1 |
| HMS 1 / HMS 2 | Grade besi tua tebal / tipis |
| Reuse | Barang yang dijual utuh tanpa dilebur |
| SPK | Surat Perintah Kerja, kontrak pasokan dari smelter |
| B2G | Kontrak dengan pemerintah NPC |
| EAF | Electric Arc Furnace, tungku lebur listrik |
| Lapak satelit | Lapak Kasta 2 milik smelter |
| Grup Usaha | Identitas permanen pemain lintas season |
| Bekal warisan | Keuntungan awal kecil yang dibawa ke season berikutnya |
| Pekan Tutup Buku | 2 minggu terakhir season |
| Faucet / sink | Sumber uang masuk / pembuangan uang keluar dari ekonomi |
| Sorotan | Meteran risiko razia dari barang gelap |
