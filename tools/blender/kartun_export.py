"""Exports one cartoon model for Unity at phone density.

Usage: blender --background --python tools/blender/kartun_export.py -- pengepul
Assets: pengepul, gerobak, triseda (packages under kartun/unity).
"""
import importlib
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
MODELS = HERE.parents[1] / 'unity/Assets/_Project/Models'
PHONE_DETAIL = .5
sys.path.insert(0, str(HERE))

import bpy  # noqa: E402
from kartun import detail  # noqa: E402  (needs the path above)
from kartun.unity import rig  # noqa: E402


def main():
    asset = sys.argv[sys.argv.index('--') + 1]
    detail.LEVEL = PHONE_DETAIL
    bpy.ops.wm.read_factory_settings(use_empty=True)
    module = importlib.import_module('kartun.unity.' + asset)
    folder, name = module.TARGET
    rig.assemble(name, module.nodes())
    target = MODELS / folder
    target.mkdir(parents=True, exist_ok=True)
    rig.export(target / (name + '.fbx'))


main()
