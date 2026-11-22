"""Renders the city plan to docs/design/sheets/denah.png. Run: python3 tools/denah/render.py

Fails when a block has no place, or a place was written on top of a street: the plan may leave no ground unused.
"""
import sys
from PIL import Image, ImageDraw
import labels, legend, parcels, ways
from plan import ROOT, load, size

KEY_WIDTH = 470
OUT = ROOT / 'docs/design/sheets/denah.png'


def main():
    plan = load()
    owner, unclaimed, on_way = parcels.owners(plan)
    for (x, z), area in unclaimed:
        print('KOSONG: blok di (%.0f, %.0f), %.0f m2, belum ada tempatnya' % (x, z, area))
    for name in on_way:
        print('DI JALAN: %s' % name)
    width, height = size(plan)
    sheet = Image.new('RGB', (width + KEY_WIDTH, height), '#F4EBDD')
    sheet.paste(parcels.paint(plan, owner), (0, 0))
    draw = ImageDraw.Draw(sheet)
    ways.draw_all(draw, plan)
    ways.details(draw, plan)
    centres, sizes = parcels.centres(plan, owner)
    labels.write(draw, plan, centres, sizes)
    labels.street_names(draw, plan, ways.path)
    ways.current(draw, plan)
    legend.draw(draw, plan, width + 10, 50, len(plan['places']))
    sheet.save(OUT)
    print('DENAH_SAVED', OUT, sheet.size)
    sys.exit(1 if unclaimed or on_way else 0)


if __name__ == '__main__':
    main()
