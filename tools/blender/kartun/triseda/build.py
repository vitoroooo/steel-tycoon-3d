"""Assembles the 300 kg motor bak (triseda)."""
from . import bed, front


def build():
    return front.steering() + front.frame() + bed.box() + bed.haul()
