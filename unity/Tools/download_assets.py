"""Download Poly Haven CC0 maps at 1K, verify provider MD5, pack URP smoothness."""
import hashlib
import json
import urllib.request
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1] / 'Assets/ThirdParty/PolyHaven'
ASSETS = ['asphalt_02', 'concrete_floor_02', 'rusty_metal_04',
          'corrugated_iron', 'brown_mud_02']
HEADERS = {'User-Agent': 'SteelTycoonPrototype/1.0 (CC0 assets)'}


def fetch(url):
    return urllib.request.urlopen(urllib.request.Request(url, headers=HEADERS), timeout=60).read()


def save(entry, path):
    data = fetch(entry['url'])
    if hashlib.md5(data).hexdigest() != entry['md5']:
        raise ValueError('Provider checksum mismatch: ' + entry['url'])
    path.write_bytes(data)
    return dict(file=str(path.relative_to(ROOT)), url=entry['url'], bytes=len(data),
                sha256=hashlib.sha256(data).hexdigest())


def main():
    records = []
    for asset in ASSETS:
        maps = json.loads(fetch('https://api.polyhaven.com/files/' + asset))
        folder = ROOT / asset
        folder.mkdir(exist_ok=True)
        for key, filename in [('Diffuse', 'color'), ('nor_gl', 'normal'), ('Rough', 'roughness')]:
            options = maps[key]['1k']
            entry = options.get('jpg') or options['png']
            records.append(save(entry, folder / (filename + '.jpg')))
        rough = Image.open(folder / 'roughness.jpg').convert('L')
        smooth = rough.point(lambda value: 255 - value)
        metal = Image.new('L', rough.size, 128 if 'metal' in asset or 'iron' in asset else 0)
        zero = Image.new('L', rough.size, 0)
        Image.merge('RGBA', (metal, zero, zero, smooth)).save(folder / 'mask.png')
    sky = json.loads(fetch('https://api.polyhaven.com/files/industrial_sunset_puresky'))
    records.append(save(sky['hdri']['1k']['hdr'], ROOT / 'sky.hdr'))
    (ROOT / 'downloads.json').write_text(json.dumps(records, separators=(',', ':')) + '\n')
    print('Verified download bytes:', sum(r['bytes'] for r in records))


if __name__ == '__main__':
    main()
