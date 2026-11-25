"""The melt shop: a crane hall on portal frames, closed at the back and open to the yard on the working side."""
from kit import scene
from kit.finish import ready
from kit.parts import box
from . import cladding, dims as d, frame, roof


def build():
    root = scene.root('Peleburan')
    parts = [ready(box('Slab', (d.WIDTH + 3, d.DEPTH + 3, .24), (0, 0, -.07), 'floor'), .02, 3)]
    parts += frame.portals() + frame.purlins() + frame.crane_runway() + frame.bracing()
    parts += cladding.sides() + cladding.end_wall(-d.HALF_W, door=False) + cladding.end_wall(d.HALF_W, door=True)
    parts += roof.slopes() + roof.monitor() + roof.drainage() + roof.lamps()
    scene.join('Body', parts, root)
    return 'Buildings', 'Peleburan'
