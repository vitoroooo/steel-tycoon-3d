"""Agent state, not aggregate progression timers."""
from dataclasses import dataclass, field


@dataclass
class Player:
    id: int
    kind: str
    sessions: int
    district: int
    born: int
    cash: float
    caste: int = 1
    reputation: float = 0
    vehicle: int = 0
    stock: float = 0
    stock_cost: float = 0
    assets: float = 0
    active: bool = True
    permit: int = 0
    ready: float = 0
    roof: bool = False
    profit: float = 0
    quarter: float = 0
    wait_since: int = 0
    milestones: dict = field(default_factory=dict)
    goal: int = 3
    turnover: float = 0
    sold_kg: float = 0
    lease_until: float = 0

    def mark(self, name, day):
        self.milestones.setdefault(name, day - self.born + 1)

    def earn(self, amount):
        self.cash += amount
        self.profit += amount
        self.quarter += amount
