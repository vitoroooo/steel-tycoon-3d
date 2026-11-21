"""One 2.4 m fence panel: three second-hand sheets of different heights nailed to a timber frame."""
from kit import scene
from kit.finish import ready
from kit.parts import box

WIDTH, HEIGHT = 2.4, 1.9
SHEETS = ((1.9, 'seng', .01), (1.72, 'seng_rust', -.015), (1.84, 'seng', .02))   # height, slot, lean


def build():
    root = scene.root('PagarSeng')
    parts = []
    for x in (-WIDTH / 2, WIDTH / 2):
        parts.append(ready(box('Post', (.08, .08, HEIGHT + .1), (x, -.05, (HEIGHT + .1) / 2), 'wood'), 0, 1, grain='z'))
    for z in (.45, 1.45):
        parts.append(ready(box('Rail', (WIDTH, .04, .08), (0, -.03, z), 'wood'), 0, 1))
    sheet_width = WIDTH / len(SHEETS)
    for i, (height, slot, lean) in enumerate(SHEETS):
        x = -WIDTH / 2 + sheet_width * (i + .5)
        sheet = box('Sheet', (sheet_width + .04, .012, height), (x, .005 * (i % 2), height / 2 + .03), slot,
                    rot=(0, lean, 0))
        parts.append(ready(sheet, 0, 1, grain='z'))
    parts.append(ready(box('Patch', (.5, .012, .6), (.35, .02, .9), 'seng_rust', rot=(0, .08, 0)), 0, 1, grain='z'))
    scene.join('Body', parts, root)
    return 'Buildings', 'PagarSeng'
