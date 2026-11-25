"""Thin curved sheet-metal parts: mudguards and cowls."""
import math
import bmesh
from .parts import from_bmesh


def arc_band(name, radius, width, start, end, at, mat, crown=.02, thickness=.004, steps=20):
    """A strip bent around the X axis through `at`, like a mudguard over a wheel.

    Angles are degrees in the YZ plane: 0 points forward (+Y), 90 straight up.
    crown lowers both edges so the strip wraps over the tyre instead of lying flat.
    """
    across = 6

    def build(bm):
        rows = []
        for step in range(steps + 1):
            angle = math.radians(start + (end - start) * step / steps)
            row = []
            for index in range(across + 1):
                side = index / across * 2 - 1
                reach = radius - crown * side * side
                row.append(bm.verts.new((side * width / 2, math.cos(angle) * reach, math.sin(angle) * reach)))
            rows.append(row)
        for near, far in zip(rows, rows[1:]):
            for index in range(across):
                bm.faces.new((near[index], near[index + 1], far[index + 1], far[index]))
        bmesh.ops.solidify(bm, geom=bm.faces[:], thickness=thickness)
        bmesh.ops.recalc_face_normals(bm, faces=bm.faces[:])
    return from_bmesh(name, build, mat, at)
