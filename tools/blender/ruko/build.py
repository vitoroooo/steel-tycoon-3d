"""Assembles one ruko unit as a single static mesh; neighbours butt against its blank side walls."""
from kit import scene
from . import back, dims as d, front, services, shell


def assemble(floors, name):
    root = scene.root(name)
    parts = shell.storeys(floors) + shell.ground() + shell.roof(floors)
    parts += front.shutter() + front.awning()
    parts += back.door_and_window() + back.utilities(floors)
    parts += services.water_tank(floors) + services.antenna(floors) + services.downpipes(floors)
    for level in range(1, floors):
        parts += front.balcony(level) + front.openings(level) + back.upper_window(level)
        z = d.floor_z(level)
        parts += services.air_conditioner(1.55 if level % 2 else -1.55, d.FRONT, z + 2.35)
        if level % 2:
            parts += back.laundry(level)
        else:
            parts += services.air_conditioner(1.2, d.BACK, z + .6, facing=-1)
    scene.join('Body', parts, root)
    return 'Buildings', name


def build():
    return assemble(2, 'Ruko')
