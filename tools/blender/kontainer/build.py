"""A 20 ft shipping container; doors closed. The paint colour and line name are chosen in Unity."""
from kit import scene
from . import doors, shell


def build():
    root = scene.root('Kontainer')
    parts = shell.frame() + shell.panels() + doors.leaves() + doors.rods() + doors.plate()
    scene.join('Body', parts, root)
    return 'Props', 'Kontainer'
