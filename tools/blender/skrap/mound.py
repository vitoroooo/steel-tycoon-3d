"""The heap itself: a lumpy mound of compacted scrap that the loose pieces sit on."""
import bmesh
from mathutils import Vector, noise
from kit.finish import ready
from kit.parts import from_bmesh

RADIUS_X, RADIUS_Y, HEIGHT = 5.0, 3.8, 2.8


def height(x, y):
    """Top of the mound over a ground point, zero outside its foot."""
    reach = ((x / RADIUS_X) ** 2 + (y / RADIUS_Y) ** 2) ** .5
    if reach >= 1:
        return 0.0
    lump = noise.noise(Vector((x * .55, y * .55, 3.1))) * .45
    return max(0.0, HEIGHT * (1 - reach ** 1.5) * (1 + lump))


def heap():
    def build(bm):
        bmesh.ops.create_grid(bm, x_segments=36, y_segments=28, size=1)
        for vertex in bm.verts:
            x, y = vertex.co.x * RADIUS_X * 1.05, vertex.co.y * RADIUS_Y * 1.05
            jag = noise.noise(Vector((x * 2.3, y * 2.3, 0))) * .16
            top = height(x, y)
            vertex.co = (x, y, top + (jag if top > .05 else -.03))
    return ready(from_bmesh('Mound', build, 'scrap'), 0, 2.5)
