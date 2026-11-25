"""What is left between the columns: brick walls with ragged tops, window holes, a stair and scaffolding."""
from kit.finish import ready
from kit.members import beam
from kit.parts import box
from kit.tubes import tube
from . import dims as d


def ragged_wall(x0, x1, y, level, rng, mat='brick'):
    """A wall panel built from uneven strips, so its top edge reads as broken brickwork."""
    parts, z0 = [], (level - 1) * d.STOREY
    strips = 6
    for k in range(strips):
        width = (x1 - x0) / strips
        height = (d.STOREY - d.BEAM_DEPTH) * rng.uniform(.35, 1) if rng.random() < .7 else d.STOREY - d.BEAM_DEPTH
        parts.append(ready(box('Wall', (width, .15, height), (x0 + width * (k + .5), y, z0 + height / 2), mat), 0, 1.2))
    return parts


def walls(rng):
    parts = []
    for level in range(1, d.FLOORS + 1):
        for i in range(d.BAYS_X):
            if not d.standing(i + 1, level):
                continue
            x0, x1 = d.column_x(i) + d.COLUMN / 2, d.column_x(i + 1) - d.COLUMN / 2
            parts += ragged_wall(x0, x1, d.DEPTH / 2, level, rng, 'plaster' if (i + level) % 2 else 'brick')   # street front
            if level == 1 or i == 0:
                parts += ragged_wall(x0, x1, -d.DEPTH / 2, level, rng)
    return parts


def stair():
    """The stair core at the west end, open to the sky now that its walls are gone."""
    parts, x, run = [], d.column_x(0) + 1.1, 3.6
    for level in range(2):
        z0, steps = level * d.STOREY, 16
        for s in range(steps):
            y = -run / 2 + (s + .5) * run / steps
            parts.append(ready(box('Step', (1.2, run / steps, .1), (x, y if level % 2 == 0 else -y, z0 + (s + 1) * d.STOREY / steps - .05),
                                   'concrete'), .005, 1.5))
    return parts


def scaffold(rng):
    """Bamboo scaffolding against the west gable: how the crew gets up there."""
    parts, x = [], -d.WIDTH / 2 - .7
    for y in (-3.6, -1.2, 1.2, 3.6):
        lean = rng.uniform(-.08, .08)
        parts.append(tube('Pole', [(x, y, 0), (x + lean, y, d.FLOORS * d.STOREY + .8)], .04, 'bamboo', bend=0, sides=7))
    for level in range(1, d.FLOORS + 1):
        z = level * d.STOREY - 1.1
        parts.append(tube('Ledger', [(x, -4, z), (x, 4, z)], .035, 'bamboo', bend=0, sides=7))
        parts.append(ready(box('Plank', (.5, 7.6, .04), (x, 0, z + .06), 'wood'), .004, 1, 'y'))
    parts.append(ready(beam('Brace', (x, -3.6, .3), (x, 3.6, d.FLOORS * d.STOREY - .5), .05, .05, 'bamboo'), 0))
    return parts
