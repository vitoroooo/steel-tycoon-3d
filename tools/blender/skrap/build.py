"""A 10 m heap of mixed scrap for the smelter's stock bays."""
import random
from kit import scene
from . import mound, pieces


def build():
    root = scene.root('TumpukanSkrap')
    scene.join('Body', [mound.heap()] + pieces.scatter(random.Random(1977)), root)
    return 'Props', 'TumpukanSkrap'
