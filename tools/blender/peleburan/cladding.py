"""Walls: blockwork at the foot, corrugated sheet above, a translucent band, and the gable ends."""
import bmesh
from kit.finish import ready
from kit.parts import box, from_bmesh
from . import dims as d

SKIN = .05


def band(name, x, width, y, low, high, mat):
    """One horizontal run of wall along X between two heights."""
    return ready(box(name, (width, SKIN, high - low), (x, y, (low + high) / 2), mat), 0, 2, 'z')


def long_wall(y, x, width):
    """A fully closed stretch of side wall centred on x."""
    low, high = d.STRIP
    return [ready(box('Dado', (width, .22, d.DADO), (x, y, d.DADO / 2), 'wall'), .01, 2),
            band('Sheet', x, width, y, d.DADO, low, 'seng'), band('Daylight', x, width, y, low, high, 'skylight'),
            band('Sheet', x, width, y, high, d.EAVES, 'seng')]


def sides():
    """The back is closed; the working side keeps its three middle bays open below the crane."""
    parts = long_wall(-d.HALF_D, 0, d.WIDTH)
    for x in (d.FRAMES[0] + d.BAY / 2, d.FRAMES[4] + d.BAY / 2):
        parts += long_wall(d.HALF_D, x, d.BAY)
    parts.append(band('Sheet', 0, d.BAY * 3, d.HALF_D, d.OPEN, d.EAVES, 'seng_rust'))
    parts.append(ready(box('Lintel', (d.BAY * 3, .3, .4), (0, d.HALF_D - .1, d.OPEN - .2), 'frame'), .01, 2))
    parts.append(ready(box('SignBoard', (15, .12, 1.7), (0, d.HALF_D + .1, 8.6), 'sign'), .01))
    return parts


def pediment(x, mat):
    """The triangle of wall under the roof at one end of the hall."""
    def build(bm):
        for offset in (-SKIN / 2, SKIN / 2):
            bm.verts.new((offset, -d.HALF_D, d.EAVES)); bm.verts.new((offset, d.HALF_D, d.EAVES))
            bm.verts.new((offset, 0, d.RIDGE))
        v = list(bm.verts)
        bm.faces.new(v[0:3]); bm.faces.new(v[3:6])
        for a, b in ((0, 1), (1, 2), (2, 0)):
            bm.faces.new((v[a], v[a + 3], v[b + 3], v[b]))
        bmesh.ops.recalc_face_normals(bm, faces=bm.faces[:])
    return ready(from_bmesh('Pediment', build, mat, (x, 0, 0)), 0, 2, 'z')


def end_wall(x, door):
    """A gable end; with a door it leaves a 6 m truck opening and hangs a half-open sliding leaf."""
    def panel(name, y, depth, low, high, mat):
        return ready(box(name, (SKIN, depth, high - low), (x, y, (low + high) / 2), mat), 0, 2, 'z')
    parts = [pediment(x, 'seng'), ready(box('Louvre', (.12, 5, 1.6), (x, 0, d.EAVES + .9), 'louvre'), .01, .25, 'y')]
    if not door:
        return parts + [ready(box('Dado', (.22, d.DEPTH, d.DADO), (x, 0, d.DADO / 2), 'wall'), .01, 2),
                        panel('Sheet', 0, d.DEPTH, d.DADO, d.EAVES, 'seng')]
    for side in (-1, 1):
        parts.append(ready(box('Dado', (.22, 6, d.DADO), (x, side * 6, d.DADO / 2), 'wall'), .01, 2))
        parts.append(panel('Sheet', side * 6, 6, d.DADO, d.EAVES, 'seng'))
    out = .16 if x > 0 else -.16
    parts += [panel('Sheet', 0, 6, 6.2, d.EAVES, 'seng_rust'),
              ready(box('DoorLeaf', (.08, 3.4, 6.1), (x + out, 4.4, 3.1), 'door'), .01, 2, 'z'),
              ready(box('DoorTrack', (.1, 13, .14), (x + out, 1.5, 6.3), 'steel_dark'), .006)]
    return parts
