"""Bent pipes: a polyline with rounded corners swept by a circle."""
import bpy
from mathutils import Vector
from .parts import material


def fillet(points, radius, steps=6):
    """Replaces every inner corner of the polyline with an arc of the given radius."""
    points = [Vector(p) for p in points]
    result = [points[0]]
    for before, corner, after in zip(points, points[1:], points[2:]):
        to_before = (before - corner).normalized()
        to_after = (after - corner).normalized()
        reach = min(radius, (before - corner).length / 2, (after - corner).length / 2)
        start, end = corner + to_before * reach, corner + to_after * reach
        for step in range(steps + 1):
            t = step / steps
            # Quadratic Bezier through the corner: close to a circular arc for these angles.
            result.append(start.lerp(corner, t).lerp(corner.lerp(end, t), t))
    result.append(points[-1])
    return result


def tube(name, points, radius, mat, bend=.06, sides=10):
    """A capped pipe following the points, converted to a mesh object."""
    curve = bpy.data.curves.new(name, 'CURVE')
    curve.dimensions = '3D'
    curve.bevel_depth = radius
    curve.bevel_resolution = max(1, sides // 4)
    curve.use_fill_caps = True
    path = fillet(points, bend) if bend > 0 else [Vector(p) for p in points]
    spline = curve.splines.new('POLY')
    spline.points.add(len(path) - 1)
    for point, position in zip(spline.points, path):
        point.co = (*position, 1)
    source = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(source)
    graph = bpy.context.evaluated_depsgraph_get()
    mesh = bpy.data.meshes.new_from_object(source.evaluated_get(graph))
    bpy.data.objects.remove(source)
    mesh.materials.append(material(mat))
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(obj)
    return obj


def text(name, body, height, at, mat, rot=(0, 0, 0), depth=.002):
    """Raised lettering, centred on its origin, converted to a mesh object."""
    curve = bpy.data.curves.new(name, 'FONT')
    curve.body, curve.size, curve.extrude = body, height, depth
    curve.align_x, curve.align_y = 'CENTER', 'CENTER'
    source = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(source)
    graph = bpy.context.evaluated_depsgraph_get()
    mesh = bpy.data.meshes.new_from_object(source.evaluated_get(graph))
    bpy.data.objects.remove(source)
    mesh.materials.append(material(mat))
    obj = bpy.data.objects.new(name, mesh)
    obj.location, obj.rotation_euler = at, rot
    bpy.context.collection.objects.link(obj)
    return obj
