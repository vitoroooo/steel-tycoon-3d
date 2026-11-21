"""Assembles the gerobak: a static body plus two wheels Unity can spin."""
from kit import scene
from . import box, dims as d, handles, props, wheel


def build():
    root = scene.root('Gerobak')
    body = (box.frame() + box.planks() + handles.push_bar() + handles.legs() +
            handles.axle() + props.sack() + props.scale() + props.sign())
    scene.join('Body', body, root)
    for name, side in (('Wheel_L', -1), ('Wheel_R', 1)):
        parts = wheel.wheel()
        for part in parts:
            part.location.x += side * d.WHEEL_X
            part.location.z += d.WHEEL_RADIUS
        merged = scene.join(name, parts, root)
        scene.pivot(merged, (side * d.WHEEL_X, 0, d.WHEEL_RADIUS))
    return 'Vehicles', 'Gerobak'
