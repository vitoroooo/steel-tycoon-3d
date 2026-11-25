"""What is left of the building next door, already torn down: column stumps, knee-high walls and a cracked floor."""
import random
from kit import scene
from kit.finish import ready
from kit.parts import box
from bongkaran import dims as d
from bongkaran.frame import rebar

WIDTH, DEPTH, GRID = 9.0, 9.0, 4.5


def stumps(rng):
    parts = []
    for i in range(3):
        for j in range(3):
            x, y, height = -WIDTH / 2 + i * GRID, -DEPTH / 2 + j * GRID, rng.uniform(.25, 1.3)
            parts.append(ready(box('Stump', (d.COLUMN, d.COLUMN, height), (x, y, height / 2), 'concrete'), .015, 1.5, 'z'))
            parts += rebar(x, y, height, rng, 4, .7)
    return parts


def low_walls(rng):
    """Brickwork knocked down to knee height, in strips so the top edge is ragged; one bay is left open as a way in."""
    parts = []
    for side, along_x in ((-1, True), (1, True), (-1, False), (1, False)):
        for k in range(12):
            if along_x and side == 1 and 4 <= k <= 6:
                continue
            height, length = rng.uniform(.15, .95), WIDTH / 12
            run = -WIDTH / 2 + (k + .5) * length
            at = (run, side * DEPTH / 2, height / 2) if along_x else (side * WIDTH / 2, run, height / 2)
            size = (length, .15, height) if along_x else (.15, length, height)
            parts.append(ready(box('LowWall', size, at, 'brick'), 0, 1.2))
    return parts


def floor(rng):
    """The ground slab, broken into plates that have settled at slightly different angles."""
    parts = []
    for i in range(3):
        for j in range(3):
            if rng.random() < .2:
                continue
            at = (-WIDTH / 3 + i * WIDTH / 3, -DEPTH / 3 + j * DEPTH / 3, .05 + rng.uniform(0, .05))
            tilt = (rng.uniform(-.04, .04), rng.uniform(-.04, .04), rng.uniform(-.06, .06))
            parts.append(ready(box('Plate', (WIDTH / 3 - .12, DEPTH / 3 - .12, .1), at, 'floor', rot=tilt), .01, 2))
    for _ in range(14):
        at = (rng.uniform(-4, 4), rng.uniform(-4, 4), .2)
        size = (rng.uniform(.25, .6), rng.uniform(.25, .5), rng.uniform(.2, .45))
        parts.append(ready(box('Chunk', size, at, 'concrete', rot=(rng.uniform(-.5, .5), rng.uniform(-.5, .5), rng.uniform(0, 3))), .015, 1.5))
    return parts


def build():
    rng = random.Random(77)
    root = scene.root('Pondasi')
    scene.join('Body', stumps(rng) + low_walls(rng) + floor(rng), root)
    return 'Buildings', 'Pondasi'
