"""Turns raw parts into game-ready meshes: bevels, smooth shading and metre-scaled UVs."""
import random
import bmesh
import bpy


def bevel(obj, width=.003):
    """Rounds hard edges so they catch light; applied immediately."""
    modifier = obj.modifiers.new('Bevel', 'BEVEL')
    modifier.width, modifier.segments = width, 2
    modifier.limit_method, modifier.angle_limit = 'ANGLE', .7
    normals = obj.modifiers.new('Normals', 'WEIGHTED_NORMAL')
    normals.keep_sharp = True
    graph = bpy.context.evaluated_depsgraph_get()
    mesh = bpy.data.meshes.new_from_object(obj.evaluated_get(graph))
    obj.modifiers.clear()
    obj.data = mesh
    for polygon in mesh.polygons:
        polygon.use_smooth = True
    return obj


def box_uv(obj, metres_per_tile=1.0, grain='x', rng=None):
    """Projects UVs along the dominant local axis so texels stay a constant world size.

    grain names the local axis that the texture's V direction follows on side faces;
    plank textures use it to run the boards along the part.
    """
    rng = rng or random
    shift = (rng.random(), rng.random())
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    layer = bm.loops.layers.uv.verify()
    for face in bm.faces:
        axis = max(range(3), key=lambda i: abs(face.normal[i]))
        first, second = [i for i in range(3) if i != axis]
        if 'xyz'[first] == grain:
            first, second = second, first
        for loop in face.loops:
            co = loop.vert.co
            loop[layer].uv = (co[first] / metres_per_tile + shift[0],
                              co[second] / metres_per_tile + shift[1])
    bm.to_mesh(obj.data)
    bm.free()
    return obj


def ready(obj, width=.003, tile=1.0, grain='x', rng=None):
    return box_uv(bevel(obj, width) if width > 0 else obj, tile, grain, rng)
