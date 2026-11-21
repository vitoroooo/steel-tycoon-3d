"""Assembles one ruko unit: a static body plus the washing, which hangs free for the wind.

Neighbours butt against its blank side walls. Children named Goyang* are swung by Unity's WindSway.
"""
from kit import scene
from . import back, dims as d, front, services, shell


def assemble(floors, name):
    root = scene.root(name)
    washing = []
    parts = shell.storeys(floors) + shell.ground() + shell.roof(floors)
    parts += front.shutter() + front.awning()
    parts += back.door_and_window() + back.utilities(floors)
    parts += services.water_tank(floors) + services.antenna(floors) + services.downpipes(floors)
    for level in range(1, floors):
        parts += front.balcony(level) + front.openings(level) + back.upper_window(level)
        z = d.floor_z(level)
        parts += services.air_conditioner(1.55 if level % 2 else -1.55, d.FRONT, z + 2.35)
        if level % 2:
            rail, hinge = back.laundry_rail(level)
            parts += rail
            washing.append((back.laundry(level), hinge))
        else:
            parts += services.air_conditioner(1.2, d.BACK, z + .6, facing=-1)
    scene.join('Body', parts, root)
    for cloths, hinge in washing:
        scene.pivot(scene.join('GoyangKain', cloths, root), hinge)
    return 'Buildings', name


def build():
    return assemble(2, 'Ruko')
