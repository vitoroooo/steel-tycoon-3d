"""The weighbridge at the smelter gate: a steel deck between ramps, the clerk's office beside it, a raised barrier."""
from kit import scene
from kit.finish import ready
from kit.members import beam
from kit.parts import box, cylinder
from kit.tubes import tube

DECK_W, DECK_L, DECK_H = 3.4, 12.0, .07   # the truck drives along Y; a pit bridge, flush with the yard
OFFICE_X, OFFICE = 4.6, (4.4, 3.4, 2.9)   # office centre and its width, depth, height


def bridge():
    parts = [ready(box('Deck', (DECK_W, DECK_L, .05), (0, 0, DECK_H - .025), 'deck'), .01, 1.5, 'y'),
             ready(box('Pit', (DECK_W + .7, DECK_L + .5, .05), (0, 0, .02), 'concrete'), 0, 2)]
    for end in (-1, 1):
        y = end * (DECK_L / 2 + 1.5)
        parts.append(ready(box('Apron', (DECK_W + .7, 2.6, .05), (0, y, .01), 'concrete'), 0, 2))
        for x in (-1.2, 0, 1.2):   # the load cells sit under these inspection covers
            parts.append(ready(box('Cover', (.5, .5, .012), (x, end * (DECK_L / 2 - .5), DECK_H + .004), 'steel_dark'), 0))
    for side in (-1, 1):
        x = side * (DECK_W / 2 + .12)
        parts.append(ready(box('Kerb', (.14, DECK_L, .1), (x, 0, .05), 'rail'), .02, 1, 'y'))
        for y in (-DECK_L / 2 - .4, DECK_L / 2 + .4):
            parts.append(cylinder('Bollard', .09, 1, (x + side * .25, y, .5), 'rail', segments=12))
    return parts


def office():
    w, depth, h = OFFICE
    x = OFFICE_X
    parts = [ready(box('Walls', (w, depth, h), (x, 0, h / 2), 'plaster'), .02, 2),
             ready(box('Plinth', (w + .2, depth + .2, .3), (x, 0, .15), 'concrete'), .02, 2),
             ready(box('RoofSlab', (w + 1.6, depth + 1, .16), (x - .4, 0, h + .08), 'concrete'), .02, 2),
             ready(box('Counter', (.1, 1.7, 1), (x - w / 2 - .02, .4, 1.6), 'glass'), 0),
             ready(box('CounterFrame', (.12, 1.9, .08), (x - w / 2 - .03, .4, 1.06), 'steel_dark'), .01),
             ready(box('Sill', (.4, 1.9, .05), (x - w / 2 - .2, .4, 1.04), 'concrete'), .01),
             ready(box('Door', (.9, .08, 2.05), (x + .9, depth / 2 + .01, 1.32), 'door'), .01, 1, 'z'),
             ready(box('Window', (1.3, .06, 1), (x - .8, depth / 2 + .01, 1.7), 'glass'), 0),
             ready(box('Aircon', (.8, .3, .55), (x + 1.1, -depth / 2 - .17, 2.1), 'cabin'), .03),
             ready(box('SignBoard', (3.6, .1, .7), (x - .4, depth / 2 + .5, h + .55), 'sign'), .01),
             ready(box('Bench', (1.6, .4, .06), (x - .7, depth / 2 + .45, .46), 'wood'), .01, 1)]
    for bx in (-1.4, 0):
        parts.append(ready(box('BenchLeg', (.06, .36, .44), (x + bx, depth / 2 + .45, .22), 'wood'), .005))
    # The weight indicator the driver reads from the cab.
    parts.append(tube('DisplayPole', [(DECK_W / 2 + .7, 4.8, 0), (DECK_W / 2 + .7, 4.8, 3)], .05, 'steel_dark', 0, 8))
    parts.append(ready(box('Display', (.16, 1.3, .55), (DECK_W / 2 + .7, 4.8, 3.2), 'steel_dark'), .02))
    parts.append(ready(box('Digits', (.02, 1.1, .36), (DECK_W / 2 + .61, 4.8, 3.2), 'digits'), 0))
    return parts


def barrier():
    """A boom that stands raised; the gate is open during the day."""
    x, y = -DECK_W / 2 - .7, -DECK_L / 2 - 3.2
    parts = [ready(box('BoomCabinet', (.4, .4, 1.05), (x, y, .52), 'rail'), .02),
             ready(beam('Boom', (x, y, 1), (x + 1.3, y, 5.2), .1, .12, 'band_white'), .01)]
    for step in range(4):
        low = (x + .2 + step * .3, y, 1.65 + step * .97)
        high = (x + .2 + step * .3 + .12, y, 1.65 + step * .97 + .4)
        parts.append(ready(beam('BoomStripe', low, high, .105, .125, 'band_red'), 0))
    return parts


def build():
    root = scene.root('PosTimbang')
    scene.join('Body', bridge() + office() + barrier(), root)
    return 'Buildings', 'PosTimbang'
