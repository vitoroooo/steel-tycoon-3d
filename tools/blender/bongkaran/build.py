"""Proyek Bongkaran: a three-storey concrete building half torn down, its reinforcement there for the taking."""
import random
from kit import scene
from . import frame, infill, rubble


def build():
    rng = random.Random(2024)
    root = scene.root('Bongkaran')
    parts = frame.columns(rng) + frame.beams_and_slabs(rng) + frame.hanging_slab()
    parts += infill.walls(rng) + infill.stair() + infill.scaffold(rng)
    parts += [rubble.mound()] + rubble.debris(rng)
    scene.join('Body', parts, root)
    return 'Buildings', 'Bongkaran'
