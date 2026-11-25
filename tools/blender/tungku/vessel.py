"""The electric arc furnace: tilting shell, lifted roof, three graphite electrodes and their mast."""
import math
from kit.finish import ready
from kit.members import frustum
from kit.parts import box, cylinder, ring
from kit.tubes import tube

RADIUS, RIM = 2.4, 3.5          # shell radius and the height of its lip
GAP = .55                       # the roof hangs this far above the lip, so the bath shows


def shell():
    parts = [ready(frustum('Hearth', 1.7, RADIUS, .8, (0, 0, 1.1), 'shell'), .03, 1.5, 'z'),
             ready(cylinder('Shell', RADIUS, RIM - 1.5, (0, 0, (RIM + 1.5) / 2), 'shell', segments=32), .03, 1.5, 'z'),
             ring('Lip', RADIUS - .12, .2, (0, 0, RIM), 'refractory', flatten=.7),
             cylinder('Bath', RADIUS - .25, .06, (0, 0, RIM - .12), 'melt', segments=32)]
    for y in (-1.5, 1.5):       # rockers the shell tilts on when it is tapped
        parts.append(ready(box('Rocker', (4.6, .35, .7), (0, y, .42), 'steel_dark'), .03))
    for z in (1.9, 2.6, 3.2):   # water-cooled panels are fed by ring mains
        parts.append(ring('CoolingMain', RADIUS + .08, .05, (0, 0, z), 'pipe'))
    spout = box('Spout', (1.5, .6, .3), (RADIUS + .5, 0, 2.55), 'shell', rot=(0, .3, 0))
    parts.append(ready(spout, .03))
    parts.append(ready(box('SpoutLining', (1.4, .36, .08), (RADIUS + .5, 0, 2.72), 'melt', rot=(0, .3, 0)), 0))
    return parts


def roof():
    z = RIM + GAP
    parts = [ready(frustum('Roof', RADIUS + .05, 1.5, .6, (0, 0, z + .3), 'shell', 32), .03, 1.5, 'z'),
             ring('RoofRing', RADIUS + .05, .09, (0, 0, z), 'pipe')]
    for index in range(3):
        angle = math.radians(90 + index * 120)
        x, y = math.cos(angle) * .75, math.sin(angle) * .75
        parts.append(cylinder('Electrode', .2, 4.6, (x, y, z + 1.9), 'graphite', segments=16))
        parts.append(cylinder('ElectrodeTip', .19, .5, (x, y, RIM - .1), 'melt', segments=16))
        parts.append(ready(box('Clamp', (.6, .6, .3), (x, y, z + 3.6), 'steel_dark'), .03))
        arm = box('Arm', (.26, 4.4 + y, .32), (x, (y - 4.4) / 2, z + 3.6), 'shell')
        parts.append(ready(arm, .02, 1.5, 'y'))
        sag = [(x, -4.5, z + 3.7), (x * 2.2, -6.2, z + 2.2), (x * 2.2, -7.6, z + 3.4)]
        parts.append(tube('PowerCable', sag, .09, 'cable', .9, 8))
    parts.append(ready(box('Mast', (1.9, .9, z + 4.2), (0, -4.5, (z + 4.2) / 2), 'steel_dark'), .03, 1.5, 'z'))
    return parts


def fume_duct():
    """Draws the fumes off the roof and out through the back wall towards the stack."""
    run = [(1.3, -.9, RIM + GAP + .5), (1.3, -.9, 8.4), (1.3, -9.4, 9.6)]
    return [tube('FumeDuct', run, .55, 'duct', 1.4, 16),
            ring('DuctFlange', .58, .06, (1.3, -.9, 6.2), 'steel_dark')]
