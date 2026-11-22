"""The push cart as a body and two wheels that roll on their axle."""
from ..gerobak import box, load
from .rig import Node

TARGET = ('Vehicles', 'GerobakKartun')
WHEEL_PARTS = ('Ban', 'Pelek', 'Naf', 'Jari')
AXLE = (box.WIDTH / 2 + .1, -.08, .33)


def nodes():
    parts = box.walls() + box.running_gear() + load.sacks() + load.finds() + load.scale() + load.sign()
    wheels = [part for part in parts if part.name.split('.')[0] in WHEEL_PARTS]
    body = [part for part in parts if part not in wheels]
    # Modelled facing -Y, so +X is the cart's left.
    return [
        Node('Body', (0, 0, 0), body),
        Node('Wheel_L', AXLE, [w for w in wheels if _mid_x(w) > 0]),
        Node('Wheel_R', (-AXLE[0],) + AXLE[1:], [w for w in wheels if _mid_x(w) < 0]),
    ]


def _mid_x(part):
    return sum(v.co.x for v in part.data.vertices) / len(part.data.vertices)
