"""The street face: roller shutter, awning, sign band, and a balcony with door and louvre windows."""
from kit.finish import ready
from kit.parts import box
from kit.tubes import tube
from . import dims as d

SHOP_Y = d.FRONT - d.TERRACE     # plane of the shopfront
DOOR_W, DOOR_H = 3.7, 2.7


def shutter():
    parts = []
    slat = .15
    for i in range(int(DOOR_H / slat)):   # proud slats read as a rolled steel curtain
        z = d.PLINTH + slat / 2 + i * slat
        parts.append(ready(box('Slat', (DOOR_W, .035, slat - .025), (-.35, SHOP_Y + .03, z), 'shutter'), 0, 1))
    top = d.PLINTH + DOOR_H
    parts.append(ready(box('ShutterBox', (DOOR_W + .2, .28, .34), (-.35, SHOP_Y + .12, top + .17), 'frame'), .01, 1))
    for x in (-.35 - DOOR_W / 2, -.35 + DOOR_W / 2):
        parts.append(ready(box('Guide', (.07, .09, DOOR_H), (x, SHOP_Y + .05, d.PLINTH + DOOR_H / 2), 'frame'), 0, 1))
    parts.append(ready(box('Handle', (.3, .03, .05), (-.35, SHOP_Y + .07, d.PLINTH + .22), 'steel'), 0, 1))
    # the narrow side door every ruko keeps for after hours
    parts.append(ready(box('SideDoor', (.8, .05, 2.1), (1.95, SHOP_Y + .03, d.PLINTH + 1.05), 'door'), .008, 1))
    parts.append(ready(box('SideDoorFrame', (.94, .08, 2.2), (1.95, SHOP_Y + .01, d.PLINTH + 1.1), 'frame'), 0, 1))
    parts.append(ready(box('Vent', (.8, .05, .35), (1.95, SHOP_Y + .03, d.PLINTH + 2.5), 'frame'), 0, 1))
    return parts


def awning():
    z, reach = d.GROUND_FLOOR - .35, 1.5
    sheet = box('Awning', (d.WIDTH - .1, reach, .03), (0, d.FRONT + reach / 2 - .1, z - .17), 'awning', rot=(-.2, 0, 0))
    parts = [ready(sheet, 0, 1, grain='y')]
    for x in (-d.HALF + .3, 0, d.HALF - .3):
        stay = [(x, d.FRONT, z - .75), (x, d.FRONT + reach - .25, z - .3)]
        parts.append(ready(tube('Stay', stay, .014, 'steel', bend=0, sides=6), 0, 1))
    parts.append(ready(box('SignBand', (d.WIDTH - .3, .08, .8), (0, d.FRONT + .06, d.GROUND_FLOOR + .55), 'sign'), .01, 1))
    return parts


def balcony(level):
    z = d.floor_z(level)
    y = d.FRONT + d.BALCONY
    parts = [ready(box('BalconySlab', (d.WIDTH - .2, d.BALCONY, .12), (0, d.FRONT + d.BALCONY / 2, z + .06), 'concrete'), 0, 2)]
    rail = [(-d.HALF + .14, d.FRONT, z + 1.0), (-d.HALF + .14, y - .05, z + 1.0),
            (d.HALF - .14, y - .05, z + 1.0), (d.HALF - .14, d.FRONT, z + 1.0)]
    parts.append(ready(tube('Rail', rail, .022, 'steel', bend=.05, sides=6), 0, 1))
    for i in range(13):
        x = -d.HALF + .14 + i * (d.WIDTH - .28) / 12
        parts.append(ready(box('Baluster', (.02, .02, .86), (x, y - .05, z + .55), 'steel'), 0, 1))
    return parts


def openings(level):
    """A door onto the balcony between two louvre windows."""
    z = d.floor_z(level) + .12
    parts = [ready(box('BalconyDoorFrame', (1.0, .1, 2.2), (0, d.FRONT + .01, z + 1.1), 'frame'), 0, 1),
             ready(box('BalconyDoor', (.86, .05, 2.08), (0, d.FRONT + .05, z + 1.06), 'door'), .008, 1)]
    for side in (-1, 1):
        x = side * 1.55
        parts.append(ready(box('WindowFrame', (1.2, .1, 1.3), (x, d.FRONT + .01, z + 1.55), 'frame'), 0, 1))
        for i in range(7):   # tilted glass louvres, the "nako" windows of older rukos
            pane = box('Louvre', (1.06, .012, .17), (x, d.FRONT + .06, z + 1.02 + i * .17), 'glass', rot=(.5, 0, 0))
            parts.append(ready(pane, 0, 1))
        parts.append(ready(box('Sill', (1.34, .16, .06), (x, d.FRONT + .06, z + .88), 'trim'), 0, 1))
    return parts
