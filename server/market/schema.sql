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
CREATE TABLE IF NOT EXISTS associations (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL UNIQUE COLLATE NOCASE
);
CREATE TABLE IF NOT EXISTS members (
    trader INTEGER PRIMARY KEY REFERENCES traders,
    association INTEGER NOT NULL REFERENCES associations
);
CREATE TABLE IF NOT EXISTS messages (
    id INTEGER PRIMARY KEY,
    association INTEGER NOT NULL REFERENCES associations,
    trader INTEGER NOT NULL REFERENCES traders,
    body TEXT NOT NULL,
    game_hour INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS reports (
    message INTEGER NOT NULL,
    reporter INTEGER NOT NULL REFERENCES traders,
    author INTEGER NOT NULL REFERENCES traders,
    body TEXT NOT NULL,
    PRIMARY KEY (message, reporter)
);
CREATE TABLE IF NOT EXISTS standing (
    trader INTEGER PRIMARY KEY REFERENCES traders,
    caste INTEGER NOT NULL CHECK (caste BETWEEN 1 AND 3),
    since INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS blockades (
    id INTEGER PRIMARY KEY,
    association INTEGER NOT NULL,
    hirer INTEGER NOT NULL REFERENCES traders,
    target INTEGER NOT NULL REFERENCES traders CHECK (target != hirer),
    start INTEGER NOT NULL,
    until INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS season_state (
    id INTEGER PRIMARY KEY CHECK (id = 1),
    number INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS legacies (
    trader INTEGER NOT NULL REFERENCES traders,
    season INTEGER NOT NULL,
    city TEXT NOT NULL,
    value INTEGER NOT NULL,
    rank INTEGER NOT NULL,
    points INTEGER NOT NULL,
    PRIMARY KEY (trader, season)
);
CREATE TABLE IF NOT EXISTS groups (
    trader INTEGER PRIMARY KEY REFERENCES traders,
    points INTEGER NOT NULL DEFAULT 0,
    picks TEXT NOT NULL DEFAULT '',
    picked INTEGER NOT NULL DEFAULT 0
);
