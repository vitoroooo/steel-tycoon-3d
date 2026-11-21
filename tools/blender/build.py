"""Builds one authored model and exports it for Unity.

Usage: blender --background --python tools/blender/build.py -- gerobak
Each asset is a package next to this file exposing build() -> root object name.
"""
import importlib
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
MODELS = HERE.parents[1] / 'unity/Assets/_Project/Models'
sys.path.insert(0, str(HERE))

from kit import scene  # noqa: E402  (needs the path above)


def main():
    asset = sys.argv[sys.argv.index('--') + 1]
    module = importlib.import_module(asset + '.build')
    scene.reset()
    folder, name = module.build()
    target = MODELS / folder
    target.mkdir(parents=True, exist_ok=True)
    scene.export(target / (name + '.fbx'))


main()
