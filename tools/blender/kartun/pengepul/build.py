"""Assembles the pengepul. Parts stay separate so the game can pose him without skinning."""
from . import body, gear, head


def build():
    parts = head.skull() + head.face() + head.hair() + body.torso() + gear.towel() + gear.sling_bag()
    for side in (-1, 1):
        parts += body.arm(side) + body.leg(side)
    return parts
