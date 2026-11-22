"""What sits on a citizen's head. Each returns parts for the head joint."""
from .. import shapes
from ..paint import cel
from ..pengepul import dims
from ..rods import rod

X, Y, Z = dims.HEAD


def _crop(tone='hair'):
    fringe = [shapes.capsule('Poni', (x, -.2, Z + .2), (x + .05, -.272, Z + .135), .08, .05, cel(tone), 14)
              for x in (-.15, -.01, .13)]   # swept to one side, so the hairline is drawn, not a bowl
    return [shapes.ball('Rambut', (0, .04, Z + .07), (.342, .312, .262), cel(tone), 32, True)] + fringe


def hijab():
    """Wraps the skull so only the face shows, then falls over the shoulders."""
    return [shapes.ball('Hijab', (0, .04, Z + .01), (.36, .31, .33), cel('teal'), 32, True),
            shapes.capsule('Juntai hijab', (0, .01, .84), (0, .0, .64), .19, .265, cel('teal'), 24),
            shapes.tube('Tepi hijab', shapes.arc((0, -.262, Z - .01), .215, 20, 160, 'xz', 13), .014, cel('cream'))]


def peci():
    return _crop() + [rod('Peci', (0, .02, Z + .17), (0, .02, Z + .4), .3, cel('ink'), 28, .2),
                      shapes.tube('Jahitan peci', shapes.arc((0, .02, Z + .23), .303, 0, 360, 'xy', 17)[:-1], .008,
                                  cel('grey'), True)]


def helmet():
    return _crop() + [shapes.ball('Helm', (0, .0, Z + .2), (.335, .32, .2), cel('yellow'), 28),
                      shapes.block('Pet helm', (0, -.27, Z + .15), (.4, .2, .035), cel('yellow'), .45),
                      shapes.capsule('Rusuk helm', (0, -.2, Z + .36), (0, .2, Z + .36), .05, .05, cel('orange'), 12)]


def cropped():
    """Short hair with one stubborn tuft at the crown."""
    tuft = shapes.capsule('Jambul', (.05, -.16, Z + .24), (.13, -.25, Z + .33), .07, .035, cel('hair'), 14)
    return _crop() + [tuft]
