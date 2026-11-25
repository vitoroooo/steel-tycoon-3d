-- kept by the career server (server/career): one whole career per account; a closed season empties it
CREATE TABLE IF NOT EXISTS careers (
    trader INTEGER PRIMARY KEY REFERENCES traders,
    save TEXT NOT NULL,
    seen_ms INTEGER NOT NULL,
    seq INTEGER NOT NULL,
    reply TEXT NOT NULL
);
-- written by the career server (server/career): what it last wrote into traders.cash and traders.stock
CREATE TABLE IF NOT EXISTS wallets (
    trader INTEGER PRIMARY KEY REFERENCES traders,
    cash INTEGER NOT NULL,
    stock INTEGER NOT NULL
);
