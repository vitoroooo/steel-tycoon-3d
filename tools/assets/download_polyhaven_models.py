"""Download the Poly Haven CC0 props listed in polyhaven_models.json as glTF with 1K maps.

Every file is checked against the provider's MD5 and recorded in checksums.json with its
source URL, size and SHA256. Models already on disk are skipped, so adding one id to the
JSON list and re-running fetches only the new prop.
"""
import hashlib
import json
import urllib.request
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1] / 'unity/Assets/ThirdParty/PolyHavenModels'
RECORDS = ROOT / 'checksums.json'
LICENSES = ROOT / 'LICENSES.md'
HEADERS = {'User-Agent': 'SteelTycoonPrototype/1.0 (CC0 assets)'}


def fetch(url):
    request = urllib.request.Request(url, headers=HEADERS)
    return urllib.request.urlopen(request, timeout=120).read()


def save(entry, path, record_path):
    """Writes one verified file and returns its provenance record."""
    data = fetch(entry['url'])
    if hashlib.md5(data).hexdigest() != entry['md5']:
        raise ValueError('MD5 mismatch for ' + entry['url'])
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(data)
    return {'path': record_path, 'url': entry['url'], 'bytes': len(data),
            'sha256': hashlib.sha256(data).hexdigest()}


def download(model):
    """Fetches the glTF and everything it references; returns the new records."""
    files = json.loads(fetch('https://api.polyhaven.com/files/' + model))
    gltf = files['gltf']['1k']['gltf']
    prefix = 'PolyHavenModels/' + model + '/'
    records = [save(gltf, ROOT / model / (model + '.gltf'), prefix + model + '.gltf')]
    for name, entry in gltf['include'].items():
        records.append(save(entry, ROOT / model / name, prefix + name))
    return records


def credit(model):
    """Adds the model's page to the licence file, above the closing notes."""
    line = '- https://polyhaven.com/a/' + model
    lines = LICENSES.read_text().splitlines()
    if line in lines:
        return
    last = max(i for i, text in enumerate(lines) if text.startswith('- https://'))
    lines.insert(last + 1, line)
    LICENSES.write_text('\n'.join(lines) + '\n')


def main():
    records = json.loads(RECORDS.read_text())
    for model in json.loads((HERE / 'polyhaven_models.json').read_text())['models']:
        if (ROOT / model / (model + '.gltf')).exists():
            continue
        records += download(model)
        credit(model)
        print('Downloaded', model)
    RECORDS.write_text(json.dumps(records) + '\n')


if __name__ == '__main__':
    main()
