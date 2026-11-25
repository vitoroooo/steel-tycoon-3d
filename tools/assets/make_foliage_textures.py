"""Paints the banana plant's textures: leaf colour with a cut-out outline, its normal map, and the stem.

Self-made (no third-party source), deterministic, and cheap to re-run:
    python3 tools/assets/make_foliage_textures.py
"""
from pathlib import Path
import numpy as np
from PIL import Image

OUT = Path(__file__).resolve().parents[2] / 'unity/Assets/_Project/Textures/Nature'
WIDTH, LENGTH = 512, 1024
VEIN_SLANT, VEINS = .22, 90            # side veins lean towards the tip; count along the blade
FRESH = ((.06, .19, .05), (.19, .35, .10), (.46, .54, .26))   # shade, lit, midrib
DRY = ((.30, .20, .10), (.56, .42, .22), (.50, .40, .26))


def noise(rng, shape, cells):
    """Smooth value noise in 0..1."""
    coarse = Image.fromarray((rng.random(cells) * 255).astype(np.uint8))
    return np.asarray(coarse.resize(shape[::-1], Image.BICUBIC), dtype=np.float32) / 255


def leaf_fields(rng):
    """Per-pixel distance from the midrib, position along the blade, and the vein coordinate."""
    v, u = np.mgrid[0:LENGTH, 0:WIDTH].astype(np.float32)
    v, side = 1 - v / LENGTH, np.abs(u / WIDTH - .5) * 2          # v: 0 at the stem, 1 at the tip
    return side, v, v - VEIN_SLANT * side + (noise(rng, side.shape, (9, 5)) - .5) * .012


def outline(rng, side, v, vein):
    """Alpha: a stalk, then the blade, with wedges torn out along the veins by the wind."""
    blade = np.clip((v - .1) / .12, 0, 1) ** .6 * np.clip((1 - v) / .2, 0, 1) ** .45 * .96
    alpha = (side < np.maximum(blade, .045)).astype(np.float32)
    for at in rng.uniform(.2, .9, 11):
        depth, gap = rng.uniform(.25, .8), rng.uniform(.004, .011)
        alpha[(np.abs(vein - at + VEIN_SLANT) < gap * (side - depth) / (1 - depth)) & (side > depth)] = 0
    return alpha


def colour(rng, palette, side, v, vein):
    shade, lit, rib = (np.array(c, dtype=np.float32) for c in palette)
    stripes = .5 + .5 * np.sin(vein * VEINS * 2 * np.pi)
    mix = np.clip(.25 + .45 * noise(rng, side.shape, (14, 6)) + .12 * stripes + .15 * side, 0, 1)[..., None]
    rgb = shade * (1 - mix) + lit * mix
    midrib = np.clip(1 - side / (.05 - .02 * v), 0, 1)[..., None] ** .7
    scorch = (np.clip((side - .82) / .14, 0, 1) * noise(rng, side.shape, (40, 12)))[..., None]
    rgb = rgb * (1 - midrib) + rib * midrib
    return rgb * (1 - scorch) + np.array(DRY[1], dtype=np.float32) * scorch, stripes, midrib[..., 0]


def normal_map(height, strength):
    dy, dx = np.gradient(height)
    normal = np.dstack([-dx * strength, dy * strength, np.ones_like(height)])
    normal /= np.linalg.norm(normal, axis=2, keepdims=True)
    return normal * .5 + .5


def stem(rng):
    """Vertical fibres with brown dried-sheath patches; tiles in both directions."""
    fibres = np.tile(noise(rng, (1, 512), (1, 90)), (512, 1)) * .5 + noise(rng, (512, 512), (6, 30)) * .5
    patch = np.clip((noise(rng, (512, 512), (3, 11)) - .55) * 3, 0, 1)[..., None]
    green = np.array((.30, .37, .15)) * (.7 + .5 * fibres[..., None])
    brown = np.array((.33, .27, .15)) * (.6 + .6 * fibres[..., None])
    return green * (1 - patch) + brown * patch


def save(name, pixels):
    Image.fromarray((np.clip(pixels, 0, 1) * 255).astype(np.uint8)).save(OUT / name)
    print('WROTE', name)


if __name__ == '__main__':
    rng = np.random.default_rng(7)
    side, v, vein = leaf_fields(rng)
    alpha = outline(rng, side, v, vein)
    for name, palette in (('banana_leaf.png', FRESH), ('banana_leaf_dry.png', DRY)):
        rgb, stripes, midrib = colour(rng, palette, side, v, vein)
        save(name, np.dstack([rgb, alpha]))
    save('banana_leaf_normal.png', normal_map(stripes * .5 + midrib * 4, 1.2))
    save('banana_stem.png', stem(rng))
