"""A 20-tonne tracked excavator with a hydraulic breaker, for the demolition site."""
from kit import scene
from . import arm, base, dims, house


def build():
    """The machine stands still; the arm is its own child (Lengan) pivoted on the boom foot so Unity can work it."""
    root = scene.root('Ekskavator')
    scene.join('Body', base.undercarriage() + house.body() + house.cab(), root)
    scene.pivot(scene.join('Lengan', arm.arm(), root), dims.BOOM_FOOT)
    return 'Props', 'Ekskavator'
