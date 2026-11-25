"""Lights, plate, mud flaps, lettering and the odds and ends a working triseda carries."""
import math
from kit.finish import ready
from kit.parts import blob, box, cylinder, ring
from kit.tubes import text, tube
from . import dims as d

REAR = d.BED_BACK - .02
FACING = (math.pi / 2, 0, 0)


def lights_and_plate():
    parts = []
    for side in (-1, 1):
        x = side * (d.BED_HALF_WIDTH - .09)
        parts.append(ready(box('LampHousing', (.15, .04, .08), (x, REAR, d.BED_FLOOR - .08), 'frame'), .006, .3))
        parts.append(ready(box('TailLamp', (.075, .016, .06), (x + side * .03, REAR - .022, d.BED_FLOOR - .08), 'lamp_red'), .004, .2))
        parts.append(ready(box('RearIndicator', (.05, .016, .06), (x - side * .04, REAR - .022, d.BED_FLOOR - .08), 'lamp_amber'), .004, .2))
        flap_x = side * d.REAR_X
        parts.append(ready(box('MudFlap', (.2, .008, .3), (flap_x, d.REAR_AXLE_Y - .36, d.BED_FLOOR - .2), 'rubber', (.12, 0, 0)), .002, .3))
        guard = box('WheelArch', (.2, .64, .012), (flap_x, d.REAR_AXLE_Y, d.BED_FLOOR - .03), 'frame')
        parts.append(ready(guard, .003, .4))
    plate_at = (0, REAR - .012, d.BED_FLOOR - .1)
    parts.append(ready(box('Plate', (.3, .006, .13), plate_at, 'plate'), .002, .3))
    number = text('PlateNumber', 'F 4821 SB', .062, (0, REAR - .016, d.BED_FLOOR - .1), 'plate_text',
                  (math.pi / 2, 0, 0), .001)
    return parts + [ready(number, 0, .3)]


def lettering():
    """Stencilled owner's name on both bed sides, readable from outside."""
    parts = []
    z = d.BED_FLOOR + d.BED_WALL - .045
    middle = (d.BED_FRONT + d.BED_BACK) / 2
    for side in (-1, 1):
        x = side * (d.BED_HALF_WIDTH + .0275)
        turn = (math.pi / 2, 0, side * math.pi / 2)
        words = text('Lettering', 'UD. ROSOK JAYA', .05, (x, middle, z), 'stencil', turn, .001)
        parts.append(ready(words, 0, .3))
    return parts


def load():
    """A coil of rope, a jerrycan and a spare tyre lashed inside the bed."""
    z = d.BED_FLOOR + .01
    parts = []
    for index in range(4):
        parts.append(ready(ring('RopeCoil', .13 - index * .008, .012, (-.38, -.12, z + .012 + index * .02), 'rope', (0, 0, index * .5), 1, 24), 0, .15))
    parts.append(ready(box('Jerrycan', (.13, .3, .38), (.46, -.1, z + .19), 'jerrycan'), .02, .5))
    parts.append(ready(cylinder('JerrySpout', .02, .05, (.46, -.01, z + .4), 'frame', segments=10), .003, .2))
    handle = [(.46, -.2, z + .38), (.46, -.2, z + .43), (.46, -.08, z + .43), (.46, -.08, z + .38)]
    parts.append(ready(tube('JerryHandle', handle, .009, 'jerrycan', bend=.02, sides=6), 0, .3))
    spare = (0, d.BED_FRONT - .09, d.BED_FLOOR + d.WHEEL_RADIUS + .02)
    parts.append(ready(ring('SpareTyre', d.WHEEL_RADIUS - d.TYRE, d.TYRE, spare, 'rubber', FACING, 1.05, 40), 0, .25))
    parts.append(ready(ring('SpareRim', d.WHEEL_RADIUS - d.TYRE - .05, .017, spare, 'rim', FACING, 2.6, 32), 0, .3))
    parts.append(ready(blob('Sack', (.5, .42, .3), (-.2, -.95, z + .15), 'sack', bulge=.5, lumps=.05), 0, .35))
    parts.append(ready(blob('Sack', (.44, .4, .26), (.26, -1.08, z + .13), 'sack', bulge=.5, lumps=.06, rot=(0, 0, .5)), 0, .35))
    return parts
