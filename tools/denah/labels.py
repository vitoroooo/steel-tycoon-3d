"""Writes each place's name and what stands there into the middle of its parcel."""
from PIL import ImageFont

INK = '#1A1C24'
FONT = '/usr/share/fonts/truetype/dejavu/DejaVuSans%s.ttf'
BIG = 9000   # square metres: sites this large get a title


def font(size, bold=False):
    return ImageFont.truetype(FONT % ('-Bold' if bold else ''), size)


def write(draw, plan, centres, sizes):
    for place, centre, size in zip(plan['places'], centres, sizes):
        title, body = (15, 11) if size > BIG else (12, 10) if size > 1400 else (11, 9)
        lines = [(line, font(title, True)) for line in place[3].split('|')]
        lines += [(line, font(body)) for line in place[4].split('|') if line]
        heights = [f.size + 3 for _, f in lines]
        y = centre[1] - sum(heights) / 2
        for (text, face), height in zip(lines, heights):
            draw.text((centre[0], y + height / 2), text, fill=INK, font=face, anchor='mm')
            y += height


def street_names(draw, plan, path):
    """The name of every arterial and the ring road, written once beside its middle."""
    for way in plan['ways']:
        if way['kind'] in ('lingkar', 'arteri'):
            points = path(plan, way)
            x, y = points[len(points) * way.get('nameAt', 30) // 100]
            draw.text((x, y), way['name'], fill='#F4EBDD', font=font(10, True), anchor='mm',
                      stroke_width=2, stroke_fill='#39404F')
