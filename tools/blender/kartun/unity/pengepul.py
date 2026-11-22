"""The pengepul as jointed parts. Node names follow the Bip01 bones the game already binds to."""
from ..pengepul import body, dims, gear, head
from .rig import Node, named

TARGET = ('Characters', 'Pengepul')


def _arm(side, tag):
    parts, x = body.arm(side), side
    shoulder = (x * dims.SHOULDER[0],) + dims.SHOULDER[1:]
    elbow = (x * dims.ELBOW[0],) + dims.ELBOW[1:]
    wrist = (x * dims.WRIST[0],) + dims.WRIST[1:]
    return [
        Node(f'Bip01 {tag} UpperArm', shoulder, named(parts, 'Lengan baju', 'Lipatan', 'Lengan atas')),
        Node(f'Bip01 {tag} Forearm', elbow, named(parts, 'Lengan bawah')),
        Node(f'Bip01 {tag} Hand', wrist, named(parts, 'Sarung tangan', 'Jempol', 'Manset')),
    ]


def _leg(side, tag):
    parts, x = body.leg(side), side * .105
    return [
        Node(f'Bip01 {tag} Thigh', (side * dims.HIP_X, 0, dims.HIP_Z), named(parts, 'Pipa celana', 'Kelim')),
        Node(f'Bip01 {tag} Calf', (x, 0, dims.KNEE_Z + .06), named(parts, 'Betis')),
        Node(f'Bip01 {tag} Foot', (x, .005, dims.ANKLE_Z), named(parts, 'Kaki', 'Sandal', 'Tali sandal')),
    ]


def nodes():
    torso = body.torso()
    tree = [
        Node('Bip01', (0, 0, dims.HIP_Z), named(torso, 'Celana', 'Sabuk')),
        Node('Bip01 Spine', (0, 0, .47), named(torso, 'Badan', 'Leher') + gear.towel() + gear.sling_bag()),
        Node('Bip01 Head', dims.NECK, head.skull() + head.face() + head.hair()),
    ]
    # Modelled facing -Y, so +X is the character's left.
    for side, tag in ((1, 'L'), (-1, 'R')):
        tree += _arm(side, tag) + _leg(side, tag)
    return tree
