using UnityEngine;
using Steel.Services;
using Steel.Presentation.Player;

namespace Steel.Editor
{
    internal static class WorkerBuilder
    {
        private static Transform Part(Transform parent, string name, PrimitiveType type, Vector3 p, Vector3 scale, Material mat)
        {
            var go = CityGeometry.Shape(name, type, Vector3.zero, scale, mat, false);
            go.isStatic = false; go.transform.SetParent(parent, false); go.transform.localPosition = p; go.layer = 2;
            return go.transform;
        }
        internal static Transform Create(CityLayout layout)
        {
            var root = new GameObject("Pekerja"); root.layer = 2;
            root.AddComponent<Steel.Presentation.World.FallRecovery>();
            root.transform.position = new Vector3(layout.Spawn.X, .25f, layout.Spawn.Z);
            var controller = root.AddComponent<CharacterController>(); controller.height = 1.8f;
            controller.center = Vector3.up * .9f; controller.radius = .3f; controller.stepOffset = .3f;
            var mover = root.AddComponent<PlayerMover>();
            var visual = new GameObject("Seragam").transform; visual.SetParent(root.transform, false); mover.Visual = visual;
            var vest = CityMaterials.Plain("Rompi keselamatan", new Color(1, .3f, .045f), .1f, .35f);
            var cloth = CityMaterials.Plain("Seragam pekerja", new Color(.055f, .12f, .16f), 0, .3f);
            var skin = CityMaterials.Plain("Kulit", new Color(.45f, .27f, .16f), 0, .3f);
            var helmet = CityMaterials.Plain("Helm keselamatan", new Color(.95f, .67f, .08f), .15f, .6f);
            var reflective = CityMaterials.Plain("Strip reflektif", new Color(.8f, .85f, .68f), .5f, .7f);
            Part(visual, "Badan", PrimitiveType.Capsule, new Vector3(0, 1.12f, 0), new Vector3(.48f, .38f, .3f), vest);
            Part(visual, "Kepala", PrimitiveType.Sphere, new Vector3(0, 1.63f, 0), new Vector3(.28f, .32f, .28f), skin);
            Part(visual, "Helm", PrimitiveType.Sphere, new Vector3(0, 1.79f, 0), new Vector3(.36f, .18f, .36f), helmet);
            Part(visual, "Pinggir helm", PrimitiveType.Cylinder, new Vector3(0, 1.75f, 0), new Vector3(.4f, .018f, .4f), helmet);
            for (int side = -1; side <= 1; side += 2)
                Part(visual, "Reflektor", PrimitiveType.Cube, new Vector3(side * .13f, 1.18f, -.154f),
                    new Vector3(.045f, .38f, .015f), reflective);
            var gait = visual.gameObject.AddComponent<WorkerGait>();
            gait.LeftArm = Limb(visual, new Vector3(-.31f, 1.32f, 0), cloth);
            gait.RightArm = Limb(visual, new Vector3(.31f, 1.32f, 0), cloth);
            gait.LeftLeg = Limb(visual, new Vector3(-.13f, .73f, 0), cloth);
            gait.RightLeg = Limb(visual, new Vector3(.13f, .73f, 0), cloth);
            return root.transform;
        }
        private static Transform Limb(Transform parent, Vector3 position, Material mat)
        {
            var pivot = new GameObject("Sendi").transform; pivot.SetParent(parent, false); pivot.localPosition = position;
            Part(pivot, "Anggota badan", PrimitiveType.Capsule, Vector3.down * .25f, new Vector3(.17f, .3f, .17f), mat);
            return pivot;
        }
    }
}
