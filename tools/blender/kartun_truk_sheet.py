"""Renders one design view of a heavy truck.

Usage: blender --background --python tools/blender/kartun_truk_sheet.py -- trailer out.png [yaw]
"""
import sys
from pathlib import Path
import bpy
sys.path.insert(0, str(Path(__file__).resolve().parent))
from kartun import paint, studio  # noqa: E402
from kartun.unity import truk  # noqa: E402

THIN = ('Nama usaha', 'Kisi gril', 'Wiper', 'Garis pintu')
FRAMING = {'engkel': ((0, 0, 1.4), 21), 'tronton': ((0, .2, 1.5), 27), 'trailer': ((0, 2.6, 1.6), 40)}

variant, out = sys.argv[sys.argv.index('--') + 1:][:2]
yaw = float(sys.argv[-1]) if sys.argv[-1] != out else 42
bpy.ops.wm.read_factory_settings(use_empty=True)
scene = bpy.context.scene
parts = [part for node in truk.nodes(variant) for part in node.parts]
graph = bpy.context.evaluated_depsgraph_get()
print('TRIANGLES', variant, sum(len(p.vertices) - 2 for part in parts
                                for p in part.evaluated_get(graph).to_mesh().polygons))
for part in parts:
    paint.outline(part, .008 if part.name.split('.')[0] in THIN else .016)
studio.light(scene)
studio.floor(scene)
target, distance = FRAMING[variant]
studio.camera(scene, target, distance, yaw, 13)
studio.render(out, 1500, 820, 32)
