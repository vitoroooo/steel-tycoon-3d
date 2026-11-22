"""Brings a finished model down to a triangle budget a phone can draw many times over."""
import bpy


def triangles():
    """Triangles of every mesh as it will be exported, modifiers applied."""
    graph = bpy.context.evaluated_depsgraph_get()
    total = 0
    for obj in bpy.data.objects:
        if obj.type == 'MESH':
            mesh = obj.evaluated_get(graph).to_mesh()
            total += sum(len(polygon.vertices) - 2 for polygon in mesh.polygons)
            obj.evaluated_get(graph).to_mesh_clear()
    return total


def to_budget(budget):
    """Collapses edges evenly over all parts until the model fits; a model already under budget is left alone."""
    now = triangles()
    if now <= budget:
        return
    for obj in bpy.data.objects:
        if obj.type == 'MESH':
            modifier = obj.modifiers.new('Ringan', 'DECIMATE')
            modifier.ratio = budget / now
    print('LIGHTENED from', now, 'to', triangles(), 'triangles')
