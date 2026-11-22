"""Splits the land between the streets into parcels, one per place, so no ground is left without a use."""
import numpy as np
from PIL import Image, ImageDraw
from scipy import ndimage
import ways
from plan import SCALE, metres, px, size

SLIVER = 220   # square metres: smaller leftovers between two streets become verge


def owners(plan):
    """For every pixel: the index of the place it belongs to, -1 for a way, -2 for land nobody claimed."""
    mask = Image.new('L', size(plan), 0)
    ways.draw_all(ImageDraw.Draw(mask), plan, dict.fromkeys(ways.COLOURS, 255))
    land = np.asarray(mask) == 0
    (left, top), (right, bottom) = px(plan, plan['bounds'][0], plan['bounds'][3]), px(plan, plan['bounds'][2], plan['bounds'][1])
    edge = np.ones_like(land)
    edge[int(top):int(bottom), int(left):int(right)] = False   # the sheet's margin is not part of the city
    blocks, _ = ndimage.label(land & ~edge)
    seeds = np.array([px(plan, p[0], p[1]) for p in plan['places']])
    home = [blocks[int(y), int(x)] for x, y in seeds]
    owner = np.full(blocks.shape, -1, dtype=int)
    unclaimed = []
    for block, where in enumerate(ndimage.find_objects(blocks), start=1):
        inside = blocks[where] == block
        rows, cols = np.nonzero(inside)
        rows, cols = rows + where[0].start, cols + where[1].start
        mine = [i for i, b in enumerate(home) if b == block]
        if not mine:
            owner[rows, cols] = -2
            if len(rows) / SCALE ** 2 > SLIVER:
                unclaimed.append((metres(plan, cols.mean(), rows.mean()), len(rows) / SCALE ** 2))
            continue
        far = (cols[:, None] - seeds[mine, 0]) ** 2 + (rows[:, None] - seeds[mine, 1]) ** 2
        owner[rows, cols] = np.array(mine)[far.argmin(axis=1)]
    on_way = [plan['places'][i][3] for i, b in enumerate(home) if b == 0]
    return owner, unclaimed, on_way


def paint(plan, owner):
    """The land coloured by district, with a lane wherever two places share a block."""
    # the last two rows answer the negative owners: -2 is verge between streets, -1 lies under a way or the margin
    colours = np.array([_rgb(plan['areas'][p[2]][1]) for p in plan['places']] + [_rgb('#B9D8A4'), _rgb('#F4EBDD')])
    picture = colours[owner]
    lane = np.zeros(owner.shape, dtype=bool)
    for shift in ((0, 1), (1, 0)):
        other = np.roll(owner, shift, axis=(0, 1))
        lane |= (owner >= 0) & (other >= 0) & (owner != other)
    picture[ndimage.binary_dilation(lane, iterations=1)] = _rgb('#8E8674')
    return Image.fromarray(picture.astype(np.uint8), 'RGB')


def centres(plan, owner):
    """Where to write each place's name: the middle of its parcel, and the parcel's size in square metres."""
    index = list(range(len(plan['places'])))
    middle = ndimage.center_of_mass(np.ones_like(owner), owner, index)
    sizes = ndimage.sum(np.ones_like(owner), owner, index) / SCALE ** 2
    return [(x, y) for y, x in middle], sizes


def _rgb(code):
    return [int(code[i:i + 2], 16) for i in (1, 3, 5)]
