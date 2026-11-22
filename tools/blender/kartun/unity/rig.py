"""A model as a tree of rigid parts: each node is one mesh that turns around its joint."""
from collections import namedtuple
import bpy
from . import bake

# joint is given in modelling space (facing -Y). Nodes are exported side by side under the root:
# the FBX space bake only survives one level, so Unity rebuilds the limb tree (ToonTree.cs).
Node = namedtuple('Node', 'name joint parts')


def named(parts, *names):
    """The parts whose base name (before Blender's .001 suffix) is one of names."""
    return [part for part in parts if part.name.split('.')[0] in names]


def assemble(root_name, nodes):
    bpy.context.view_layer.update()
    shared = bpy.data.materials.new('Kartun')
    root = bpy.data.objects.new(root_name, None)
    bpy.context.collection.objects.link(root)
    for node in nodes:
        obj = bake.join(node.name, node.parts)
        bake.tint(obj.data, shared)
        joint = bake.turned(node.joint)
        bake.pivot(obj, joint)
        obj.parent, obj.location = root, joint
    return root


def export(path):
    """Blender +Y (forward) and +Z (up) become Unity +Z and +Y, in metres, colours untouched."""
    bpy.ops.export_scene.fbx(
        filepath=str(path), object_types={'MESH', 'EMPTY'}, use_mesh_modifiers=True,
        mesh_smooth_type='FACE', add_leaf_bones=False, bake_anim=False, colors_type='LINEAR',
        apply_scale_options='FBX_SCALE_UNITS', bake_space_transform=True,
        axis_forward='Z', axis_up='Y')
    triangles = sum(len(p.vertices) - 2 for o in bpy.data.objects if o.type == 'MESH'
                    for p in o.data.polygons)
    print('EXPORTED', path, 'triangles:', triangles)
