"""The wreckage at the east end: a mound of broken concrete with slabs, blocks and twisted bars on it."""
import bmesh
from mathutils import Vector, noise
from kit.finish import ready
from kit.parts import box, from_bmesh
from kit.tubes import tube
from . import dims as d

CENTRE_X, RADIUS_X, RADIUS_Y, HEIGHT = d.WIDTH / 2 - 1.5, 5.5, 6.0, 2.2


def height(x, y):
    reach = (((x - CENTRE_X) / RADIUS_X) ** 2 + (y / RADIUS_Y) ** 2) ** .5
    if reach >= 1:
        return 0.0
    return max(0.0, HEIGHT * (1 - reach ** 1.4) * (1 + noise.noise(Vector((x * .5, y * .5, 7.7))) * .5))


def mound():
    def build(bm):
        bmesh.ops.create_grid(bm, x_segments=30, y_segments=30, size=1)
        for vertex in bm.verts:
            x, y = CENTRE_X + vertex.co.x * RADIUS_X * 1.05, vertex.co.y * RADIUS_Y * 1.05
            top = height(x, y)
            jag = noise.noise(Vector((x * 2.6, y * 2.6, 1.3))) * .14
            vertex.co = (x, y, top + (jag if top > .05 else -.03))
        flat = [face for face in bm.faces if all(v.co.z < 0 for v in face.verts)]
        bmesh.ops.delete(bm, geom=flat, context='FACES')   # no skirt: the mound ends where it meets the ground
    return ready(from_bmesh('Rubble', build, 'rubble'), 0, 2)


def debris(rng):
    parts = []
    for _ in range(46):
        x, y = CENTRE_X + rng.uniform(-4.6, 4.6), rng.uniform(-5, 5)
        z = height(x, y)
        if z <= 0 and rng.random() < .6:
            continue
        tilt = (rng.uniform(-.7, .7), rng.uniform(-.7, .7), rng.uniform(0, 3.1))
        if rng.random() < .3:      # a piece of floor plate
            size = (rng.uniform(1, 2.4), rng.uniform(.8, 1.8), d.SLAB)
            parts.append(ready(box('BrokenSlab', size, (x, y, z + .15), 'floor', rot=tilt), .008, 2))
        elif rng.random() < .6:    # a chunk of beam or column
            size = (rng.uniform(.3, .5), rng.uniform(.3, .5), rng.uniform(.5, 1.6))
            parts.append(ready(box('Chunk', size, (x, y, z + .2), 'concrete', rot=tilt), .015, 1.5))
        else:                      # bars pulled out of the concrete: the scrap worth coming here for
            a, b = rng.uniform(-1.4, 1.4), rng.uniform(-1.4, 1.4)
            points = [(x, y, z + .05), (x + a * .5, y + b * .3, z + rng.uniform(.3, .9)), (x + a, y + b, z + rng.uniform(.1, .5))]
            parts.append(tube('TwistedBar', points, .014, 'rebar', bend=.3, sides=5))
    return parts
