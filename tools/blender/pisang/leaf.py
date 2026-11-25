"""Banana leaves: a strip folded along its midrib; the texture's alpha cuts the outline and the tears."""
import math
from kit.parts import from_bmesh

SECTIONS, ACROSS = 14, 6
FOLD = .5   # how far the two halves fold up at the stem end, as a fraction of the half width


def leaf(length, width, yaw, lift, droop, at, mat='leaf', twist=0.0):
    """A blade growing along local +Y; lift raises it from the stem, droop bends the tip back down."""
    def point(t, s):
        """Position at t along the midrib and s (-1..1) across the blade."""
        fold = abs(s) * width / 2 * (FOLD * (1 - t) - .25 * t)   # folded up at the stem, hanging at the tip
        x, z, roll = s * width / 2, fold - abs(s) ** 3 * width * .08, twist * t
        return (x * math.cos(roll) - z * math.sin(roll), length * t,
                length * (lift * t - droop * t * t) + x * math.sin(roll) + z * math.cos(roll))

    def build(bm):
        layer = bm.loops.layers.uv.verify()
        for underside in (False, True):   # each side owns its vertices so it gets its own normals
            grid = [[bm.verts.new(point(i / SECTIONS, k / ACROSS * 2 - 1)) for k in range(ACROSS + 1)]
                    for i in range(SECTIONS + 1)]
            for i in range(SECTIONS):
                for k in range(ACROSS):
                    corners = [(i, k), (i, k + 1), (i + 1, k + 1), (i + 1, k)]
                    corners = corners[::-1] if underside else corners
                    face = bm.faces.new([grid[row][col] for row, col in corners])
                    face.smooth = True
                    for loop, (row, col) in zip(face.loops, corners):
                        loop[layer].uv = (col / ACROSS, row / SECTIONS)
    return from_bmesh('Leaf', build, mat, at, (0, 0, yaw))


def crown(top, count, size, start=0.0):
    """Leaves spiralling out of the stem top: young ones upright, old ones hanging."""
    parts = []
    for i in range(count):
        age = i / max(1, count - 1)
        parts.append(leaf(size * (1.0 + .25 * age), size * .36, start + i * 2.4,
                          1.35 - 1.0 * age, .4 + .8 * age, top, twist=.5 * math.sin(i * 1.7)))
    return parts
