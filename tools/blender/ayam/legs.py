"""One leg: the feathered thigh, the scaly shank and four toes; pivoted at the hip so it can stride."""
from kit.tubes import tube
from .dims import HIP, HIP_SIDE
from .feathers import egg, smooth

HOCK, ANKLE = .125, .014      # heights of the joint under the thigh and of the foot


def leg(side):
    x, y = side * HIP_SIDE, HIP[1]
    parts = [egg('Thigh', (.066, .09, .12), (x, y + .005, .18), 'feather_body', lumps=.025, taper=-.3, rot=(.35, 0, 0)),
             smooth(tube('Shank', [(x, y - .012, HOCK + .02), (x, y - .004, ANKLE)], .0075, 'shank', bend=0, sides=8)),
             egg('Hock', (.02, .02, .02), (x, y - .01, HOCK), 'shank')]
    toes = (((0, .058), .0058), ((.03, .046), .005), ((-.03, .046), .005), ((0, -.028), .0045))   # reach, girth
    for (dx, dy), girth in toes:
        knuckle = (x + dx * .45, y - .004 + dy * .45, ANKLE + .004)
        claw = (x + dx, y - .004 + dy, .004)
        parts.append(smooth(tube('Toe', [(x, y - .004, ANKLE), knuckle, claw], girth, 'shank', bend=.01, sides=8)))
    return parts
