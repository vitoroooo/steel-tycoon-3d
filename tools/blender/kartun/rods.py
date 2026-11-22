"""Straight-sided parts: rods, drums, wheels and lettering."""
import math
import bmesh
import bpy
from mathutils import Matrix, Vector
from .shapes import _object
from .paint import cel
from . import detail
from .detail import count, steps


def rod(name, start, end, radius, material, segments=14, cap_round=.3):
    """A cylinder between two points with softly rounded ends."""
    start, end = Vector(start), Vector(end)
    axis = end - start
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, segments=count(segments), radius1=radius, radius2=radius,
                          depth=axis.length)
    rims = [e for e in bm.edges if abs(e.verts[0].co.z - e.verts[1].co.z) < 1e-6]
    bmesh.ops.bevel(bm, geom=rims, offset=radius * cap_round, segments=steps(3), affect='EDGES')
    turn = Vector((0, 0, 1)).rotation_difference(axis.normalized()).to_matrix().to_4x4()
    bmesh.ops.transform(bm, matrix=Matrix.Translation((start + end) / 2) @ turn, verts=bm.verts)
    return _object(name, bm, material)


def wheel(centre, radius, width, hub='yellow', spokes=0):
    """A fat tyre on the X axle with a coloured hub; spokes > 0 gives a cart wheel."""
    x, y, z = centre
    half = width / 2
    parts = [rod('Ban', (x - half, y, z), (x + half, y, z), radius, cel('tire'), 28, .55),
             rod('Pelek', (x - half - .012, y, z), (x + half + .012, y, z), radius * .62, cel(hub), 24, .2),
             rod('Naf', (x - half - .03, y, z), (x + half + .03, y, z), radius * .2, cel('navy'), 14, .4)]
    for i in range(spokes):
        angle = math.pi * i / spokes
        dy, dz = math.cos(angle) * radius * .56, math.sin(angle) * radius * .56
        for sx in (-1, 1):
            edge = x + sx * (half + .016)
            parts.append(rod('Jari', (edge, y - dy, z - dz), (edge, y + dy, z + dz), radius * .05, cel('navy'), 8))
    return parts


def lettering(name, text, centre, height, material, facing='-y'):
    """Extruded sign text; facing is the direction the letters look towards."""
    curve = bpy.data.curves.new(name, 'FONT')
    curve.body, curve.size, curve.extrude = text, height, height * .08
    curve.resolution_u = steps(12 * detail.LEVEL, 2)
    curve.align_x, curve.align_y = 'CENTER', 'CENTER'
    curve.font = bpy.data.fonts.load('/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf', check_existing=True)
    obj = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(obj)
    curve.materials.append(material)
    yaw = {'-y': 0, 'y': math.pi, 'x': math.pi / 2, '-x': -math.pi / 2}[facing]
    obj.rotation_euler = (math.pi / 2, 0, yaw)
    obj.location = centre
    return obj
