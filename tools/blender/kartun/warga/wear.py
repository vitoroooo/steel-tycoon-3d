"""What a citizen carries on the torso. Each returns parts for the spine joint."""
from .. import shapes
from ..paint import cel


def nothing():
    return []


def shopping_bag():
    """A woven market bag on the right shoulder, a bunch of greens sticking out."""
    strap = [(-.2, 0, .72), (-.27, -.03, .58), (-.29, -.04, .44), (-.27, .05, .58)]
    return [shapes.tube('Tali tas belanja', strap, .014, cel('wood'), True),
            shapes.block('Tas belanja', (-.3, -.02, .34), (.13, .3, .24), cel('cream'), .3),
            shapes.block('Motif tas', (-.368, -.02, .34), (.012, .3, .06), cel('orange'), .3),
            shapes.capsule('Sayur', (-.3, -.08, .44), (-.31, -.12, .56), .04, .07, cel('green'), 12)]


def vest():
    """A hi-vis vest: open at the front, two reflective bands."""
    parts = [shapes.capsule('Rompi', (0, .012, .5), (0, .012, .7), .212, .188, cel('orange'), 28)]
    for z, radius in ((.54, .214), (.63, .2)):
        ring = shapes.arc((0, .012, z), radius, 0, 360, 'xy', 17)[:-1]
        parts.append(shapes.tube('Pita rompi', ring, .013, cel('cream'), True))
    parts.append(shapes.block('Kartu nama', (.09, -.19, .64), (.07, .02, .05), cel('white'), .3))
    return parts


def chest_pocket():
    return [shapes.block('Saku', (.09, -.178, .62), (.08, .02, .07), cel('cream'), .25),
            shapes.capsule('Pulpen', (.105, -.19, .63), (.105, -.19, .69), .009, .009, cel('navy'), 8)]
