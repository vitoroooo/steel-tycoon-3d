CREATE TABLE IF NOT EXISTS city (
    id INTEGER PRIMARY KEY CHECK (id = 1),
    opened_at INTEGER NOT NULL,
    last_time INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS traders (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    token_hash BLOB NOT NULL UNIQUE,
    cash INTEGER NOT NULL CHECK (typeof(cash) = 'integer' AND cash >= 0),
    stock INTEGER NOT NULL CHECK (typeof(stock) = 'integer' AND stock >= 0),
    accrued_at INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS lots (
    id INTEGER PRIMARY KEY,
    seller INTEGER NOT NULL REFERENCES traders,
    kg INTEGER NOT NULL CHECK (kg > 0),
    price INTEGER NOT NULL CHECK (price > 0),
    status TEXT NOT NULL CHECK (status IN ('open', 'sold', 'cancelled'))
);
CREATE UNIQUE INDEX IF NOT EXISTS open_lot_per_trader ON lots(seller) WHERE status = 'open';
CREATE TABLE IF NOT EXISTS trades (
    lot INTEGER PRIMARY KEY REFERENCES lots,
    buyer INTEGER NOT NULL REFERENCES traders,
    gross INTEGER NOT NULL CHECK (gross > 0),
    tax INTEGER NOT NULL CHECK (tax BETWEEN 0 AND gross),
    game_hour INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS requests (
    trader INTEGER NOT NULL REFERENCES traders,
    key TEXT NOT NULL,
    payload TEXT NOT NULL,
    status INTEGER NOT NULL,
    receipt TEXT NOT NULL,
    PRIMARY KEY (trader, key)
);
CREATE TABLE IF NOT EXISTS suspensions (
    trader INTEGER PRIMARY KEY REFERENCES traders
);
CREATE TABLE IF NOT EXISTS moderator_events (
    id INTEGER PRIMARY KEY,
    trader INTEGER NOT NULL REFERENCES traders,
    operation TEXT NOT NULL CHECK(operation IN ('suspend','restore')),
    recorded_at INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS contracts (
    id INTEGER PRIMARY KEY,
    seller INTEGER NOT NULL REFERENCES traders,
    buyer INTEGER NOT NULL REFERENCES traders CHECK (buyer != seller),
    kg INTEGER NOT NULL CHECK (kg > 0),
    price INTEGER NOT NULL CHECK (price > 0),
    days INTEGER NOT NULL CHECK (days > 0),
    delivered INTEGER NOT NULL CHECK (delivered BETWEEN 0 AND days),
    locked INTEGER NOT NULL CHECK (locked >= 0),
    due INTEGER NOT NULL,
    penalty INTEGER NOT NULL DEFAULT 0,
    status TEXT NOT NULL CHECK (status IN ('offered', 'active', 'done', 'withdrawn', 'broken'))
);
