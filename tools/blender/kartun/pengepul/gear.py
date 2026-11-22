"""What marks him as a tukang rosok: neck towel, sling bag and a hand scale on the strap."""
from .. import shapes
from ..paint import cel


def towel():
    """Draped round the neck, the left end longer: it is what he wipes his face with."""
    ring = shapes.arc((0, .0, .8), .15, 0, 360, 'xy', 15)[:-1]
    parts = [shapes.tube('Handuk', ring, .04, cel('white'), True)]
    for side, drop in ((-1, .21), (1, .15)):
        top, end = (side * .08, -.165, .8), (side * .105, -.205, .8 - drop)
        parts.append(shapes.capsule('Ujung handuk', top, end, .04, .046, cel('white')))
        band = tuple(t + (e - t) * .78 for t, e in zip(top, end))
        parts.append(shapes.capsule('Garis handuk', band, tuple(t + (e - t) * .88 for t, e in zip(top, end)),
                                    .049, .05, cel('teal')))
    return parts


def sling_bag():
    strap = [(.2, -.02, .72), (.12, -.17, .66), (-.03, -.2, .56), (-.17, -.15, .46),
             (-.235, 0, .42), (-.16, .17, .5), (.02, .2, .62), (.17, .12, .7)]
    return [
        shapes.tube('Tali tas', strap, .017, cel('navy'), True),
        shapes.block('Tas', (-.245, -.06, .37), (.12, .21, .17), cel('navy'), .3),
        shapes.block('Tutup tas', (-.262, -.06, .425), (.105, .215, .07), cel('teal'), .35),
        shapes.ball('Kancing', (-.315, -.06, .40), (.014, .022, .022), cel('yellow')),
        shapes.block('Gesper', (.06, -.203, .615), (.05, .02, .04), cel('yellow'), .2, .2),
    ]
