"""A scrap-yard gantry crane. The trolley and its hanging magnet are separate objects so Unity can move them."""
import random
from kit import scene
from kit.finish import ready
from kit.members import beam
from kit.parts import box, cylinder
from kit.tubes import tube

SPAN, HEIGHT, RUN = 16.0, 9.0, 26.0     # between the legs, under the girders, length of the ground rails
HALF = SPAN / 2
HANG = HEIGHT - .1                      # where the load hangs from: the pivot of its swing
MAGNET_Z = 4.6


def gantry():
    parts = []
    for side in (-1, 1):
        x = side * HALF
        parts.append(ready(box('GroundRail', (.1, RUN, .14), (x, 0, .07), 'steel_dark'), 0, 2, 'y'))
        parts.append(ready(box('EndTruck', (.5, 5.6, .5), (x, 0, .55), 'crane'), .02, 2, 'y'))
        parts.append(ready(box('Sill', (.4, 2.4, .4), (x, 0, HEIGHT - .2), 'crane'), .02))
        for y in (-2.3, 2.3):
            parts.append(cylinder('TruckWheel', .26, .16, (x, y, .3), 'steel_dark', rot=(0, 1.5708, 0), segments=18))
            leg = beam('Leg', (x, y * .95, .8), (x, y * .4, HEIGHT - .1), .38, .44, 'crane')
            parts.append(ready(leg, .02, 2, 'y'))
        parts.append(ready(beam('LegTie', (x, -1.6, 4.3), (x, 1.6, 4.3), .2, .24, 'crane'), .01))
        parts.append(ready(box('Buffer', (.3, .3, .3), (x, RUN / 2 - .2, .32), 'rail'), .02))
        parts.append(ready(box('Buffer', (.3, .3, .3), (x, -RUN / 2 + .2, .32), 'rail'), .02))
    for y in (-.95, .95):
        parts.append(ready(box('Girder', (SPAN + 1.6, .5, 1.1), (0, y, HEIGHT + .55), 'crane'), .02, 2))
        parts.append(ready(box('TrolleyRail', (SPAN + 1.2, .07, .08), (0, y, HEIGHT + 1.14), 'steel_dark'), 0, 2))
    for height in (.55, 1.05):          # walkway handrail along the back girder
        parts.append(tube('Handrail', [(-HALF, -1.6, HEIGHT + 1.1 + height), (HALF, -1.6, HEIGHT + 1.1 + height)],
                          .022, 'rail', 0, 6))
    parts.append(ready(box('Walkway', (SPAN, .6, .05), (0, -1.5, HEIGHT + 1.1), 'deck'), 0, 1.5))
    parts.append(ready(box('Cab', (1.7, 1.7, 1.9), (HALF - 2.2, -2.1, HEIGHT - 1.1), 'cabin'), .03, 1.5))
    parts.append(ready(box('CabGlass', (1.72, 1.4, .8), (HALF - 2.2, -2.2, HEIGHT - .9), 'glass'), 0))
    return parts


def trolley():
    parts = [ready(box('Frame', (2.3, 2.5, .45), (0, 0, HEIGHT + 1.45), 'steel_dark'), .03),
             cylinder('Drum', .32, 1.5, (0, 0, HEIGHT + 1.95), 'crane', rot=(1.5708, 0, 0), segments=18),
             ready(box('Motor', (.7, .6, .55), (.75, .7, HEIGHT + 1.95), 'cabin'), .04)]
    for x in (-.95, .95):
        for y in (-.95, .95):
            parts.append(cylinder('Wheel', .16, .1, (x, y, HEIGHT + 1.3), 'steel_dark', rot=(1.5708, 0, 0), segments=14))
    return parts


def load(rng):
    """Ropes, hook block, chains and the lifting magnet with a few pieces of scrap stuck to it."""
    parts = [ready(box('HookBlock', (.6, .45, .5), (0, 0, MAGNET_Z + 1.5), 'crane'), .04),
             ready(cylinder('Magnet', .95, .3, (0, 0, MAGNET_Z), 'magnet', segments=28), .03),
             cylinder('MagnetFace', .8, .06, (0, 0, MAGNET_Z - .17), 'steel_dark', segments=28)]
    for x in (-.18, .18):
        parts.append(tube('Rope', [(x, 0, HANG + 1.6), (x, 0, MAGNET_Z + 1.7)], .018, 'steel_dark', 0, 6))
    for x, y in ((.6, 0), (-.3, .52), (-.3, -.52)):
        parts.append(tube('Chain', [(0, 0, MAGNET_Z + 1.3), (x, y, MAGNET_Z + .15)], .028, 'steel_dark', 0, 6))
    for _ in range(9):
        size = (rng.uniform(.2, .8), rng.uniform(.1, .5), rng.uniform(.02, .12))
        at = (rng.uniform(-.6, .6), rng.uniform(-.6, .6), MAGNET_Z - .25 - rng.uniform(0, .25))
        turn = (rng.uniform(-.5, .5), rng.uniform(-.5, .5), rng.uniform(0, 3))
        parts.append(ready(box('Stuck', size, at, 'scrap', rot=turn), .004))
    return parts


def build():
    root = scene.root('Derek')
    scene.join('Body', gantry(), root)
    carriage = scene.join('Trolley', trolley(), root)
    hanging = scene.pivot(scene.join('Load', load(random.Random(7)), None), (0, 0, HANG + 1.6))
    hanging.parent = carriage
    return 'Props', 'Derek'
