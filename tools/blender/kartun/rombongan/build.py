"""The pengepul with his cart beside the motor bak, to judge scale and how the theme holds together."""
import bpy
from mathutils import Matrix
from ..gerobak import build as gerobak
from ..pengepul import build as pengepul
from ..triseda import build as triseda

CART_SCALE, BIKE_SCALE = .78, .9    # vehicles are sized to the short, big-headed cast


def _placed(parts, offset, scale):
    bpy.context.view_layer.update()     # lettering sets its own transform; read it before moving
    move = Matrix.Translation(offset) @ Matrix.Scale(scale, 4)
    for part in parts:
        part.matrix_world = move @ part.matrix_world
    return parts


def build():
    return (pengepul.build() + _placed(gerobak.build(), (-1.45, -.15, 0), CART_SCALE) +
            _placed(triseda.build(), (2.0, -.3, 0), BIKE_SCALE))
