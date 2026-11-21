"""An electric arc furnace with its ladle, fume duct and operating deck; +Y is where the crew stands back."""
from kit import scene
from . import floor, vessel


def build():
    root = scene.root('Tungku')
    parts = vessel.shell() + vessel.roof() + vessel.fume_duct()
    parts += floor.ladle() + floor.deck() + floor.pulpit()
    scene.join('Body', parts, root)
    return 'Props', 'Tungku'
