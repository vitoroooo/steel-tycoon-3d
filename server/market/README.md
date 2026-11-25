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

Setelah halaman **Pasar jaringan** terdaftar di Buku usaha, pilih **Hubungkan akun**. Dengan token Sari, pilih **Jual HMS 1**, tentukan harga, lalu **Pasang lot 5 kg**. Buka **Akun & sinkron → Lepas akun**. Hubungkan token Hasan, buka **Papan lot** dan bayar lot Sari. Masuk kembali sebagai Sari untuk memeriksa kas. Dengan dua perangkat, tekan **Muat ulang** untuk memperbarui papan setelah tindakan pedagang lain.

Setiap akun baru mendapat Rp200.000 dan 20 kg HMS 1 dari server. Rute pengumpulan pasar menambah 1 kg per jam game (600 detik), dengan kemajuan maksimal 48 jam game per jeda akses akun. Jam kota dan IBN tetap berjalan saat proses server berhenti. Harga lot harus dalam rentang 50–130% acuan HMS 1 saat dipasang dan dibeli. Penjual membayar pajak 2% dari nilai kotor, dibulatkan ke rupiah terdekat. Satu lot aktif per penjual. Membatalkan lot mengembalikan stok titipan.

## Protokol lokal

Kedua endpoint memakai header `Authorization: Bearer <token>`. `GET /v1/market` mengembalikan akun terautentikasi, harga kota, batas harga, dan lot aktif. `POST /v1/orders` menerima JSON dengan `key` unik per niat transaksi dan salah satu bentuk berikut.

| action | Kolom tambahan |
|---|---|
| sell | kg, price |
| buy | lot |
| cancel | lot |

Setelah respons terputus, kirim ulang isi dan key yang sama untuk menerima nota sebelumnya. Konflik bisnis memakai HTTP 409, kredensial keliru 401, dan kolom keliru 400. Pembatalan milik pedagang lain mendapat 403. SQLite mengunci transaksi penulisan sebelum membaca saldo, menyelesaikan lot dan mencatat nota dalam satu commit. Tabel `trades` menyimpan nilai bruto, pajak dan akun pembeli. Relasi dengan `lots` mengidentifikasi penjual.

## Pengujian

```sh
python3 -m unittest discover -s server/tests -v
```

Suite memakai basis data sementara dan proses HTTP terpisah. Cakupan mencakup pembelian serentak, permintaan ganda, kegagalan penyimpanan, pemulihan setelah proses dihentikan paksa, serta batas pengumpulan setelah pedagang lama tidak aktif.

Uji EditMode `SettleBetweenAuthenticatedUnityClients` menjalankan server sementara dan dua instans klien Unity. Jalankan Editor dengan checkout lengkap serta Python 3.9+ pada PATH agar uji ini dapat membuka proses server.
