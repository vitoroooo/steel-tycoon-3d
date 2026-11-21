"""The masonry: plinth, storeys, corner columns, slabs and the roof parapet."""
from kit.finish import ready
from kit.parts import box
from . import dims as d

TILE = 2.0   # metres of wall per texture repeat


def _wall(name, size, at, mat='wall'):
    return ready(box(name, size, at, mat), 0, TILE)


def storeys(floors):
    top = d.roof_z(floors)
    shop_depth = d.DEPTH - d.TERRACE
    parts = [
        _wall('Shop', (d.WIDTH, shop_depth, d.GROUND_FLOOR), (0, d.BACK + shop_depth / 2, d.GROUND_FLOOR / 2)),
        _wall('Upper', (d.WIDTH, d.DEPTH, top - d.GROUND_FLOOR), (0, 0, (top + d.GROUND_FLOOR) / 2)),
        _wall('BackWall', (d.WIDTH + .02, .04, top), (0, d.BACK - .02, top / 2), 'backwall'),
    ]
    for side in (-1, 1):   # columns carrying the upper floors over the terrace
        parts.append(_wall('Column', (.3, .3, d.GROUND_FLOOR),
                           (side * (d.HALF - .15), d.FRONT - .15, d.GROUND_FLOOR / 2), 'trim'))
    for level in range(1, floors):
        z = d.floor_z(level)
        parts.append(_wall('Band', (d.WIDTH + .08, .1, .22), (0, d.FRONT + .03, z), 'trim'))
    return parts


def ground():
    terrace = ready(box('Terrace', (d.WIDTH, d.TERRACE + .5, d.PLINTH),
                        (0, d.FRONT - d.TERRACE / 2 + .25, d.PLINTH / 2), 'tile'), 0, 1.2)
    step = ready(box('Step', (d.WIDTH, .3, d.PLINTH / 2), (0, d.FRONT + .65, d.PLINTH / 4), 'concrete'), 0, TILE)
    apron = ready(box('BackApron', (d.WIDTH, 1.2, .08), (0, d.BACK - .6, .04), 'concrete'), 0, TILE)
    gutter = ready(box('Gutter', (d.WIDTH, .3, .02), (0, d.BACK - 1.05, .085), 'steel'), 0, 1)
    return [terrace, step, apron, gutter]


def roof(floors):
    top = d.roof_z(floors)
    parts = [ready(box('RoofSlab', (d.WIDTH + .1, d.DEPTH + .1, .14), (0, 0, top + .07), 'concrete'), 0, TILE)]
    height = d.PARAPET
    for y, size in ((d.FRONT, (d.WIDTH + .1, d.WALL, height)), (d.BACK, (d.WIDTH + .1, d.WALL, height * .5))):
        z = top + .14 + size[2] / 2
        parts.append(_wall('Parapet', size, (0, y - (d.WALL / 2 if y > 0 else -d.WALL / 2), z)))
    for side in (-1, 1):
        parts.append(_wall('SideParapet', (d.WALL, d.DEPTH, height * .5),
                           (side * (d.HALF - d.WALL / 2), 0, top + .14 + height * .25)))
    parts.append(_wall('Cornice', (d.WIDTH + .2, .26, .12), (0, d.FRONT - .02, top + .14 + height + .06), 'trim'))
    return parts
