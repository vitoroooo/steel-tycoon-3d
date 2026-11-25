"""The steel tub of the engkel and the tronton, heaped with loose scrap."""
import math
from .. import shapes
from ..paint import cel
from ..rods import lettering, rod
from . import spec

W = 2.36
HEAP = ('rust', 'steel', 'grey', 'rust', 'light', 'steel')


def tub(truck):
    front, back, floor = spec.cab_back(truck) + .3, truck.length / 2, spec.FRAME_TOP + .08
    length, mid_y, mid_z = back - front, (front + back) / 2, floor + truck.wall / 2
    parts = [
        shapes.block('Lantai bak', (0, mid_y, floor), (W, length, .12), cel('navy'), .2),
        shapes.block('Dinding depan', (0, front, mid_z + .2), (W, .08, truck.wall + .4), cel('teal'), .2),
        shapes.block('Pintu bak', (0, back, mid_z), (W, .08, truck.wall), cel('teal'), .2),
        shapes.block('Strip pintu', (0, back + .045, mid_z), (W - .3, .02, .16), cel('cream'), .3),
    ]
    for side in (-1, 1):
        x = side * W / 2
        parts += [
            shapes.block('Dinding bak', (x, mid_y, mid_z), (.08, length, truck.wall), cel('teal'), .2),
            shapes.block('Rel atas', (x, mid_y, floor + truck.wall), (.12, length + .1, .08), cel('navy'), .4),
            shapes.block('Strip bak', (x + side * .045, mid_y, mid_z + .1), (.02, length - .5, .34), cel('cream'), .3),
            shapes.block('Tambalan', (x + side * .05, mid_y + length * .36, mid_z - .28), (.03, .5, .22), cel('rust'), .2),
            lettering('Nama usaha', 'UD. ROSOK JAYA', (x + side * .1, mid_y, mid_z + .02), .26, cel('navy'),
                      'x' if side > 0 else '-x'),
        ]
        posts = max(3, int(length / .95))
        for i in range(posts + 1):
            parts.append(shapes.block('Tiang bak', (x + side * .03, front + .12 + i * (length - .24) / posts, mid_z),
                                      (.07, .1, truck.wall), cel('navy'), .3))
    return parts + heap(front + .3, back - .3, floor + truck.wall)


def heap(front, back, rim):
    """Loose scrap showing over the rim: plates, lumps and a few pipes, placed by a fixed rhythm."""
    parts, count = [], int((back - front) / .55)
    for i in range(count):
        y = front + (i + .5) * (back - front) / count
        x, lift = math.sin(i * 2.3) * .55, .12 + .22 * math.sin(i * .9 + 1) ** 2
        size = (.7 + .3 * math.cos(i * 1.7), .6, .3 + .12 * math.sin(i * 3.1))
        parts.append(shapes.block('Rongsok', (x, y, rim - .1 + lift), size, cel(HEAP[i % len(HEAP)]), .18, i * .7))
        if i % 3 == 1:
            parts.append(rod('Pipa bekas', (-.7, y - .5, rim + lift), (.6, y + .5, rim + lift + .18), .06, cel('steel'), 10))
        if i % 4 == 2:
            parts.append(shapes.ball('Gumpalan', (-x, y, rim + .05), (.32, .3, .22), cel('rust'), 14))
    return parts
