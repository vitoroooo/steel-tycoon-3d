#!/usr/bin/env bash
# Membuat akun pemain dan menampilkan tokennya sekali:  deploy/account.sh <ssh host> <nama> [--rotate]
set -euo pipefail
host=${1:?ssh host}; name=${2:?nama}; rotate=${3:-}
ssh "$host" "cd /opt/sukabaja && sudo -u sukabaja python3 -m server.market --db /var/lib/sukabaja/pasar.sqlite3 trader '$name' $rotate"
