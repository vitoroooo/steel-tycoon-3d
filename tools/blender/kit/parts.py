"""Primitive parts. Sizes are metres; every part gets exactly one named material slot."""
import bmesh
import bpy
from mathutils import Vector, noise


def material(name):
    return bpy.data.materials.get(name) or bpy.data.materials.new(name)


def from_bmesh(name, build, mat, at=(0, 0, 0), rot=(0, 0, 0)):
    """Creates an object whose mesh is filled in by build(bm)."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    build(bm)
    bm.to_mesh(mesh)
    bm.free()
    mesh.materials.append(material(mat))
    obj = bpy.data.objects.new(name, mesh)
    obj.location, obj.rotation_euler = at, rot
    bpy.context.collection.objects.link(obj)
    return obj


def box(name, size, at, mat, rot=(0, 0, 0)):
    def build(bm):
        bmesh.ops.create_cube(bm, size=1)
        bmesh.ops.scale(bm, vec=size, verts=bm.verts)
    return from_bmesh(name, build, mat, at, rot)


def cylinder(name, radius, depth, at, mat, rot=(0, 0, 0), segments=24):
    """A capped cylinder along local Z."""
    def build(bm):
        bmesh.ops.create_cone(bm, cap_ends=True, segments=segments,
                              radius1=radius, radius2=radius, depth=depth)
    return from_bmesh(name, build, mat, at, rot)


def ring(name, major, minor, at, mat, rot=(0, 0, 0), flatten=1.0, segments=48):
    """A torus around local Z; flatten < 1 squashes the tube along the axis."""
    bpy.ops.mesh.primitive_torus_add(major_radius=major, minor_radius=minor,
                                     major_segments=segments, minor_segments=12)
    obj = bpy.context.object
    obj.name = name
    for vertex in obj.data.vertices:
        vertex.co.z *= flatten
    obj.data.materials.append(material(mat))
    obj.location, obj.rotation_euler = at, rot
    return obj


def blob(name, size, at, mat, bulge=.35, rot=(0, 0, 0), lumps=0.0, taper=0.0):
    """A soft, rounded box for sacks and bundles.

    lumps displaces the surface with noise (as a fraction of the size) and taper
    narrows the bottom, so cloth reads as hanging rather than as a rigid block.
    """
    def build(bm):
        bmesh.ops.create_cube(bm, size=1)
        bmesh.ops.subdivide_edges(bm, edges=bm.edges, cuts=7, use_grid_fill=True)
        for vertex in bm.verts:
            co = vertex.co.lerp(vertex.co.normalized() * .5, bulge)
            co += co.normalized() * noise.noise(co * 4.3 + Vector(at) * 9) * lumps
            narrow = 1 - taper * (.5 - co.z)
            vertex.co = (co.x * narrow, co.y * narrow, co.z)
        bmesh.ops.scale(bm, vec=size, verts=bm.verts)
    return from_bmesh(name, build, mat, at, rot)
