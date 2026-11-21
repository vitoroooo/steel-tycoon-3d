"""Paints the self-made ground cover textures: grass tufts (green and dry), an oil stain and a puddle.

Deterministic: python3 tools/assets/make_ground_textures.py
Tufts are alpha cut-outs for the terrain's wind-blown grass; the stain and puddle are soft decals laid flat on the ground.
"""
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw, ImageFilter

OUT = Path(__file__).resolve().parents[2] / 'unity/Assets/_Project/Textures/Ground'
SIZE, SCALE = 256, 4          # final size, and the oversampling that smooths blade edges
GREEN = ((58, 70, 36), (112, 120, 66))
DRY = ((104, 94, 56), (164, 150, 98))


def blade(draw, rng, root_x, colours):
    """One blade: a thin curved strip from the bottom edge, darker at the root, lighter at the tip."""
    big = SIZE * SCALE
    height = rng.uniform(.45, .97) * big
    lean = rng.uniform(-.28, .28) * big
    width = rng.uniform(.022, .046) * big
    steps = 14
    for step in range(steps):
        t0, t1 = step / steps, (step + 1) / steps
        points = []
        for t, side in ((t0, -1), (t0, 1), (t1, 1), (t1, -1)):
            x = root_x + lean * t * t + side * width * (1 - t) * .5
            points.append((x, big - 1 - height * t))
        shade = [int(a + (b - a) * (t0 * .8 + rng.uniform(0, .2))) for a, b in zip(*colours)]
        draw.polygon(points, fill=(*shade, 255))


def tuft(seed, colours, blades=34):
    rng = np.random.default_rng(seed)
    big = SIZE * SCALE
    image = Image.new('RGBA', (big, big), (*colours[0], 0))
    draw = ImageDraw.Draw(image)
    for root in sorted(rng.normal(.5, .17, blades)):
        blade(draw, rng, float(np.clip(root, .08, .92)) * big, colours)
    return image.resize((SIZE, SIZE), Image.LANCZOS)


def blotch(seed, colour):
    """Pool with a soft irregular edge and a few satellite drips: an oil stain or a rain puddle."""
    rng = np.random.default_rng(seed)
    mask = Image.new('L', (SIZE, SIZE), 0)
    draw = ImageDraw.Draw(mask)
    for _ in range(90):
        angle, reach = rng.uniform(0, 6.28), abs(rng.normal(0, .15)) * SIZE
        x, y = SIZE / 2 + np.cos(angle) * reach * 1.3, SIZE / 2 + np.sin(angle) * reach * .8
        radius = rng.uniform(.015, .09) * SIZE * max(.15, 1 - 2.2 * reach / SIZE)
        draw.ellipse((x - radius, y - radius, x + radius, y + radius), fill=255)
    # Blur, then steepen: neighbouring drops merge into one pool with a soft but definite edge.
    mask = mask.filter(ImageFilter.GaussianBlur(7)).point(lambda v: int(min(1, max(0, (v - 40) / 90)) * 215))
    mask = mask.filter(ImageFilter.GaussianBlur(1.5))
    stain = Image.new('RGBA', (SIZE, SIZE), (*colour, 0))
    stain.putalpha(mask)
    return stain


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    tuft(5, GREEN).save(OUT / 'grass_tuft.png')
    tuft(9, DRY, blades=26).save(OUT / 'grass_tuft_dry.png')
    blotch(2, (22, 19, 17)).save(OUT / 'oil_stain.png')
    blotch(8, (70, 74, 72)).save(OUT / 'puddle.png')
    print('WROTE grass_tuft.png grass_tuft_dry.png oil_stain.png puddle.png')


if __name__ == '__main__':
    main()
