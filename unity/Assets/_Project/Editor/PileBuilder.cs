using UnityEngine;
using Steel.Services;
using Steel.Presentation.World;

namespace Steel.Editor
{
    internal static class PileBuilder
    {
        internal static void Create(CityLayout layout)
        {
            var rust = CityMaterials.Surface("rusty_metal_04", new Vector2(2, 1));
            var iron = CityMaterials.Surface("corrugated_iron", new Vector2(4, 2));
            foreach (var pile in layout.Piles)
            {
                var root = new GameObject(pile.Id); root.transform.SetParent(CityGeometry.Root);
                root.transform.position = new Vector3(pile.Position.X, 0, pile.Position.Z);
                root.AddComponent<PileMarker>().Id = pile.Id;
                var label = new GameObject("Rongsok"); label.transform.SetParent(root.transform, false);
                label.transform.localPosition = Vector3.up * 1.1f;
                var text = label.AddComponent<TextMesh>(); text.text = "RONGSOK";
                text.anchor = TextAnchor.MiddleCenter; text.fontSize = 64; text.characterSize = .09f;
                text.color = new Color(1, .8f, .4f); label.AddComponent<WorldLabel>();
                var random = new System.Random(71 + int.Parse(pile.Id.Substring(1)));
                for (int i = 0; i < 18; i++)
                {
                    Vector3 p = root.transform.position + new Vector3((float)random.NextDouble() * 1.5f - .75f,
                        .2f + i * .018f, (float)random.NextDouble() - .5f);
                    var go = CityGeometry.Box("Potongan baja", p,
                        new Vector3(.15f + (float)random.NextDouble() * .3f, .06f, .6f + (float)random.NextDouble()),
                        i % 3 == 0 ? iron : rust, false);
                    go.transform.rotation = Quaternion.Euler(i * 13, i * 71, i * 5);
                    go.transform.SetParent(root.transform);
                }
            }
        }
    }
}
