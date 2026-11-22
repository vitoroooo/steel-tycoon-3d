"""One palette and one cel material for the whole game, so every asset shares a theme."""
import bpy

PALETTE = {
    'navy': '1F2F4A', 'yellow': 'F2B632', 'orange': 'E8743B', 'teal': '2E8C86',
    'cream': 'F4EBDD', 'skin': 'C98A5E', 'skin_dark': 'A96F48', 'hair': '23252E',
    'grey': '6E737C', 'light': 'C9CDD2', 'white': 'FAF6EE', 'ink': '1A1C24',
    'red': 'C8402F', 'rust': '9A5230', 'wood': 'B07B4A', 'blue': '3F7FB5',
    'blush': 'E58A6E', 'glass': 'A9D6E5', 'green': '5E9C4B', 'tire': '2B2D33', 'steel': '8D949B',
}


def _linear(channel):
    return channel / 12.92 if channel <= .04045 else ((channel + .055) / 1.055) ** 2.4


def colour(name):
    code = PALETTE[name]
    return tuple(_linear(int(code[i:i + 2], 16) / 255) for i in (0, 2, 4)) + (1,)


def cel(name):
    """Two flat tones with a crisp terminator, no texture: cheap on a phone and easy to read."""
    key = 'Cel ' + name
    if key in bpy.data.materials:
        return bpy.data.materials[key]
    material = bpy.data.materials.new(key)
    material.use_nodes = True
    nodes, links = material.node_tree.nodes, material.node_tree.links
    nodes.clear()
    toon = nodes.new('ShaderNodeBsdfToon')
    toon.inputs['Color'].default_value = colour(name)
    toon.inputs['Size'].default_value, toon.inputs['Smooth'].default_value = .72, .04
    out = nodes.new('ShaderNodeOutputMaterial')
    links.new(toon.outputs[0], out.inputs[0])
    return material


def ink():
    """The outline shell: only its far side is visible, so it reads as a drawn line."""
    if 'Ink' in bpy.data.materials:
        return bpy.data.materials['Ink']
    material = bpy.data.materials.new('Ink')
    material.use_nodes = True
    nodes, links = material.node_tree.nodes, material.node_tree.links
    nodes.clear()
    line = nodes.new('ShaderNodeEmission')
    line.inputs['Color'].default_value = colour('ink')
    clear, mix = nodes.new('ShaderNodeBsdfTransparent'), nodes.new('ShaderNodeMixShader')
    facing, ray = nodes.new('ShaderNodeNewGeometry'), nodes.new('ShaderNodeLightPath')
    out = nodes.new('ShaderNodeOutputMaterial')
    # Drawn only where the camera sees the shell's far side, so the line never shades the model.
    front, seen, hidden = (nodes.new('ShaderNodeMath') for _ in range(3))
    front.operation, seen.operation, hidden.operation = 'SUBTRACT', 'MULTIPLY', 'SUBTRACT'
    front.inputs[0].default_value = hidden.inputs[0].default_value = 1
    links.new(facing.outputs['Backfacing'], front.inputs[1])
    links.new(ray.outputs['Is Camera Ray'], seen.inputs[0])
    links.new(front.outputs[0], seen.inputs[1])
    links.new(seen.outputs[0], hidden.inputs[1])
    links.new(hidden.outputs[0], mix.inputs[0])
    links.new(line.outputs[0], mix.inputs[1])
    links.new(clear.outputs[0], mix.inputs[2])
    links.new(mix.outputs[0], out.inputs[0])
    return material


def outline(obj, width=.012):
    if obj.type != 'MESH':
        return obj
    obj.data.materials.append(ink())
    shell = obj.modifiers.new('Outline', 'SOLIDIFY')
    shell.thickness, shell.offset = width, 1
    shell.use_flip_normals, shell.use_rim = True, False
    shell.material_offset = len(obj.data.materials) - 1
    return obj
