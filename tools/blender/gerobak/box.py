"""Cargo box: a welded angle-bar frame filled with mismatched, repainted planks."""
import math
import random
from kit.finish import ready
from kit.parts import box, cylinder
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)


def frame():
    parts = []
    top = d.FLOOR + d.WALL
    for z in (d.FLOOR - d.BAR / 2, top):
        for x in (-d.HALF_WIDTH, d.HALF_WIDTH):
            parts.append(box('Rail', (d.BAR, d.HALF_LENGTH * 2 + d.BAR, d.BAR), (x, 0, z), 'frame'))
        for y in (-d.HALF_LENGTH, d.HALF_LENGTH):
            parts.append(box('Rail', (d.HALF_WIDTH * 2 - d.BAR, d.BAR, d.BAR), (0, y, z), 'frame'))
    for x in (-d.HALF_WIDTH, d.HALF_WIDTH):
        for y in (-d.HALF_LENGTH, 0, d.HALF_LENGTH):
            parts.append(box('Post', (d.BAR, d.BAR, d.WALL), (x, y, d.FLOOR + d.WALL / 2), 'frame'))
    for x in (-.2, .2):   # cross members under the floor carry the axle brackets
        parts.append(box('Rib', (d.BAR, d.HALF_LENGTH * 2, d.BAR), (x, 0, d.FLOOR - d.BAR * 1.5), 'frame'))
    return [ready(p, .004, 1.2, 'z') for p in parts]


def _wall(rng, along, fixed, length, inward):
    """Four stacked planks; one is bare old wood and one is cut short, like a repair."""
    parts = []
    height = (d.WALL - d.BAR) / 4
    odd, short = rng.sample(range(4), 2)
    for row in range(4):
        size = length - (rng.uniform(.18, .3) if row == short else rng.uniform(0, .02))
        offset = (length - size) / 2 * rng.choice((-1, 1))
        z = d.FLOOR + height * (row + .5) + rng.uniform(-.003, .003)
        tilt = rng.uniform(-.006, .006)
        mat = 'wood' if row == odd else 'planks'
        if along == 'y':
            part = box('Plank', (d.PLANK, size, height - .007), (fixed + inward * d.PLANK, offset, z),
                       mat, (tilt, 0, 0))
        else:
            part = box('Plank', (size, d.PLANK, height - .007), (offset, fixed + inward * d.PLANK, z),
                       mat, (0, tilt, 0))
        # The painted texture has its boards across V, the bare wood along it.
        parts.append(ready(part, .003, .9, along if mat == 'wood' else 'z', rng))
    return parts


def planks(seed=7):
    rng = random.Random(seed)
    parts = []
    for side in (-1, 1):
        parts += _wall(rng, 'y', side * d.HALF_WIDTH, d.HALF_LENGTH * 2 - d.BAR, -side)
        parts += _wall(rng, 'x', side * d.HALF_LENGTH, d.HALF_WIDTH * 2 - d.BAR, -side)
    for x in (-d.HALF_WIDTH, d.HALF_WIDTH):   # nail heads where planks meet the posts
        for y in (-d.HALF_LENGTH + .05, -.03, .03, d.HALF_LENGTH - .05):
            for row in range(4):
                z = d.FLOOR + (d.WALL - d.BAR) / 4 * (row + .5)
                side = -1 if x > 0 else 1
                nail = cylinder('Nail', .005, .004, (x + side * d.PLANK * 1.55, y, z), 'steel', SIDEWAYS, 6)
                parts.append(ready(nail, 0, .3))
    width = d.HALF_WIDTH * 2 / 5
    for index in range(5):   # floor boards run front to back
        x = -d.HALF_WIDTH + width * (index + .5)
        board = box('Floor', (width - .006, d.HALF_LENGTH * 2 - d.BAR, .02),
                    (x, 0, d.FLOOR - .01 + rng.uniform(-.002, .002)), 'wood')
        parts.append(ready(board, .003, .9, 'y', rng))
    return parts
