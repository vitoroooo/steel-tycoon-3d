"""The melt shop in metres. +Y is the open working side, X runs along the hall, the origin is the footprint centre."""
import math

WIDTH, DEPTH = 30.0, 18.0
HALF_W, HALF_D = WIDTH / 2, DEPTH / 2
EAVES, RIDGE = 11.0, 14.0
BAY = 6.0                              # spacing of the portal frames
FRAMES = [-HALF_W + i * BAY for i in range(6)]
OPEN = 5.6                             # clear height of the open bays on the working side
DADO = 1.2                             # blockwork at the foot of the walls
STRIP = (7.0, 8.2)                     # translucent band that lets daylight into the hall
CRANE_RAIL = 8.4                       # top of the crane runway beams
PITCH = math.atan2(RIDGE - EAVES, HALF_D)
SLOPE = math.hypot(RIDGE - EAVES, HALF_D)
MONITOR_HALF, MONITOR_RISE = 1.6, 1.3  # the raised vent along the ridge


def roof_z(y):
    """Height of the roof plane above a point y metres from the ridge line."""
    return RIDGE - abs(y) * (RIDGE - EAVES) / HALF_D
