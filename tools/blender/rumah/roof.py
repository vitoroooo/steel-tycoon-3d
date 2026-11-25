"""Clay tile gable roof with ridge caps, fascia boards and the porch and kitchen lean-tos."""
import math
from kit.finish import ready
from kit.parts import box, cylinder
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)


def _slope(name, side):
    """One face of the gable; side is +1 for the lane face."""
    run = d.SPAN + d.OVERHANG
    length = run / math.cos(d.PITCH)
    y = d.MID + side * run / 2
    z = d.RIDGE - run / 2 * math.tan(d.PITCH) + .05
    slab = box(name, (d.WIDTH + .8, length, .07), (0, y, z), 'roof', rot=(-side * d.PITCH, 0, 0))
    return ready(slab, 0, 1.4, grain='y')


def main():
    parts = [_slope('RoofFront', 1), _slope('RoofBack', -1),
             ready(cylinder('Ridge', .11, d.WIDTH + .9, (0, d.MID, d.RIDGE + .09), 'roof', SIDEWAYS, 10), 0, 1.4)]
    eave_z = d.EAVE - d.OVERHANG * math.tan(d.PITCH) + .02
    for side in (-1, 1):   # fascia boards hide the rafter ends
        y = d.MID + side * (d.SPAN + d.OVERHANG)
        parts.append(ready(box('Fascia', (d.WIDTH + .8, .03, .16), (0, y, eave_z), 'wood'), 0, 1))
    return parts


def lean_to(name, y_wall, reach, half_width, x, mat, top, pitch=math.radians(14)):
    """A single-pitch roof leaning on a wall; reach is signed, positive towards the lane."""
    length = abs(reach) / math.cos(pitch)
    side = 1 if reach > 0 else -1
    z = top - abs(reach) / 2 * math.tan(pitch)
    slab = box(name, (half_width * 2, length, .04), (x, y_wall + reach / 2, z), mat, rot=(-side * pitch, 0, 0))
    return ready(slab, 0, 1.2, grain='y')


def porch():
    reach = d.PORCH + .3
    parts = [lean_to('PorchRoof', d.FRONT + d.OVERHANG - .1, reach - d.OVERHANG + .1, d.HALF + .3, 0, 'roof',
                     d.EAVE - d.OVERHANG * math.tan(d.PITCH) + .04)]
    beam_y = d.FRONT + d.PORCH - .15
    parts.append(ready(box('PorchBeam', (d.WIDTH + .3, .1, .12), (0, beam_y, 2.52), 'wood'), 0, 1))
    for x in (-d.HALF + .2, -.9, d.HALF - .2):
        parts.append(ready(box('PorchPost', (.1, .1, 2.34), (x, beam_y, .15 + 1.17), 'wood'), .006, 1))
    return parts


def dish():
    """The small satellite dish bolted to the ridge end."""
    x, y, z = d.HALF - .7, d.MID - .5, d.RIDGE - .15
    return [ready(cylinder('DishPole', .02, .9, (x, y, z + .3), 'steel', segments=6), 0, 1),
            ready(cylinder('Dish', .38, .03, (x, y - .05, z + .8), 'appliance', (1.1, 0, .4), 16), 0, 1),
            ready(cylinder('DishFeed', .012, .4, (x - .08, y - .22, z + .9), 'steel', (1.1, 0, .4), 5), 0, 1)]
