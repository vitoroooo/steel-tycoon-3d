import argparse
import os
import sqlite3
from pathlib import Path

from .http import serve
from .moderation import moderate
from .storage import initialize, provision


def main():
    parser = argparse.ArgumentParser(description="Pasar lokal Steel Tycoon")
    parser.add_argument("--db", type=Path, required=True)
    commands = parser.add_subparsers(dest="command", required=True)
    trader = commands.add_parser("trader", help="Terbitkan token akun lokal")
    trader.add_argument("name")
    trader.add_argument("--rotate", action="store_true", help="Ganti token akun yang sudah ada")
    for action in ("suspend", "restore"):
        commands.add_parser(action).add_argument("name")
    server = commands.add_parser("serve")
    server.add_argument("--port", type=int, default=8765)
    args = parser.parse_args()
    os.umask(0o077)
    initialize(args.db)
    if args.command == "serve":
        serve(args.db, args.port)
        return
    try:
        if args.command != "trader":
            moderate(args.db, args.name, args.command == "suspend")
            return
        print(provision(args.db, args.name, args.rotate))
    except (ValueError, sqlite3.IntegrityError) as error:
        parser.exit(1, f"Gagal memproses akun. {error}\n")


if __name__ == "__main__":
    main()
