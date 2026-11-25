"""Draws everything that runs through the city: river, railway, streets and roundabouts."""
from curves import smooth
from plan import SCALE, px

COLOURS = {'lingkar': '#39404F', 'arteri': '#454C5C', 'kolektor': '#6B7280', 'rel': '#8A6A52', 'kali': '#7DC3E0'}
ORDER = ('kali', 'rel', 'kolektor', 'arteri', 'lingkar')
LINED = ('lingkar', 'arteri')
MARKING = '#F2B632'


def path(plan, way):
    return [px(plan, x, z) for x, z in smooth(way['points'], way.get('closed', False))]


def stroke(draw, points, width, colour):
    draw.line(points, fill=colour, width=max(1, int(round(width))), joint='curve')
    radius = width / 2
    for x, y in (points[0], points[-1]):
        draw.ellipse([x - radius, y - radius, x + radius, y + radius], fill=colour)


def draw_all(draw, plan, colours=COLOURS):
    """Paints every way, widest class last. Pass one colour for all kinds to get a mask of what is not land."""
    for kind in ORDER:
        for way in plan['ways']:
            if way['kind'] == kind:
                stroke(draw, path(plan, way), plan['widths'][kind] * SCALE, colours[kind])
    for x, z, radius in plan['roundabouts']:
        left, top = px(plan, x - radius, z + radius)
        right, bottom = px(plan, x + radius, z - radius)
        draw.ellipse([left, top, right, bottom], fill=colours['lingkar'])


def details(draw, plan):
    """Lane markings, sleepers and the islands in the roundabouts: only on the finished sheet."""
    for way in plan['ways']:
        points = path(plan, way)
        if way['kind'] in LINED:
            draw.line(points, fill=MARKING, width=1)
        if way['kind'] == 'rel':
            for (ax, ay), (bx, by) in list(zip(points, points[1:]))[::2]:
                draw.line([ax, ay, (ax + bx) / 2, (ay + by) / 2], fill='#E9DCC6', width=int(plan['widths']['rel'] * SCALE * .7))
    for x, z, radius in plan['roundabouts']:
        for part, colour in ((.55, '#7FC08A'), (.25, MARKING)):
            left, top = px(plan, x - radius * part, z + radius * part)
            right, bottom = px(plan, x + radius * part, z - radius * part)
            draw.ellipse([left, top, right, bottom], fill=colour)


def current(draw, plan):
    """Outline of the map as it is built today, for scale."""
    west, south, east, north = plan['current']
    draw.rectangle([*px(plan, west, north), *px(plan, east, south)], outline='#D7263D', width=3)
