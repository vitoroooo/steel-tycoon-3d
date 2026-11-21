"""A bundle of 6 m steel billets on timber dunnage, strapped; the product that leaves the smelter."""
import random
from kit import scene
from kit.finish import ready
from kit.parts import box

SECTION, LENGTH, ROWS, LAYERS = .15, 6.0, 6, 4
PITCH = SECTION + .012


def build():
    rng = random.Random(11)
    root = scene.root('Billet')
    parts = []
    for layer in range(LAYERS):
        z = .1 + layer * (PITCH + .05)
        for y in (-2.1, 0, 2.1):
            parts.append(ready(box('Dunnage', (ROWS * PITCH + .3, .09, .05), (0, y, z - .075 + .05), 'wood'), .004, 1))
        for row in range(ROWS):
            x = (row - (ROWS - 1) / 2) * PITCH
            bar = box('Bar', (SECTION, LENGTH, SECTION), (x, rng.uniform(-.12, .12), z + SECTION / 2 + .025), 'billet',
                      rot=(0, rng.uniform(-.03, .03), rng.uniform(-.004, .004)))
            parts.append(ready(bar, .012, 1, 'y'))
    top = .1 + LAYERS * (PITCH + .05)
    for y in (-1.4, 1.4):
        parts.append(ready(box('Strap', (ROWS * PITCH + .02, .04, top - .08), (0, y, top / 2 + .04), 'strap'), 0))
    scene.join('Body', parts, root)
    return 'Props', 'Billet'
