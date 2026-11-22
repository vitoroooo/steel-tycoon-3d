"""The pikap as a body and four rolling wheels; the front pair also steers in Unity."""
from ..pikap import bed, cab, dims
from .rig import Node

TARGET = ('Vehicles', 'PikapKartun')
NAMES = ('Wheel_FL', 'Wheel_FR', 'Wheel_RL', 'Wheel_RR')


def nodes():
    axles = [(side * dims.TRACK, y, dims.WHEEL) for y in (dims.FRONT_Y, dims.REAR_Y) for side in (1, -1)]
    rolling = [Node(name, axle, parts) for name, axle, parts in zip(NAMES, axles, bed.wheels())]
    return [Node('Body', (0, 0, 0), cab.shell() + bed.box())] + rolling
