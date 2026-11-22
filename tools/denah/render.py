"""Renders the city plan to docs/design/sheets/denah.png. Run: python3 tools/denah/render.py"""
from pathlib import Path
from PIL import Image, ImageDraw
import blocks, legend, ways
from plan import load, size

KEY_WIDTH = 440
OUT = Path(__file__).resolve().parents[2] / 'docs/design/sheets/denah.png'


def main():
    plan = load()
    width, height = size(plan)
    sheet = Image.new('RGB', (width + KEY_WIDTH, height), '#F4EBDD')
    draw = ImageDraw.Draw(sheet)
    blocks.cells(draw, plan)
    ways.draw_all(draw, plan)
    blocks.merged(draw, plan)
    ways.current(draw, plan)
    legend.draw(draw, plan, width + 10, 50)
    sheet.save(OUT)
    print('DENAH_SAVED', OUT, sheet.size)


if __name__ == '__main__':
    main()
