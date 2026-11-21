"""An ayam kampung rooster; Unity repaints the slots and rescales it to make hens."""
from kit import scene
from kit.finish import box_uv
from . import body, dims, head, legs, tail


def part(name, pieces, root, origin):
    return scene.pivot(scene.join(name, [box_uv(piece, .25) for piece in pieces], root), origin)


def build():
    """Body, Leher, Ekor and the two Kaki are separate children, each pivoted on its joint, so Unity can animate them."""
    root = scene.root('Ayam')
    part('Body', body.torso() + body.wing(-1) + body.wing(1), root, dims.HIP)
    part('Leher', head.neck() + head.head(), root, dims.NECK_BASE)
    part('Ekor', tail.tail(), root, dims.TAIL_ROOT)
    for name, side in (('KakiKiri', -1), ('KakiKanan', 1)):
        part(name, legs.leg(side), root, (side * dims.HIP_SIDE, dims.HIP[1], dims.HIP[2]))
    return 'Animals', 'Ayam'
