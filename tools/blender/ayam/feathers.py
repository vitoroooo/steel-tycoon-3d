"""The two shapes a bird is made of: rounded feathered masses and single flat plumes."""
import bmesh
from mathutils import Vector, noise
from kit.parts import from_bmesh

STEPS, SMALL = 5, .05


def egg(name, size, at, mat, lumps=.0, taper=.0, rot=(0, 0, 0)):
    """An ellipsoid; lumps ruffles the surface like plumage, taper narrows the end towards -Y."""
    def build(bm):
        around, down = (12, 8) if max(size) > SMALL else (8, 5)   # eyes and wattles need no more
        bmesh.ops.create_uvsphere(bm, u_segments=around, v_segments=down, radius=.5)
        for vertex in bm.verts:
            co = vertex.co.copy()
            co += co.normalized() * noise.noise(co * 5.1 + Vector(at) * 17) * lumps
            narrow = 1 - taper * (.5 - co.y)
            vertex.co = Vector((co.x * narrow, co.y, co.z * narrow))
        bmesh.ops.scale(bm, vec=size, verts=bm.verts)
    return smooth(from_bmesh(name, build, mat, at, rot))


def plume(name, root, bend, tip, width, thin, mat):
    """One feather: a blade swept from root to tip through bend, flat across the `thin` axis."""
    root, bend, tip, thin = Vector(root), Vector(bend), Vector(tip), Vector(thin).normalized()

    def build(bm):
        rings = []
        for step in range(STEPS + 1):
            t = step / STEPS
            centre = (1 - t) ** 2 * root + 2 * t * (1 - t) * bend + t * t * tip
            tangent = (2 * (1 - t) * (bend - root) + 2 * t * (tip - bend)).normalized()
            across = tangent.cross(thin).normalized()
            half = width * (.35 + 1.3 * t) * (1 - t) ** .6 + .0015   # widest a third of the way, a point at the tip
            offsets = (across * half, thin * half * .16, -across * half, -thin * half * .16)
            rings.append([bm.verts.new(centre + offset) for offset in offsets])
        for lower, upper in zip(rings, rings[1:]):
            for i in range(4):
                bm.faces.new((lower[i], lower[(i + 1) % 4], upper[(i + 1) % 4], upper[i]))
        bm.faces.new(rings[0][::-1])
        bm.faces.new(rings[-1])
    return smooth(from_bmesh(name, build, mat))


def smooth(obj):
    for polygon in obj.data.polygons:
        polygon.use_smooth = True
    return obj
