"""Assembles the kampung house: a static body plus the washing, which Unity's WindSway swings (Goyang*)."""
from kit import scene
from . import back, front, roof, walls


def build():
    root = scene.root('Rumah')
    parts = walls.body() + walls.side_windows()
    parts += roof.main() + roof.porch() + roof.dish()
    parts += front.porch_floor() + front.door() + front.window() + front.bench() + front.bird_cage()
    parts += back.kitchen() + back.door() + back.yard() + back.washing_line()
    scene.join('Body', parts, root)
    scene.pivot(scene.join('GoyangKain', back.washing(), root), (0, back.LINE_Y, back.LINE_Z))
    return 'Buildings', 'Rumah'
