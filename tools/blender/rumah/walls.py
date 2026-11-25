"""Masonry body with its painted dado band, gable ends and plinth."""
import bmesh
from kit.finish import ready
from kit.parts import box, from_bmesh
from . import dims as d

TILE = 2.0


def _gable(x):
    """The triangle of wall under the roof at one end of the house."""
    def build(bm):
        ends = []
        for dx in (-d.WALL / 2, d.WALL / 2):
            ends.append([bm.verts.new((x + dx, y, z)) for y, z in
                         ((d.BACK, d.EAVE), (d.FRONT, d.EAVE), (d.MID, d.RIDGE))])
        bm.faces.new(ends[0])
        bm.faces.new(reversed(ends[1]))
        for i in range(3):
            j = (i + 1) % 3
            bm.faces.new((ends[0][j], ends[0][i], ends[1][i], ends[1][j]))
        bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    return ready(from_bmesh('Gable', build, 'wall'), 0, TILE)


def body():
    depth = d.FRONT - d.BACK
    height = d.EAVE - d.PLINTH
    parts = [
        ready(box('Plinth', (d.WIDTH + .1, depth + .1, d.PLINTH), (0, d.MID, d.PLINTH / 2), 'concrete'), 0, TILE),
        ready(box('Walls', (d.WIDTH, depth, height), (0, d.MID, d.PLINTH + height / 2), 'wall'), 0, TILE),
        # the lower metre is painted darker so splashes and scuffs do not show
        ready(box('Dado', (d.WIDTH + .02, depth + .02, .9), (0, d.MID, d.PLINTH + .45), 'dado'), 0, TILE),
        _gable(-d.HALF + d.WALL / 2), _gable(d.HALF - d.WALL / 2),
    ]
    for x in (-d.HALF + .001, d.HALF - .001):   # a vent high in each gable
        parts.append(ready(box('GableVent', (.06, .6, .35), (x, d.MID, d.EAVE + .55), 'frame'), 0, 1))
    return parts


def side_windows():
    parts = []
    for side in (-1, 1):
        x = side * d.HALF
        parts.append(ready(box('SideFrame', (.08, 1.0, 1.1), (x, d.MID + .6, d.PLINTH + 1.55), 'frame'), 0, 1))
        parts.append(ready(box('SideGlass', (.1, .86, .96), (x, d.MID + .6, d.PLINTH + 1.55), 'glass'), 0, 1))
        for i in range(5):
            parts.append(ready(box('SideBar', (.12, .016, .96), (x, d.MID + .25 + i * .175, d.PLINTH + 1.55),
                                   'steel'), 0, 1))
    return parts
