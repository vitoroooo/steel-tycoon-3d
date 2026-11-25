"""Fetch licensed source models; keep only the worker FBX and model dependencies."""
import hashlib
import io
import json
import urllib.request
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1] / 'Assets/ThirdParty'
HEADERS = {'User-Agent': 'SteelTycoonPrototype/1.0 (CC0 assets)'}
MODELS = ['barrel_03', 'old_tyre', 'wooden_crate_01', 'modular_industrial_pipes_01',
          'modular_chainlink_fence', 'modular_electric_cables', 'street_lamp_01',
          'rollershutter_window_01', 'rollershutter_door']


def fetch(url):
    return urllib.request.urlopen(urllib.request.Request(url, headers=HEADERS), timeout=90).read()


def record(data, url, path):
    return dict(path=path, url=url, bytes=len(data), sha256=hashlib.sha256(data).hexdigest())


def download_models():
    records = []
    for name in MODELS:
        files = json.loads(fetch('https://api.polyhaven.com/files/' + name))['gltf']['1k']['gltf']
        entries = dict(files.get('include', {}))
        entries[name + '.gltf'] = files
        folder = ROOT / 'PolyHavenModels' / name
        folder.mkdir(exist_ok=True)
        for relative, entry in entries.items():
            data = fetch(entry['url'])
            if hashlib.md5(data).hexdigest() != entry['md5']:
                raise ValueError('Provider MD5 mismatch: ' + relative)
            path = folder / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_bytes(data)
            records.append(record(data, entry['url'], str(path.relative_to(ROOT))))
        print(name, 'downloaded')
    (ROOT / 'PolyHavenModels/checksums.json').write_text(json.dumps(records) + '\n')
    return sum(r['bytes'] for r in records)


def download_worker():
    url = 'https://opengameart.org/sites/default/files/ultimate_animated_character_pack_by_quaternius.zip'
    data = fetch(url)
    archive = zipfile.ZipFile(io.BytesIO(data))
    folder = ROOT / 'Quaternius'
    records = [record(data, url, 'source archive (not committed)')]
    for name in archive.namelist():
        if name.endswith('/FBX/Worker_Male.fbx') or name.lower().endswith(('.png', '.jpg')):
            blob = archive.read(name)
            path = folder / Path(name).name
            path.write_bytes(blob)
            records.append(record(blob, url + '#' + name, path.name))
    url = 'https://static.poly.pizza/4e925a01-dbb8-4aab-848b-221306b835ea.glb'
    blob = fetch(url)
    (folder / 'Pickup.glb').write_bytes(blob)
    records.append(record(blob, url, 'Pickup.glb'))
    (folder / 'checksums.json').write_text(json.dumps(records) + '\n')
    return len(data) + len(blob)


if __name__ == '__main__':
    print('New source download bytes:', download_models() + download_worker())
