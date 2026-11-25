"""Everything that swings from the shoulders: neck, hackle, head, comb, wattles, beak and eyes."""
import math
from kit.members import frustum
from .dims import HEAD
from .feathers import egg, plume, smooth


def neck():
    parts = [egg('Neck', (.088, .1, .15), (0, .14, .33), 'feather_neck', lumps=.02, rot=(-.5, 0, 0)),
             egg('Throat', (.06, .068, .11), (0, .185, .4), 'feather_neck', lumps=.01, rot=(-.3, 0, 0))]
    # The hackle: long pointed feathers hanging from the nape down over the shoulders.
    for i in range(14):
        angle = math.pi * (.5 + (i - 6.5) / 7.6)          # all round except under the throat
        out = (math.cos(angle), 0, math.sin(angle) * .4 - .1)
        root = (out[0] * .026, .185 - out[2] * .02, .425)
        bend = (out[0] * .062, .14, .35 + out[2] * .05)
        tip = (out[0] * .085, .07 - abs(out[0]) * .02, .285 + math.sin(angle) * .04)
        parts.append(plume('Hackle', root, bend, tip, .025, (out[0], .25, max(out[2], .1)), 'feather_neck'))
    return parts


def head():
    x, y, z = HEAD
    parts = [egg('Head', (.052, .07, .058), HEAD, 'feather_neck', lumps=.01)]
    upper = frustum('Beak', .0125, .001, .042, (x, y + .048, z - .004), 'beak', segments=8)
    lower = frustum('Beak', .009, .001, .03, (x, y + .04, z - .014), 'beak', segments=8)
    upper.rotation_euler, lower.rotation_euler = (-math.pi / 2 - .22, 0, 0), (-math.pi / 2 - .1, 0, 0)
    parts += [smooth(upper), smooth(lower)]
    # A single serrated comb: five points, tallest in the middle, leaning back off the skull.
    for i, tall in enumerate((.022, .034, .04, .034, .024)):
        at = (x, y + .03 - i * .017, z + .026 + tall * .42)
        parts.append(egg('Comb', (.009, .026, tall), at, 'comb', lumps=.003, rot=(-.25 + i * .09, 0, 0)))
    for side in (-1, 1):
        parts.append(egg('Wattle', (.011, .017, .036), (x + side * .011, y + .022, z - .042), 'comb', lumps=.003))
        parts.append(egg('Lobe', (.006, .014, .017), (x + side * .025, y - .008, z - .012), 'comb'))
        parts.append(egg('Eye', (.009, .011, .011), (x + side * .0235, y + .017, z + .006), 'eye'))
    return parts
