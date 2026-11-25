using UnityEngine;
using Steel.Services;
using Steel.Presentation.World;

namespace Steel.Editor
{
    internal static class PileBuilder
    {
        internal static void Create(CityLayout layout)
        {
            foreach (var pile in layout.Piles)
            {
                var root = new GameObject(pile.Id); root.transform.SetParent(CityGeometry.Root);
                Vector3 p = new Vector3(pile.Position.X, .15f, pile.Position.Z);
                root.AddComponent<PileMarker>().Id = pile.Id;
                ModelLibrary.Place("barrel_03", p + new Vector3(-.5f, 0, -.15f), .95f, 15, root.transform);
                ModelLibrary.Place("old_tyre", p + new Vector3(.65f, 0, -.1f), .32f, 20, root.transform);
                ModelLibrary.Place("wooden_crate_01", p + new Vector3(-.3f, 0, .8f), .65f, -12, root.transform);
                for (int i = 0; i < 3; i++)
                {
                    var pipe = ModelLibrary.Place("modular_industrial_pipes_01",
                        p + new Vector3(.25f + i * .25f, .1f + i * .08f, .4f), .24f, i * 17, root.transform,
                        "modular_industrial_pipes_01_pipe0" + (i + 1));
                    pipe.transform.Rotate(0, 0, i * 5);
                    SheetMesh.Place(p + new Vector3(.15f, .12f + i * .14f, -.3f),
                        new Vector3(1.15f, 1, .8f), Quaternion.Euler(i * 8, i * 53, 8), root.transform);
                }
            }
        }
    }
}
