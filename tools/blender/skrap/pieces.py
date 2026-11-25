"""Recognisable things thrown on the heap: sheets, pipes, beams, drums, rims, doors and tangles of rebar."""
import math
from kit.finish import ready
from kit.parts import box, cylinder, ring
from kit.tubes import tube
from . import mound


def spot(rng, spread=.92):
    """A random point on the heap, with a random tumble for whatever lands there."""
    angle, reach = rng.uniform(0, math.tau), rng.uniform(0, 1) ** .6 * spread
    x, y = math.cos(angle) * mound.RADIUS_X * reach, math.sin(angle) * mound.RADIUS_Y * reach
    turn = (rng.uniform(-.7, .7), rng.uniform(-.7, .7), rng.uniform(0, math.tau))
    return (x, y, mound.height(x, y) + .12), turn


def sheet(rng):
    at, turn = spot(rng)
    slot = rng.choice(('seng_rust', 'seng_rust', 'paint_blue', 'paint_red'))
    return ready(box('Sheet', (rng.uniform(.8, 2), rng.uniform(.5, 1.1), .012), at, slot, rot=turn), 0, 1)


def pipe(rng):
    at, turn = spot(rng)
    return cylinder('Pipe', rng.uniform(.04, .11), rng.uniform(1.2, 3.2), at, 'steel_dark',
                    rot=(1.5708 + turn[0] * .5, 0, turn[2]), segments=10)


def girder(rng):
    at, turn = spot(rng)
    return ready(box('Girder', (.14, rng.uniform(1.5, 3.4), .2), at, 'scrap', rot=(turn[0] * .5, 0, turn[2])), .01, 1, 'y')


def drum(rng):
    at, turn = spot(rng)
    slot = rng.choice(('paint_blue', 'paint_red', 'seng_rust'))
    return ready(cylinder('Drum', .29, .88, (at[0], at[1], at[2] + .2), slot, rot=turn, segments=16), .015, 1, 'z')


def rim(rng):
    at, turn = spot(rng)
    return ring('Rim', .24, .07, at, 'steel_dark', rot=turn, flatten=1.6, segments=18)


def door(rng):
    at, turn = spot(rng)
    return ready(box('Door', (1.05, .07, .85), (at[0], at[1], at[2] + .2), rng.choice(('paint_red', 'paint_blue')),
                     rot=turn), .03, 1)


def rebar(rng):
    """A bent bundle of reinforcing bar, as it comes out of a demolished slab."""
    at, _ = spot(rng, .8)
    points = [at]
    for _ in range(4):
        last = points[-1]
        points.append((last[0] + rng.uniform(-.9, .9), last[1] + rng.uniform(-.9, .9), last[2] + rng.uniform(-.1, .5)))
    return tube('Rebar', points, .012, 'rebar', .25, 6)


def scatter(rng):
    recipe = ((sheet, 16), (pipe, 12), (girder, 9), (drum, 7), (rim, 8), (door, 4), (rebar, 14))
    return [make(rng) for make, count in recipe for _ in range(count)]
