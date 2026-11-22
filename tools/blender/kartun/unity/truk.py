"""A heavy truck as a body and rolling wheels; the trailer adds a deck that swings on its kingpin."""
from ..truk import cab, cargo, deck, frame, spec
from .rig import Node

NAMES = {'engkel': 'EngkelKartun', 'tronton': 'TrontonKartun', 'trailer': 'TrailerKartun'}


def target(variant):
    return ('Vehicles', NAMES[variant])


def nodes(variant):
    truck = spec.FLEET[variant]
    body = cab.shell(truck) + frame.chassis(truck) + (cargo.tub(truck) if truck.wall else [])
    found = [Node('Body', (0, 0, 0), body)] + _rolling('Wheel', truck.axles, frame.wheels(truck.axles, truck.dual))
    if variant == 'trailer':
        pin = spec.kingpin(truck)
        found.append(Node('Deck', (0, pin, spec.FRAME_TOP), deck.body(pin)))
        found += _rolling('DeckWheel', [pin + y for y in spec.DECK_AXLES], deck.wheels(pin))
    return found


def _rolling(prefix, axles, groups):
    """Wheel_1L, Wheel_1R, Wheel_2L ... each turning on its own axle."""
    joints = [(side * spec.TRACK, y, spec.WHEEL) for y in axles for side in (1, -1)]
    names = ['%s_%d%s' % (prefix, i + 1, side) for i in range(len(axles)) for side in 'LR']
    return [Node(name, joint, parts) for name, joint, parts in zip(names, joints, groups)]
