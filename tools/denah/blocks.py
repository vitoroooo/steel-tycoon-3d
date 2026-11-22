"""Draws the city blocks and their names."""
from PIL import ImageFont
from plan import cell_box, cell_inner

BOLD = '/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf'
PLAIN = '/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf'
INK = '#1A1C24'


def font(size, bold=False):
    return ImageFont.truetype(BOLD if bold else PLAIN, size)


def label(draw, bounds, text, big=False):
    """Centres a name in its block: first line bold, the rest plain."""
    lines = [line for line in text.split('|') if line]
    sizes = [(17 if big else 13) if i == 0 else (13 if big else 11) for i in range(len(lines))]
    height = sum(size + 4 for size in sizes)
    y = (bounds[1] + bounds[3]) / 2 - height / 2
    for i, line in enumerate(lines):
        draw.text(((bounds[0] + bounds[2]) / 2, y), line, fill=INK, font=font(sizes[i], i == 0), anchor='ma')
        y += sizes[i] + 4


def cells(draw, plan):
    for row, names in enumerate(plan['cells']):
        for col, (area, name) in enumerate(names):
            bounds = cell_box(plan, col, row)
            draw.rectangle(bounds, fill=plan['areas'][area][1])
            label(draw, cell_inner(plan, col, row), name)


def merged(draw, plan):
    """Sites that take more than one block; drawn over the streets they swallow."""
    for site in plan['merged']:
        north = len(plan['cells']) - 1 - site['rows'][1]
        south = len(plan['cells']) - 1 - site['rows'][0]
        first, last = cell_box(plan, site['cols'][0], north), cell_box(plan, site['cols'][1], south)
        bounds = [first[0] + 14, first[1] + 14, last[2] - 14, last[3] - 14]
        draw.rounded_rectangle(bounds, 10, fill=plan['areas'][site['area']][1], outline=INK, width=3)
        label(draw, bounds, site['name'], big=True)
