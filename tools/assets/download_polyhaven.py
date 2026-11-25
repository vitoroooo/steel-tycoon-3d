"""Download the Poly Haven CC0 surfaces listed in polyhaven_textures.json at 1K.

Each surface gets color, OpenGL normal and roughness maps plus a derived URP mask
(metallic in R, smoothness in A). Surfaces already on disk are skipped, so adding one
line to the JSON list and re-running fetches only the new surface. "tintable": true also
derives tint.jpg, a near-white greyscale of the colour map for materials that paint it.
"""
import hashlib
import json
import urllib.request
from pathlib import Path
from PIL import Image, ImageStat

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1] / 'unity/Assets/ThirdParty/PolyHaven'
RECORDS = ROOT / 'downloads.json'
HEADERS = {'User-Agent': 'SteelTycoonPrototype/1.0 (CC0 assets)'}
MAPS = [('Diffuse', 'color'), ('nor_gl', 'normal'), ('Rough', 'roughness')]


def fetch(url):
    request = urllib.request.Request(url, headers=HEADERS)
    return urllib.request.urlopen(request, timeout=60).read()


def save(entry, path):
    """Writes one provider file after checking the MD5 the provider publishes."""
    data = fetch(entry['url'])
    if hashlib.md5(data).hexdigest() != entry['md5']:
        raise ValueError('Provider checksum mismatch: ' + entry['url'])
    path.write_bytes(data)
    return dict(file=str(path.relative_to(ROOT)), url=entry['url'], bytes=len(data),
                sha256=hashlib.sha256(data).hexdigest())


def pack_mask(folder, metal):
    rough = Image.open(folder / 'roughness.jpg').convert('L')
    smooth = rough.point(lambda value: 255 - value)
    metallic = Image.new('L', rough.size, 128 if metal else 0)
    zero = Image.new('L', rough.size, 0)
    Image.merge('RGBA', (metallic, zero, zero, smooth)).save(folder / 'mask.png')


def derive_tint(folder):
    """Keeps the wear and dirt of the colour map but drops its hue, so a material tint sets the paint."""
    grey = Image.open(folder / 'color.jpg').convert('L')
    mean = ImageStat.Stat(grey).mean[0]
    grey.point(lambda value: min(255, int(value * 215 / mean))).convert('RGB').save(folder / 'tint.jpg', quality=92)


def download(texture):
    files = json.loads(fetch('https://api.polyhaven.com/files/' + texture['id']))
    folder = ROOT / texture['id']
    folder.mkdir(exist_ok=True)
    records = []
    for key, filename in MAPS:
        options = files[key]['1k']
        records.append(save(options.get('jpg') or options['png'], folder / (filename + '.jpg')))
    pack_mask(folder, texture['metal'])
    return records


def main():
    records = json.loads(RECORDS.read_text())
    wanted = json.loads((HERE / 'polyhaven_textures.json').read_text())['textures']
    for texture in wanted:
        folder = ROOT / texture['id']
        if not (folder / 'mask.png').exists():
            records += download(texture)
            print('Downloaded', texture['id'])
        if texture.get('tintable') and not (folder / 'tint.jpg').exists():
            derive_tint(folder)
    RECORDS.write_text(json.dumps(records, separators=(',', ':')) + '\n')


if __name__ == '__main__':
    main()
