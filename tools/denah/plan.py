"""Loads the city plan and turns map metres into picture pixels."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
PLAN = ROOT / 'docs/design/peta/sukabaja.json'
SCALE = 3.0          # pixels per metre
MARGIN = 30


def load():
    return json.loads(PLAN.read_text())


def px(plan, x, z):
    """Picture position of a map point; north is up."""
    west, _, _, north = plan['bounds']
    return (MARGIN + (x - west) * SCALE, MARGIN + (north - z) * SCALE)


def metres(plan, left, top):
    """The map point under a picture pixel."""
    west, _, _, north = plan['bounds']
    return (west + (left - MARGIN) / SCALE, north - (top - MARGIN) / SCALE)


def size(plan):
    west, south, east, north = plan['bounds']
    return int((east - west) * SCALE + MARGIN * 2), int((north - south) * SCALE + MARGIN * 2)


def area_ratio(plan):
    """How many times the playable area of today's map fits into the plan."""
    west, south, east, north = plan['bounds']
    old_west, old_south, old_east, old_north = plan['current']
    return (east - west) * (north - south) / ((old_east - old_west) * (old_north - old_south))
