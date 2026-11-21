"""Paints the tileable cloud-shadow cookie that the sun projects onto the city.

Self-made and deterministic: python3 tools/assets/make_sky_textures.py
White is full sunlight; the soft grey patches are cloud shadows drifting over the ground.
"""
from pathlib import Path
import numpy as np
from PIL import Image

OUT = Path(__file__).resolve().parents[2] / 'unity/Assets/_Project/Textures/Sky/cloud_shadow.png'
SIZE, WAVES = 512, 40
COVER, SOFTNESS, DARKEST = .56, .2, .5   # share of sky that is clear, edge softness, light left under a cloud


def clouds(rng):
    """Sine waves with whole-number frequencies wrap at every edge, so the cookie tiles seamlessly."""
    y, x = np.mgrid[0:SIZE, 0:SIZE].astype(np.float32) / SIZE
    field = np.zeros((SIZE, SIZE), dtype=np.float32)
    for _ in range(WAVES):
        fx, fy = rng.integers(-6, 7), rng.integers(0, 7)
        field += np.sin(2 * np.pi * (fx * x + fy * y) + rng.uniform(0, 6.28)) / (1 + abs(fx) + fy)
    return (field - field.min()) / (field.max() - field.min())


def main():
    field = clouds(np.random.default_rng(3))
    shade = np.clip((field - COVER) / SOFTNESS, 0, 1)
    shade = shade * shade * (3 - 2 * shade)                       # smoothstep
    light = 1 - shade * (1 - DARKEST)
    Image.fromarray((light * 255).astype(np.uint8)).convert('RGB').save(OUT)
    print('WROTE', OUT.name, 'mean light %.2f' % light.mean())


if __name__ == '__main__':
    main()
