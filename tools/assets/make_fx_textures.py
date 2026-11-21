"""Paints the self-made particle sprites: a soft smoke puff and a spark dot.

Deterministic: python3 tools/assets/make_fx_textures.py
"""
from pathlib import Path
import numpy as np
from PIL import Image

OUT = Path(__file__).resolve().parents[2] / 'unity/Assets/_Project/Textures/Fx'
SIZE = 256


def falloff():
    axis = np.linspace(-1, 1, SIZE)
    x, y = np.meshgrid(axis, axis)
    return x, y, np.clip(1 - np.hypot(x, y), 0, 1)


def puff(rng):
    """White with a lumpy alpha: several overlapping soft discs, so puffs do not read as perfect circles."""
    x, y, _ = falloff()
    alpha = np.zeros((SIZE, SIZE))
    for _ in range(26):
        cx, cy, radius = rng.uniform(-.42, .42), rng.uniform(-.42, .42), rng.uniform(.22, .5)
        alpha = np.maximum(alpha, np.clip(1 - np.hypot(x - cx, y - cy) / radius, 0, 1) ** 1.6 * rng.uniform(.5, 1))
    alpha *= np.clip(1.15 - np.hypot(x, y), 0, 1) ** .8      # always fades out before the quad's edge
    return alpha


def save(name, alpha):
    pixels = np.full((SIZE, SIZE, 4), 255, dtype=np.uint8)
    pixels[..., 3] = (np.clip(alpha, 0, 1) * 255).astype(np.uint8)
    OUT.mkdir(parents=True, exist_ok=True)
    Image.fromarray(pixels, 'RGBA').save(OUT / name)
    print('WROTE', OUT / name)


def main():
    save('smoke_puff.png', puff(np.random.default_rng(1977)))
    save('spark.png', falloff()[2] ** 2.2)


main()
