"""Assembles the motor triseda: body, a steering assembly and three wheels Unity can spin."""
from kit import scene
from . import bed, body, dims as d, front, props, wheel


def _wheel(name, x, y, parent):
    parts = wheel.wheel()
    for part in parts:
        part.location.x += x
        part.location.y += y
        part.location.z += d.WHEEL_RADIUS
    return scene.pivot(scene.join(name, parts, parent), (x, y, d.WHEEL_RADIUS))


def build():
    root = scene.root('Triseda')
    fixed = (body.frame() + body.tank_and_seat() + body.engine() + body.exhaust() + bed.chassis() +
             bed.bed() + bed.headboard() + props.lights_and_plate() + props.lettering() + props.load())
    scene.join('Body', fixed, root)
    turning = front.fork() + front.headlamp() + front.handlebar()
    scene.pivot(scene.join('Steer', turning, root), (0, d.HEAD_Y, d.HEAD_Z))
    _wheel('Wheel_F', 0, d.FRONT_AXLE_Y, root)   # Unity hangs it under Steer; nested pivots break in FBX
    _wheel('Wheel_RL', -d.REAR_X, d.REAR_AXLE_Y, root)
    _wheel('Wheel_RR', d.REAR_X, d.REAR_AXLE_Y, root)
    return 'Vehicles', 'Triseda'
