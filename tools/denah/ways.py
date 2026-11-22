"""Draws streets, the river, the railway and the roundabout, all to scale."""
from plan import px, SCALE

COLOURS = {'lingkar': '#3B4252', 'arteri': '#3B4252', 'kolektor': '#6B7280', 'kali': '#7CC3E2', 'rel': '#8A6F5A'}


def _strip(draw, plan, kind, along_x, at):
    """One line of the grid, as wide as the real thing."""
    half = plan['widths'][kind] / 2
    if along_x:
        start, end = px(plan, at - half, plan['z'][-1]), px(plan, at + half, plan['z'][0])
    else:
        start, end = px(plan, plan['x'][0], at + half), px(plan, plan['x'][-1], at - half)
    draw.rectangle([start, end], fill=COLOURS[kind])
    if kind in ('lingkar', 'arteri'):
        middle = px(plan, at, at)
        line = [middle[0], start[1], middle[0], end[1]] if along_x else [start[0], middle[1], end[0], middle[1]]
        draw.line(line, fill='#F2B632', width=2)
    if kind == 'rel':
        for step in range(int(start[1]), int(end[1]), 12):
            draw.line([start[0], step, end[0], step], fill='#F4EBDD', width=2)


def draw_all(draw, plan):
    order = ['kali', 'kolektor', 'rel', 'arteri', 'lingkar']   # bridges read as road over water
    for kind in order:
        for at, found in plan['linesX'].items():
            if found == kind: _strip(draw, plan, kind, True, float(at))
        for at, found in plan['linesZ'].items():
            if found == kind: _strip(draw, plan, kind, False, float(at))
    x, z, radius = plan['roundabout']
    centre = px(plan, x, z)
    for size, fill in ((radius, COLOURS['arteri']), (radius - 9, '#8FCB8A'), (4, '#F2B632')):
        reach = size * SCALE
        draw.ellipse([centre[0] - reach, centre[1] - reach, centre[0] + reach, centre[1] + reach], fill=fill)


def current(draw, plan):
    """Outline of the map the game has today, for size."""
    west, south, east, north = plan['current']
    draw.rectangle([px(plan, west, north), px(plan, east, south)], outline='#D7263D', width=4)
