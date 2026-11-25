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
