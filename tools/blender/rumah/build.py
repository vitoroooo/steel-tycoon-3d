"""Assembles the kampung house as a single static mesh; Unity paints each instance its own colours."""
from kit import scene
from . import back, front, roof, walls


def build():
    root = scene.root('Rumah')
    parts = walls.body() + walls.side_windows()
    parts += roof.main() + roof.porch() + roof.dish()
    parts += front.porch_floor() + front.door() + front.window() + front.bench() + front.bird_cage()
    parts += back.kitchen() + back.door() + back.yard() + back.washing_line()
    scene.join('Body', parts, root)
    return 'Buildings', 'Rumah'
