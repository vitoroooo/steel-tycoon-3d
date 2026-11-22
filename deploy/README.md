# Memasang server game di VPS

Satu alamat HTTPS untuk game: `/v1/` dilayani server pasar (Python), `/career/` dilayani server karier (.NET).
Keduanya hanya mendengarkan di `127.0.0.1`; nginx yang menghadap internet.

| Di VPS | Isi |
|---|---|
| `/opt/sukabaja` | program dan aturan game (ditimpa tiap pemasangan) |
| `/var/lib/sukabaja/pasar.sqlite3` | akun dan karier pemain (tidak pernah disentuh pemasangan; inilah yang dicadangkan) |
| `sukabaja-market`, `sukabaja-career` | layanan systemd, berjalan sebagai pengguna `sukabaja` |
| `/etc/nginx/sites-available/sukabaja` | satu-satunya berkas nginx milik game |

## Perintah

Perlu `dotnet` 8 dan akses SSH dengan sudo ke VPS (Ubuntu, nginx dan certbot terpasang).

```sh
deploy/push.sh <ssh host>                          # pasang atau perbarui program, lalu nyalakan ulang
deploy/certificate.sh <ssh host> <domain> [email]  # sekali, setelah catatan DNS A menunjuk ke VPS
deploy/account.sh <ssh host> <nama> [--rotate]     # buat akun pemain; token tampil sekali
```

Memeriksa: `systemctl status sukabaja-career`, `journalctl -u sukabaja-career -n 50`.

Tes dari game sendiri: taruh `{"origin":"https://<domain>","token":"<token akun uji>"}` di
`unity/UserSettings/akun-online.json` (tidak ikut git), lalu jalankan tes EditMode `PublicServerTests`.

## Belum ada

- Pendaftaran akun dari dalam game: akun dibuat pengelola dengan `account.sh`.
- Cadangan otomatis `/var/lib/sukabaja`.
- Aturan game di server ikut versi yang dipasang; APK lama yang aturannya berbeda akan terus dikoreksi server.
