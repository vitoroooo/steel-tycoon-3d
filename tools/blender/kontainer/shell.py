"""The box: frame of posts and rails with corner castings, wall panels set into it, roof and floor."""
from kit.finish import ready
from kit.parts import box
from .dims import CAST, HEIGHT, LENGTH, POST, RAIL, SKIN, WIDTH


def frame():
    parts = []
    for x in (-1, 1):
        for y in (-1, 1):
            at = (x * (WIDTH - POST) / 2, y * (LENGTH - POST) / 2)
            parts.append(ready(box('Post', (POST, POST, HEIGHT), (*at, HEIGHT / 2), 'frame'), .008))
            for z in (CAST / 2, HEIGHT - CAST / 2):
                corner = (x * (WIDTH - CAST) / 2 + x * .01, y * (LENGTH - CAST) / 2 + y * .01, z)
                parts.append(ready(box('Casting', (CAST, CAST, CAST), corner, 'steel'), .01))
        for z in (RAIL / 2, HEIGHT - RAIL / 2):
            parts.append(ready(box('SideRail', (POST * .6, LENGTH, RAIL), (x * (WIDTH - POST * .6) / 2, 0, z), 'frame'), .006))
    for y in (-1, 1):
        for z in (RAIL / 2, HEIGHT - RAIL / 2):
            parts.append(ready(box('EndRail', (WIDTH, POST * .6, RAIL), (0, y * (LENGTH - POST * .6) / 2, z), 'frame'), .006))
    return parts


def panels():
    """Side walls and the closed end carry the corrugated paint; the texture supplies the ribs."""
    parts = []
    for x in (-1, 1):
        parts.append(ready(box('Side', (.02, LENGTH - POST, HEIGHT - RAIL * 2), (x * (WIDTH / 2 - SKIN), 0, HEIGHT / 2),
                               'paint'), 0, 1.94, 'z'))
    parts.append(ready(box('End', (WIDTH - POST, .02, HEIGHT - RAIL * 2), (0, LENGTH / 2 - SKIN, HEIGHT / 2), 'paint'),
                       0, 1.94, 'z'))
    parts.append(ready(box('Roof', (WIDTH - .04, LENGTH - .04, .02), (0, 0, HEIGHT - .03), 'roof'), 0, 1.94, 'y'))
    parts.append(ready(box('Floor', (WIDTH - .04, LENGTH - .04, .04), (0, 0, RAIL), 'frame'), 0))
    return parts
