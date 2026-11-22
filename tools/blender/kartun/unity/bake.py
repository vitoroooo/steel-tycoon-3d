"""Makes every part a plain mesh facing +Y, with its palette colour stored per vertex."""
import math
import bpy
from mathutils import Matrix
from ..paint import PALETTE

TURN = Matrix.Rotation(math.pi, 4, 'Z')     # the cast is modelled facing -Y; the game wants +Y


def turned(point):
    return (-point[0], -point[1], point[2])


def _select(obj):
    bpy.ops.object.select_all(action='DESELECT')
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj


def solid(part):
    """Curves and text become meshes, and the object transform is burnt into the vertices."""
    if part.type != 'MESH':
        _select(part)
        bpy.ops.object.convert(target='MESH')
        part = bpy.context.object
    part.data.transform(TURN @ part.matrix_world)
    part.matrix_world = Matrix.Identity(4)
    return part


def join(name, parts):
    parts = [solid(part) for part in parts]
    _select(parts[0])
    for part in parts:
        part.select_set(True)
    bpy.ops.object.join()
    merged = bpy.context.object
    merged.name = merged.data.name = name
    return merged


def _srgb(material):
    code = PALETTE[material.name.split('.')[0][len('Cel '):]]
    return tuple(int(code[i:i + 2], 16) / 255 for i in (0, 2, 4)) + (1,)


def tint(mesh, shared):
    """One material for the whole model: the colour of each old slot moves into the vertices.

    Values are written as sRGB and exported untouched; the Unity shader linearises them.
    """
    shades = [_srgb(material) for material in mesh.materials]
    layer = mesh.color_attributes.new('Col', 'FLOAT_COLOR', 'CORNER')
    for polygon in mesh.polygons:
        for loop in polygon.loop_indices:
            layer.data[loop].color = shades[polygon.material_index]
        polygon.material_index = 0
    mesh.materials.clear()
    mesh.materials.append(shared)


def pivot(obj, origin):
    """Moves the origin to a joint so the game can rotate the part around it."""
    bpy.context.scene.cursor.location = origin
    _select(obj)
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
