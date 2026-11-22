"""Loads the city plan and turns map metres into picture pixels."""
import json
from pathlib import Path

PLAN = Path(__file__).resolve().parents[2] / 'docs/design/peta/sukabaja.json'
SCALE = 2.6          # pixels per metre
MARGIN = 40


def load():
    return json.loads(PLAN.read_text())


def px(plan, x, z):
    """Picture position of a map point; north is up."""
    return (MARGIN + (x - plan['x'][0]) * SCALE, MARGIN + (plan['z'][-1] - z) * SCALE)


def box(plan, west, south, east, north):
    left, top = px(plan, west, north)
    right, bottom = px(plan, east, south)
    return [left, top, right, bottom]


def cell_box(plan, col, row_from_north):
    """Bounds of one grid cell; rows are counted from the north, as written in the plan."""
    row = len(plan['z']) - 2 - row_from_north
    return box(plan, plan['x'][col], plan['z'][row], plan['x'][col + 1], plan['z'][row + 1])


def inner(plan, bounds, west, south, east, north):
    """The part of a block left after the streets along its edges take their half width."""
    def half(lines, at):
        return plan['widths'].get(lines.get(str(at), ''), 0) / 2 * SCALE
    return [bounds[0] + half(plan['linesX'], west), bounds[1] + half(plan['linesZ'], north),
            bounds[2] - half(plan['linesX'], east), bounds[3] - half(plan['linesZ'], south)]


def cell_inner(plan, col, row_from_north):
    row = len(plan['z']) - 2 - row_from_north
    return inner(plan, cell_box(plan, col, row_from_north),
                 plan['x'][col], plan['z'][row], plan['x'][col + 1], plan['z'][row + 1])


def size(plan):
    width = (plan['x'][-1] - plan['x'][0]) * SCALE + MARGIN * 2
    height = (plan['z'][-1] - plan['z'][0]) * SCALE + MARGIN * 2
    return int(width), int(height)
