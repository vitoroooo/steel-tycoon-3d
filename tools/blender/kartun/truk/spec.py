"""The heavy fleet as numbers, in metres; the front points to -Y. One cab serves every truck."""
from collections import namedtuple

# axles: Y of each axle, the first one steers. dual: twin tyres on every axle but the first.
Truck = namedtuple('Truck', 'name length axles dual wall')

WHEEL, TYRE, TRACK = .5, .3, .98
CAB = (2.3, 1.9)                 # width, length
BELT, ROOF = 1.75, 2.75          # bottom of the windows, top of the cab
FRAME_TOP = 1.05
DECK = (2.4, 7.4)                # the flat-deck trailer: width, length
DECK_TOP, DECK_NOSE = 1.42, 1.1  # loading height, how far the deck reaches ahead of its kingpin
DECK_AXLES = (4.3, 5.5)          # behind the kingpin

ENGKEL = Truck('Engkel', 6.6, (-2.3, 1.9), False, 1.0)            # "engkel": single rear tyres, 8 tonnes
TRONTON = Truck('Tronton', 9.2, (-3.6, 2.2, 3.45), True, 1.3)     # three axles, 20 tonnes and more
KEPALA = Truck('Kepala trailer', 6.2, (-2.1, 1.2, 2.45), True, 0)  # the tractor that pulls the deck
FLEET = {'engkel': ENGKEL, 'tronton': TRONTON, 'trailer': KEPALA}


def nose(truck):
    return -truck.length / 2


def cab_y(truck):
    return nose(truck) + CAB[1] / 2


def cab_back(truck):
    return nose(truck) + CAB[1]


def kingpin(truck):
    """Where the trailer rests on the tractor: over the middle of the driven axles."""
    return sum(truck.axles[1:]) / len(truck.axles[1:])
