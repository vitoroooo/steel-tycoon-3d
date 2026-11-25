# Pasar lokal HMS 1

Python 3.9+ dengan SQLite bawaan. Jalankan dari akar repositori. Gunakan lokasi basis data yang sama di setiap perintah.

```sh
mkdir -p ~/.steel-tycoon
python3 -m server.market --db ~/.steel-tycoon/pasar.sqlite3 trader Sari
python3 -m server.market --db ~/.steel-tycoon/pasar.sqlite3 trader Hasan
python3 -m server.market --db ~/.steel-tycoon/pasar.sqlite3 serve
```

Dua perintah pertama masing-masing menampilkan satu token acak khusus akun. Salin token langsung dari terminal pengelola ke dialog akun pasar. Basis data menyimpan hash token. Untuk mengganti token Sari tanpa mengubah saldonya, ulangi perintah pedagang dengan `trader Sari --rotate`.

Server mendengarkan di 127.0.0.1 port 8765. Untuk Android lewat USB, jalankan `adb reverse tcp:8765 tcp:8765` pada komputer server. Editor atau player desktop pada komputer server memakai alamat yang sama. Klien hanya menyimpan token selama sesi.

## Transaksi dari Buku usaha

Untuk uji HP tanpa USB, pengelola perlu menyediakan domain dengan sertifikat TLS tepercaya dan reverse proxy HTTPS
ke `127.0.0.1:8765`. Jangan membuka listener Python mentah ke internet. Batasi permintaan dan ukuran body di proxy;
jangan log token, header Authorization, atau isi pesanan. Belum ada server publik bawaan.
Di HP pilih **Alamat pasar**, ketik origin HTTPS dari pengelola, periksa nama server di dialog **Hubungkan akun**,
lalu masukkan token khusus alamat itu. Alamat dan token tidak disimpan ketika game ditutup.

Pakai nama samaran: papan lot menampilkan alias, harga dan jumlah, bukan kas atau stok pribadi.
Jumlah pelanggan berbeda dihitung dari transaksi selesai; pembelian berulang oleh akun yang sama tidak menambahnya.
Operator lokal dapat menjalankan `suspend NAMA` untuk menghentikan akses dan mengembalikan stok titipan satu kali,
atau `restore NAMA` untuk memulihkan akses. Perintah menggunakan opsi `--db` yang sama seperti `trader`.
Tindakan tercatat di `moderator_events`; tidak ada endpoint moderator lewat jaringan.

Setelah halaman **Pasar jaringan** terdaftar di Buku usaha, pilih **Hubungkan akun**. Dengan token Sari, pilih **Jual HMS 1**, tentukan harga, lalu **Pasang lot 5 kg**. Buka **Akun & sinkron → Lepas akun**. Hubungkan token Hasan, buka **Papan lot** dan bayar lot Sari. Masuk kembali sebagai Sari untuk memeriksa kas. Dengan dua perangkat, tekan **Muat ulang** untuk memperbarui papan setelah tindakan pedagang lain.

Setiap akun baru mendapat Rp200.000 dan 20 kg HMS 1 dari server. Rute pengumpulan pasar menambah 1 kg per jam game (600 detik), dengan kemajuan maksimal 48 jam game per jeda akses akun. Jam kota dan IBN tetap berjalan saat proses server berhenti. Harga lot harus dalam rentang 50–130% acuan HMS 1 saat dipasang dan dibeli. Penjual membayar pajak 2% dari nilai kotor, dibulatkan ke rupiah terdekat. Satu lot aktif per penjual. Membatalkan lot mengembalikan stok titipan.

## Kontrak langsung (PRD C.2)

Di halaman **Kontrak langsung** Buku usaha, penjual memilih pembeli, berat HMS 1 per hari game, harga (dalam batas acuan)
dan lama 3–10 hari game, lalu **Kirim tawaran**. Pembeli menerima dengan **Terima**: dana kiriman pertama dikunci dari kas
pasarnya. Tiap hari game server memindahkan stok ke pembeli, membayar dana terkunci ke penjual (pajak 2%), dan mengunci dana
kiriman berikutnya. Kiriman yang jatuh tempo diselesaikan saat siapa pun mengakses pasar, jadi tetap berjalan ketika kedua
pedagang offline. Membatalkan kontrak berjalan mengembalikan dana terkunci dan membayar penalti 10% dari nilai sisa kontrak
ke pihak lain. Penjual tanpa stok atau pembeli tanpa dana untuk kiriman berikutnya dianggap membatalkan dan membayar penalti
sebesar kas yang ia punya. Menarik tawaran yang belum diterima tidak dikenai penalti. Satu kontrak per pasangan penjual-pembeli.
`suspend NAMA` menghentikan kontrak pedagang itu tanpa penalti.

## Asosiasi dan blokade (PRD 6.3 dan 8.1)

Di halaman **Asosiasi** pedagang mendirikan atau bergabung dengan asosiasi (maksimal 10 anggota; asosiasi bubar saat anggota
terakhir keluar). Anggota berbagi chat: pesan maksimal 120 karakter, kata di `bannedWords` (`rules.json`) disensor, dan
pesan anggota lain dapat dilaporkan ke tabel `reports` untuk pengelola. Kasta akun pasar naik dengan `promote`, yang memotong
paket PRD A.4 dari kas pasar (Rp250 juta ke Kasta 2, Rp2,95 miliar ke Kasta 3). Anggota Kasta 3 dapat menyewa blokade rute
pesaing Kasta 3 di luar asosiasinya: Rp500 juta, 1 hari game, jeda 3 hari game per target, dan target yang baru 7 hari nyata
di Kasta 3 dilindungi. Selama diblokade, target tidak dapat memasang atau membeli lot dan kiriman kontraknya tertahan. Target
dapat membalas dengan blokade sendiri atau menyewa pengawalan Rp50 juta (`escort`) yang langsung membuka rutenya.

## Season dan Grup Usaha (PRD 11.3)

Satu season berjalan 90 hari nyata sejak basis data dibuat; hari 76–90 adalah Pekan Tutup Buku. Kontrak hanya dapat
ditawarkan atau diterima bila seluruh kirimannya selesai sebelum tutup buku. Saat season berakhir, lot dan kontrak
dihentikan tanpa penalti, nilai usaha tiap pedagang (kas + stok × acuan) dicatat di sejarah grupnya beserta peringkat,
Rp1 juta nilai menjadi 1 Poin Grup, lalu semua akun mulai lagi di Kasta 1 dengan modal awal di kota berikutnya (`cities`).
Asosiasi tetap. Veteran memilih sekali per season paling banyak 3 bekal warisan yang dibuka level grupnya
(`groupLevelPoints`, `legacies`). Server baru menerapkan bekal **Antrean offline +2 jam**; bekal lain (triseda, PT,
reputasi, sortir) tercatat tetapi belum mengubah karier di game. Pedagang yang didaftarkan di tengah season mendapat
stok kejar ketinggalan: 50% dari hasil rute pengumpulan sejak hari pertama season.

## Protokol lokal

Kedua endpoint memakai header `Authorization: Bearer <token>`. `GET /v1/market` mengembalikan akun terautentikasi, harga kota, batas harga, dan lot aktif. `POST /v1/orders` menerima JSON dengan `key` unik per niat transaksi dan salah satu bentuk berikut.

| action | Kolom tambahan |
|---|---|
| sell | kg, price |
| buy | lot |
| cancel | lot |
| offer | buyer (nama pedagang), kg, price, days |
| accept | contract |
| end | contract |
| found, join | name (nama asosiasi) |
| leave, promote, escort | – |
| say | text |
| report | message (id pesan) |
| blockade | name (nama pedagang target) |
| bekal | picks (id bekal dipisah koma, maksimal 3) |

Setelah respons terputus, kirim ulang isi dan key yang sama untuk menerima nota sebelumnya. Konflik bisnis memakai HTTP 409, kredensial keliru 401, dan kolom keliru 400. Pembatalan milik pedagang lain mendapat 403. SQLite mengunci transaksi penulisan sebelum membaca saldo, menyelesaikan lot dan mencatat nota dalam satu commit. Tabel `trades` menyimpan nilai bruto, pajak dan akun pembeli. Relasi dengan `lots` mengidentifikasi penjual.

## Pengujian

```sh
python3 -m unittest discover -s server/tests -v
```

Suite memakai basis data sementara dan proses HTTP terpisah. Cakupan mencakup pembelian serentak, permintaan ganda, kegagalan penyimpanan, pemulihan setelah proses dihentikan paksa, serta batas pengumpulan setelah pedagang lama tidak aktif.

Uji EditMode `SettleBetweenAuthenticatedUnityClients` menjalankan server sementara dan dua instans klien Unity. Jalankan Editor dengan checkout lengkap serta Python 3.9+ pada PATH agar uji ini dapat membuka proses server.
