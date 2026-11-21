"""Scene lifecycle: start empty, group parts, export one FBX for Unity."""
import bpy


def reset():
    bpy.ops.wm.read_factory_settings(use_empty=True)


def join(name, parts, parent=None):
    """Merges the parts into one object, keeping their material slots."""
    bpy.ops.object.select_all(action='DESELECT')
    for part in parts:
        part.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    merged = bpy.context.object
    merged.name = merged.data.name = name
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    merged.parent = parent
    return merged


def pivot(obj, origin):
    """Moves the object's origin, so Unity can rotate it around that point."""
    bpy.context.scene.cursor.location = origin
    bpy.ops.object.select_all(action='DESELECT')
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    return obj


def root(name):
    empty = bpy.data.objects.new(name, None)
    bpy.context.collection.objects.link(empty)
    return empty


def export(path):
    """Blender +Y (forward) and +Z (up) become Unity +Z and +Y, in metres."""
    bpy.ops.export_scene.fbx(
        filepath=str(path), object_types={'MESH', 'EMPTY'}, use_mesh_modifiers=True,
        mesh_smooth_type='FACE', add_leaf_bones=False, bake_anim=False,
        apply_scale_options='FBX_SCALE_UNITS', bake_space_transform=True,
        axis_forward='Z', axis_up='Y')
    triangles = sum(len(p.vertices) - 2 for o in bpy.data.objects if o.type == 'MESH'
                    for p in o.data.polygons)
    print('EXPORTED', path, 'triangles:', triangles)

