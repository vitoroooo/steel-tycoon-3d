"""The motor bak as a body, a steering head and three rolling wheels."""
from ..triseda import bed, dims, front
from .rig import Node

TARGET = ('Vehicles', 'TrisedaKartun')
WHEEL_PARTS = ('Ban', 'Pelek', 'Naf')
FRONT_AXLE = (0, dims.FRONT_Y, dims.WHEEL)


def _mid_x(part):
    return sum(v.co.x for v in part.data.vertices) / len(part.data.vertices)


def _split(parts):
    wheels = [part for part in parts if part.name.split('.')[0] in WHEEL_PARTS]
    return wheels, [part for part in parts if part not in wheels]


def nodes():
    front_wheel, steering = _split(front.steering())
    rear_wheels, box = _split(bed.box())
    rear = (dims.TRACK, dims.REAR_Y, dims.WHEEL)
    # Modelled facing -Y, so +X is the rider's left.
    return [
        Node('Body', (0, 0, 0), front.frame() + box + bed.haul()),
        Node('Steer', FRONT_AXLE, steering),
        Node('Wheel_F', FRONT_AXLE, front_wheel),
        Node('Wheel_RL', rear, [w for w in rear_wheels if _mid_x(w) > 0]),
        Node('Wheel_RR', (-rear[0],) + rear[1:], [w for w in rear_wheels if _mid_x(w) < 0]),
    ]
