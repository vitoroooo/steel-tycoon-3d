"""Salvage set aside for resale: cleaned bricks on pallets, door leaves on a rack, window frames and roofing sheets."""
import math
import random
from kit import scene
from kit.finish import ready
from kit.members import beam
from kit.parts import box


def pallet(x, y, rng, courses):
    """A timber pallet carrying a cube of reclaimed bricks; the top course is left part-stacked."""
    parts = []
    for k in range(5):
        parts.append(ready(box('Slat', (1.1, .12, .025), (x, y - .44 + k * .22, .13), 'wood'), .003, 1, 'x'))
    for run in (-.45, 0, .45):
        parts.append(ready(box('Bearer', (.09, 1.0, .1), (x + run, y, .06), 'wood'), .004, 1, 'y'))
    for course in range(courses):
        last = course == courses - 1
        width = rng.uniform(.5, .9) if last else 1.0
        parts.append(ready(box('Bricks', (width, .9, .12), (x - (1 - width) / 2, y, .2 + course * .125), 'brick'), .004, .6))
    return parts


def door_rack(x, y, rng):
    """An A-frame with door leaves leaning against both sides, each still in its old paint."""
    parts = [ready(beam('RackLeg', (x + sx, y + sy, 0), (x + sx, y, 1.9), .06, .06, 'wood'), .004)
             for sx in (-1.0, 1.0) for sy in (-.45, .45)]
    parts.append(ready(box('RackRidge', (2.1, .06, .06), (x, y, 1.9), 'wood'), .004))
    for side in (-1, 1):
        for k in range(3):
            lean = side * (math.radians(13) + k * .012)
            at = (x - .55 + k * .5 + rng.uniform(-.05, .05), y + side * (.3 + k * .045), 1.0)
            paint = rng.choice(('paint_green', 'paint_blue', 'wood'))
            parts.append(ready(box('DoorLeaf', (.82, .04, 2.0), at, paint, rot=(lean, 0, rng.uniform(-.03, .03))), .005, 1, 'z'))
            parts.append(ready(box('DoorPanel', (.5, .012, .6), (at[0], at[1] + side * .028, 1.35), paint, rot=(lean, 0, 0)), .004, 1, 'z'))
    return parts


def frames_and_sheets(x, y, rng):
    """Window frames stacked flat, and a pile of roofing sheets pulled off the building."""
    parts = []
    for k in range(5):
        z, turn = .05 + k * .09, rng.uniform(-.12, .12)
        for dx, dy, sx, sy in ((0, -.5, 1.3, .07), (0, .5, 1.3, .07), (-.62, 0, .07, 1.0), (.62, 0, .07, 1.0), (0, 0, .05, 1.0)):
            c, s = math.cos(turn), math.sin(turn)
            parts.append(ready(box('Frame', (sx, sy, .07), (x + dx * c - dy * s, y + dx * s + dy * c, z), 'wood', rot=(0, 0, turn)), .004, 1))
    for k in range(7):
        at = (x + 2.1 + rng.uniform(-.06, .06), y + rng.uniform(-.06, .06), .03 + k * .03)
        parts.append(ready(box('Sheet', (.85, 2.0, .012), at, 'seng', rot=(rng.uniform(-.02, .02), 0, rng.uniform(-.08, .08))), 0, 1, 'y'))
    return parts


def build():
    rng = random.Random(311)
    root = scene.root('BahanBekas')
    parts = pallet(-2.6, 0, rng, 6) + pallet(-1.3, .1, rng, 4) + door_rack(1.0, 0, rng) + frames_and_sheets(-2.4, 1.7, rng)
    scene.join('Body', parts, root)
    return 'Props', 'BahanBekas'
