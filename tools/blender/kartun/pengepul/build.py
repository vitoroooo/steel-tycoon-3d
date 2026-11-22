"""Assembles the pengepul. Parts stay separate so the game can pose him without skinning."""
from . import body, gear, head
from .look import Look

PENGEPUL = Look(shirt='orange', long_sleeves=False, trousers='grey', long_legs=False, hands='light', cuff='yellow',
                sandal='navy', strap='yellow', ears=True, top=head.hair, gear=gear.outfit)


def build(look=PENGEPUL):
    parts = head.skull(look.ears) + head.face() + look.top() + body.torso(look) + look.gear()
    for side in (-1, 1):
        parts += body.arm(side, look) + body.leg(side, look)
    return parts
