#!/usr/bin/env bash
# Menerbitkan sertifikat dan menyalakan HTTPS:  deploy/certificate.sh <ssh host> <domain> [email]
# Email hanya perlu kalau VPS belum punya akun Let's Encrypt. Jalankan setelah catatan DNS A domain itu menunjuk ke VPS. Hanya menambah satu berkas situs nginx.
set -euo pipefail
host=${1:?ssh host}; domain=${2:?domain}; email=${3:-}
here=$(cd "$(dirname "$0")" && pwd)
site=/etc/nginx/sites-available/sukabaja

put() { sed "s/__DOMAIN__/$domain/g" "$here/nginx/$1" | ssh "$host" "sudo tee $site >/dev/null"; }
reload() { ssh "$host" "sudo ln -sf $site /etc/nginx/sites-enabled/sukabaja && sudo nginx -t && sudo systemctl reload nginx"; }

if ! ssh "$host" "sudo test -e /etc/letsencrypt/live/$domain/fullchain.pem"; then
    put sukabaja-http.conf; reload
    ssh "$host" "sudo certbot certonly --webroot -w /var/www/html -d $domain --non-interactive --agree-tos ${email:+-m $email}"
fi
put sukabaja-https.conf; reload
