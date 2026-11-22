"""Finds room for hand-fixed lots on a place of the baked map. Run after export.py:

    python3 tools/denah/sites.py PLACE WIDTH DEPTH [COUNT]

Prints "sites" entries for docs/design/peta/sukabaja.json: lots of WIDTH x DEPTH metres that lie wholly
on place number PLACE, face their nearest street and do not touch each other.
"""
import json
import sys
import numpy as np
from PIL import Image
from scipy import ndimage
from export import OUT

TURN, MARGIN, SETBACK = 3, 1.0, 12  # degrees a lot turns by; metres kept clear around it; how far back it may sit


def corners(x, z, yaw, width, depth, step=1.0):
    """Map points covering the lot and its margin."""
    front = np.array([np.sin(np.radians(yaw)), np.cos(np.radians(yaw))])
    right = np.array([front[1], -front[0]])
    a, b = np.meshgrid(np.arange(-width / 2 - MARGIN, width / 2 + MARGIN + .01, step),
                       np.arange(-depth / 2 - MARGIN, depth / 2 + MARGIN + .01, step))
    return np.array([x, z]) + a.reshape(-1, 1) * right + b.reshape(-1, 1) * front


def main(place, width, depth, count=1):
    land = np.asarray(Image.open(OUT / 'SukabajaLahan.bytes').convert('RGB')).astype(int)
    plan = json.loads((OUT / 'Sukabaja.json').read_text())
    west, north = plan['bounds']['west'], plan['bounds']['north']
    free = land[..., 0] == place + 1
    gap, (row_of, col_of) = ndimage.distance_transform_edt(land[..., 1] < 3, return_indices=True)

    def fits(x, z, yaw):
        spots = corners(x, z, yaw, width, depth)
        cols, rows = np.floor(spots[:, 0] - west).astype(int), np.floor(north - spots[:, 1]).astype(int)
        inside = (rows >= 0) & (rows < free.shape[0]) & (cols >= 0) & (cols < free.shape[1])
        return inside.all() and free[rows, cols].all(), rows, cols

    for _ in range(count):
        best = None
        rows_left, cols_left = np.nonzero(free)
        middle = cols_left.mean(), rows_left.mean()   # lots go to the ends of the land, so the next one still fits
        for row, col in zip(*np.nonzero(free)):
            if not depth / 2 <= gap[row, col] <= depth / 2 + SETBACK:
                continue
            x, z = col + west + .5, north - row - .5
            yaw = round(np.degrees(np.arctan2(col_of[row, col] - col, row - row_of[row, col])) / TURN) * TURN % 360
            good, rows, cols = fits(x, z, yaw)
            away = np.hypot(col - middle[0], row - middle[1])
            if good and (best is None or away > best[0]):
                best = (away, x, z, yaw, rows, cols)
        if best is None:
            raise SystemExit('Tidak ada tempat lagi di ' + plan['places'][place]['name'])
        free = free.copy()
        free[best[4], best[5]] = False
        print('{"name": "%s", "kind": "?", "x": %s, "z": %s, "yaw": %d, "width": %g, "depth": %g},'
              % (plan['places'][place]['name'], best[1], best[2], best[3], width, depth))


if __name__ == '__main__':
    main(int(sys.argv[1]), float(sys.argv[2]), float(sys.argv[3]), int(sys.argv[4]) if len(sys.argv) > 4 else 1)
