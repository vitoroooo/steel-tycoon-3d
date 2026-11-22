#!/usr/bin/env bash
# Memasang atau memperbarui server game di VPS:  deploy/push.sh <ssh host>
# Aman diulang. Tidak menyentuh situs lain, tidak menyentuh /var/lib/sukabaja (akun dan karier pemain).
set -euo pipefail
host=${1:?pakai: deploy/push.sh <ssh host>}
root=$(cd "$(dirname "$0")/.." && pwd)
stage=$(mktemp -d); trap 'rm -rf "$stage"' EXIT

dotnet publish "$root/server/career" -nologo -v q -c Release -r linux-x64 --self-contained -o "$stage/career"
mkdir -p "$stage/server"
rsync -a --exclude __pycache__ --exclude career --exclude tests "$root/server/" "$stage/server/"
# server pasar membaca aturan dari jalur repo ini, jadi susunan foldernya dipertahankan
rules=unity/Assets/_Project/Resources
mkdir -p "$stage/$rules"
rsync -a --prune-empty-dirs --include '*/' --include '*.json' --exclude '*' "$root/$rules/" "$stage/$rules/"
cp -r "$root/deploy/systemd" "$stage/systemd"

rsync -az --delete "$stage/" "$host:sukabaja-release/"
ssh "$host" 'sudo bash -s' <<'REMOTE'
set -euo pipefail
id sukabaja >/dev/null 2>&1 || useradd --system --home /var/lib/sukabaja --shell /usr/sbin/nologin sukabaja
install -d -o sukabaja -g sukabaja -m 750 /var/lib/sukabaja
install -d /opt/sukabaja
rsync -a --delete --exclude systemd /home/ubuntu/sukabaja-release/ /opt/sukabaja/
chown -R root:root /opt/sukabaja && chmod -R a+rX /opt/sukabaja && chmod 755 /opt/sukabaja/career/Steel.Career
install -m 644 /home/ubuntu/sukabaja-release/systemd/* /etc/systemd/system/
systemctl daemon-reload
systemctl enable --now sukabaja-market sukabaja-career
systemctl enable --now sukabaja-backup.timer
systemctl restart sukabaja-market sukabaja-career
sleep 3
systemctl is-active sukabaja-market sukabaja-career
REMOTE
