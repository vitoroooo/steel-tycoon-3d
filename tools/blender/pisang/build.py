"""A mature banana plant with a hanging bunch, a dead leaf and a sucker growing beside it."""
import math
from kit import scene
from kit.finish import ready
import bmesh
from kit.parts import blob, cylinder, from_bmesh
from kit.tubes import tube
from . import leaf

HEIGHT = 2.3


def stem(x, y, height, radius):
    """The pseudostem: a leaning taper, with the flare of old leaf sheaths at its foot."""
    def taper(bottom, top, depth):
        def build(bm):
            bmesh.ops.create_cone(bm, cap_ends=True, segments=16, radius1=bottom, radius2=top, depth=depth)
        return build
    lean = (.0, .14, .5)   # tilts the top towards where the crown and the bunch hang
    trunk = from_bmesh('Stem', taper(radius * 1.25, radius * .8, height), 'stem', (x, y, 0), lean)
    foot = from_bmesh('Foot', taper(radius * 1.7, radius * 1.2, height * .16), 'stem', (x, y, 0), lean)
    for part, depth in ((trunk, height), (foot, height * .16)):
        for vertex in part.data.vertices:
            vertex.co.z += depth / 2   # stand both on the ground so the lean pivots at the root
    return [ready(trunk, .01, .5, 'z'), ready(foot, .01, .5, 'z')]


def bunch():
    """The fruit stalk bends out and down; green hands of bananas, then the purple heart."""
    stalk = [(.32, .19, HEIGHT - .1), (.6, .05, HEIGHT + .05), (.85, -.05, HEIGHT - .35), (.88, -.06, HEIGHT - 1.15)]
    parts = [ready(tube('Stalk', stalk, .025, 'stem', bend=.15, sides=6), 0, 1)]
    for row in range(4):
        z = HEIGHT - .45 - row * .14
        for i in range(7):
            angle = i * math.tau / 7 + row * .4
            at = (.87 + .09 * math.cos(angle), -.06 + .09 * math.sin(angle), z)
            parts.append(ready(cylinder('Banana', .022, .15, at, 'banana', (.5, 0, angle), 5), 0, 1))
    parts.append(ready(blob('Heart', (.11, .11, .2), (.88, -.06, HEIGHT - 1.22), 'heart', bulge=.7, taper=.5), 0, 1))
    return parts


def build():
    root = scene.root('Pisang')
    parts = stem(0, 0, HEIGHT, .11) + leaf.crown((.32, .19, HEIGHT - .08), 9, 1.9) + bunch()
    parts.append(leaf.leaf(1.7, .6, 4.0, .15, 1.25, (.3, .18, HEIGHT - .3), 'leaf_dry', twist=.6))
    parts += stem(-.55, .35, 1.0, .06) + leaf.crown((-.55 + .32 * .2, .35 + .19 * .2, .95), 4, 1.0, start=1.0)   # the sucker
    scene.join('Body', parts, root)
    return 'Nature', 'Pisang'
