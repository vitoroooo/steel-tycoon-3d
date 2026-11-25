"""Salinan harian basis data pemain:  python3 -m server.backup <basis data> <folder> [jumlah disimpan]

Memakai salinan daring SQLite, jadi aman dijalankan saat server pasar dan server karier sedang menulis.
"""
import sqlite3
import sys
import time
from contextlib import closing
from pathlib import Path


def backup(database, folder, keep):
    folder.mkdir(parents=True, exist_ok=True)
    target = folder / time.strftime("pasar-%Y%m%d-%H%M%S.sqlite3", time.gmtime())
    with closing(sqlite3.connect(database)) as source, closing(sqlite3.connect(target)) as copy:
        source.backup(copy)
        if copy.execute("PRAGMA integrity_check").fetchone()[0] != "ok":
            raise SystemExit("Salinan rusak: " + str(target))
    for old in sorted(folder.glob("pasar-*.sqlite3"))[:-keep]:
        old.unlink()
    return target


if __name__ == "__main__":
    if len(sys.argv) < 3:
        raise SystemExit(__doc__)
    print(backup(sys.argv[1], Path(sys.argv[2]), int(sys.argv[3]) if len(sys.argv) > 3 else 14))
