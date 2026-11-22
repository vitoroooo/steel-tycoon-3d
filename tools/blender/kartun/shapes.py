"""Rounded building blocks for the cartoon cast: every form is a soft, readable solid."""
import math
import bmesh
import bpy
from mathutils import Matrix, Vector
from . import detail
from .detail import count, steps


def _object(name, bm, material, smooth=True):
    mesh = bpy.data.meshes.new(name)
    bm.to_mesh(mesh)
    bm.free()
    for polygon in mesh.polygons:
        polygon.use_smooth = smooth
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(obj)
    mesh.materials.append(material)
    return obj


def ball(name, centre, size, material, segments=24):
    """An ellipsoid; size is its three radii."""
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=count(segments), v_segments=count(segments // 2), radius=1)
    bmesh.ops.scale(bm, vec=Vector(size), verts=bm.verts)
    bmesh.ops.translate(bm, vec=Vector(centre), verts=bm.verts)
    return _object(name, bm, material)


def capsule(name, start, end, radius_start, radius_end, material, segments=20):
    """A limb: a sphere pulled apart between two joints, each end with its own radius."""
    start, end = Vector(start), Vector(end)
    axis = end - start
    turn = Vector((0, 0, 1)).rotation_difference(axis.normalized()).to_matrix().to_4x4()
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=count(segments), v_segments=count(10), radius=1)
    for vert in bm.verts:
        upper = vert.co.z > 1e-4
        vert.co = vert.co * (radius_end if upper else radius_start)
        if upper:
            vert.co.z += axis.length
    bmesh.ops.transform(bm, matrix=Matrix.Translation(start) @ turn, verts=bm.verts)
    return _object(name, bm, material)


def block(name, centre, size, material, round_by=.25, turn_z=0):
    """A box whose edges are rounded by a fraction of its smallest side."""
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1)
    bmesh.ops.scale(bm, vec=Vector(size), verts=bm.verts)
    bmesh.ops.bevel(bm, geom=bm.edges[:], offset=min(size) * round_by, segments=steps(4, 2),
                    affect='EDGES', profile=.5)
    bmesh.ops.transform(bm, matrix=Matrix.Translation(Vector(centre)) @
                        Matrix.Rotation(turn_z, 4, 'Z'), verts=bm.verts)
    return _object(name, bm, material)


def tube(name, points, radius, material, closed=False):
    """A round cord through the points: straps, smiles, rims."""
    curve = bpy.data.curves.new(name, 'CURVE')
    thin = detail.LEVEL < 1 and radius <= .016      # a cord this fine reads the same with four sides
    curve.dimensions, curve.bevel_depth = '3D', radius
    curve.bevel_resolution, curve.resolution_u = 0 if thin else steps(4 * detail.LEVEL), steps(12 * detail.LEVEL, 2)
    curve.use_fill_caps = True
    spline = curve.splines.new('NURBS')
    spline.points.add(len(points) - 1)
    for point, co in zip(spline.points, points):
        point.co = (*co, 1)
    spline.use_endpoint_u, spline.use_cyclic_u, spline.order_u = not closed, closed, 3
    obj = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(obj)
    curve.materials.append(material)
    return obj


def arc(centre, radius, start_deg, end_deg, plane='xz', steps=9):
    """Points on a circle segment, for tube()."""
    cx, cy, cz = centre
    points = []
    for i in range(steps):
        angle = math.radians(start_deg + (end_deg - start_deg) * i / (steps - 1))
        a, b = radius * math.cos(angle), radius * math.sin(angle)
        points.append({'xz': (cx + a, cy, cz + b), 'yz': (cx, cy + a, cz + b), 'xy': (cx + a, cy + b, cz)}[plane])
    return points
