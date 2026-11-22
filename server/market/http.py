import json
import sqlite3
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

from .orders import MarketError, require
from .service import dispatch


class Handler(BaseHTTPRequestHandler):
    def setup(self):
        super().setup()
        self.connection.settimeout(8)

    def do_GET(self):
        self.handle_market(False)

    def do_POST(self):
        self.handle_market(True)

    def read_order(self):
        require(self.headers.get_content_type() == "application/json", 415, "Kirim pesanan JSON.")
        size = int(self.headers.get("Content-Length", "0"))
        require(0 < size <= 2048, 400, "Ukuran pesanan keliru.")
        return json.loads(self.rfile.read(size))

    def handle_market(self, is_order):
        try:
            route = "/v1/orders" if is_order else "/v1/market"
            require(self.path == route, 404, "Alamat tidak ditemukan.")
            auth = self.headers.get("Authorization", "")
            token = auth.removeprefix("Bearer ") if auth.startswith("Bearer ") else ""
            body = self.read_order() if is_order else None
            status, result = dispatch(self.server.database, token, body, is_order)
        except MarketError as error:
            status, result = error.status, {"message": str(error)}
        except (ValueError, RecursionError):
            status, result = 400, {"message": "Format pesanan keliru."}
        except sqlite3.Error:
            status, result = 503, {"message": "Basis data belum tersedia. Coba lagi dengan ID yang sama."}
        data = json.dumps(result, allow_nan=False).encode()
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Cache-Control", "no-store")
        self.send_header("Content-Length", str(len(data)))
        self.end_headers()
        self.wfile.write(data)

    def log_request(self, code="-", size="-"):
        # Do not record URLs or headers supplied by clients.
        pass


def serve(database, port):
    with ThreadingHTTPServer(("127.0.0.1", port), Handler) as server:
        server.database = database
        print(f"Pasar lokal pada 127.0.0.1:{server.server_port}", flush=True)
        server.serve_forever()
