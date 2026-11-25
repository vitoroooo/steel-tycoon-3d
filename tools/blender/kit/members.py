"""Structural members placed by their two ends: rafters, braces, crane legs."""
from mathutils import Vector
from .parts import box


def beam(name, start, end, width, depth, mat):
    """A rectangular section running from start to end; depth is measured in the vertical plane."""
    start, end = Vector(start), Vector(end)
    along = end - start
    turn = along.to_track_quat('Y', 'Z').to_euler()
    return box(name, (width, along.length, depth), (start + end) / 2, mat, rot=turn)


def frustum(name, bottom_radius, top_radius, depth, at, mat, segments=24):
    """A capped cone section along Z, for stacks, ladles and hoppers; `at` is its centre."""
    import bmesh
    from .parts import from_bmesh

    def build(bm):
        bmesh.ops.create_cone(bm, cap_ends=True, segments=segments,
                              radius1=bottom_radius, radius2=top_radius, depth=depth)
    return from_bmesh(name, build, mat, at)
