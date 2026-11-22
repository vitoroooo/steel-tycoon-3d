"""Bakes the plan into what the game reads. Run: python3 tools/denah/export.py

Writes Resources/Map/Sukabaja.json (streets as smooth lines, places with their size) and
Resources/Map/SukabajaLahan.bytes, a PNG at one pixel per metre: red is the place that owns the
ground (index + 1, 0 for none), green is what runs over it (see KINDS), blue marks verge.
"""
import json
import numpy as np
from PIL import Image, ImageDraw
import parcels, ways
from curves import smooth
from plan import ROOT, SCALE, load, px, size

OUT = ROOT / 'unity/Assets/_Project/Resources/Map'
KINDS = {'kali': 1, 'rel': 2, 'kolektor': 3, 'arteri': 4, 'lingkar': 5}
FINE = 40   # pieces between two written points: about two metres each, so curves build round


def baked_ways(plan):
    found = []
    for way in plan['ways']:
        line = smooth(way['points'], way.get('closed', False), FINE)
        found.append({'name': way['name'], 'kind': way['kind'], 'width': plan['widths'][way['kind']],
                      'closed': way.get('closed', False),
                      'xs': [round(x, 2) for x, _ in line], 'zs': [round(z, 2) for _, z in line]})
    return found


def rect(box):
    return dict(zip(('west', 'south', 'east', 'north'), box))


def per_metre(plan, picture):
    """One sample per metre, taken at the middle of that metre; row 0 is the north edge."""
    west, south, east, north = plan['bounds']
    left, top = px(plan, west + .5, north - .5)
    cols = (left + np.arange(int(east - west)) * SCALE).astype(int)
    rows = (top + np.arange(int(north - south)) * SCALE).astype(int)
    return picture[np.ix_(rows, cols)]


def main():
    plan = load()
    owner, unclaimed, on_way = parcels.owners(plan)
    if unclaimed or on_way:
        raise SystemExit('Denah belum lengkap: jalankan render.py dan isi blok yang kosong.')
    kinds = Image.new('L', size(plan), 0)
    ways.draw_all(ImageDraw.Draw(kinds), plan, KINDS)
    owner, over = per_metre(plan, owner), per_metre(plan, np.asarray(kinds))
    land = np.zeros(owner.shape + (3,), dtype=np.uint8)
    land[..., 0] = np.where(owner >= 0, owner + 1, 0)
    land[..., 1] = over
    land[..., 2] = np.where(owner == -2, 255, 0)
    OUT.mkdir(parents=True, exist_ok=True)
    Image.fromarray(land, 'RGB').save(OUT / 'SukabajaLahan.bytes', format='PNG')
    sqm = np.bincount(owner[owner >= 0], minlength=len(plan['places']))
    places = [{'name': p[3].replace('|', ' '), 'detail': p[4].replace('|', ' '), 'area': p[2], 'x': p[0], 'z': p[1],
               'sqm': int(sqm[i])} for i, p in enumerate(plan['places'])]
    baked = {'bounds': rect(plan['bounds']), 'core': rect(plan['current']), 'built': [rect(r) for r in plan['built']],
             'ways': baked_ways(plan), 'places': places,
             'roundabouts': [{'x': x, 'z': z, 'radius': r} for x, z, r in plan['roundabouts']]}
    (OUT / 'Sukabaja.json').write_text(json.dumps(baked, ensure_ascii=False, separators=(',', ':')) + '\n')
    print('MAP_BAKED', len(baked['ways']), 'ways,', len(places), 'places, smallest', int(sqm.min()), 'm2,', land.shape[:2])


if __name__ == '__main__':
    main()
