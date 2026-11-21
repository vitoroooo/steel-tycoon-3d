"""Download the Poly Haven CC0 sky used by the city as sky.hdr, and report where its sun is.

Usage: python3 tools/assets/download_polyhaven_sky.py [asset_id] [resolution]
The sun's yaw and pitch (degrees, for an unrotated panoramic skybox) are printed so the
directional light in CityLighting can be lined up with the picture.
"""
import hashlib
import json
import sys
import urllib.request
from pathlib import Path
import numpy as np
import radiance

ROOT = Path(__file__).resolve().parents[2] / 'unity/Assets/ThirdParty/PolyHaven'
HEADERS = {'User-Agent': 'SteelTycoonPrototype/1.0 (CC0 assets)'}


def fetch(url):
    return urllib.request.urlopen(urllib.request.Request(url, headers=HEADERS), timeout=300).read()


def record(entry, data):
    """Replaces the sky line in downloads.json so provenance always matches the file on disk."""
    path = ROOT / 'downloads.json'
    records = [r for r in json.loads(path.read_text()) if r['file'] != 'sky.hdr']
    records.append(dict(file='sky.hdr', url=entry['url'], bytes=len(data), sha256=hashlib.sha256(data).hexdigest()))
    path.write_text(json.dumps(records, separators=(',', ':')) + '\n')


def sun_direction(path):
    """Centre of the brightest pixels in the equirectangular image, as yaw and pitch."""
    image = radiance.brightness(path)
    rows, columns = np.nonzero(image >= np.percentile(image, 99.995))
    yaw = (columns.mean() / image.shape[1] - .5) * 360
    pitch = (.5 - rows.mean() / image.shape[0]) * 180
    return yaw, pitch


def main():
    asset = sys.argv[1] if len(sys.argv) > 1 else 'kloofendal_48d_partly_cloudy_puresky'
    resolution = sys.argv[2] if len(sys.argv) > 2 else '4k'
    entry = json.loads(fetch('https://api.polyhaven.com/files/' + asset))['hdri'][resolution]['hdr']
    data = fetch(entry['url'])
    if hashlib.md5(data).hexdigest() != entry['md5']:
        raise ValueError('Provider checksum mismatch: ' + entry['url'])
    (ROOT / 'sky.hdr').write_bytes(data)
    record(entry, data)
    yaw, pitch = sun_direction(ROOT / 'sky.hdr')
    print('SKY', asset, resolution, len(data), 'bytes; sun yaw %.1f pitch %.1f' % (yaw, pitch))


if __name__ == '__main__':
    main()
