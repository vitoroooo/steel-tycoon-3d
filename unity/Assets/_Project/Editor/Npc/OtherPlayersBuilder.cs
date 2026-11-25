using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Presentation.World;
using Steel.Presentation.World.Town;

namespace Steel.Editor.Npc
{
    /// <summary>The figures other players are shown as in the shared city: collectors like the player himself, each with
    /// a name board over his head, all hidden until the server says someone is near.</summary>
    internal static class OtherPlayersBuilder
    {
        internal const int Count = 8;
        private const float Height = 1.7f, Board = 2.15f;
        private const int IgnoreRaycastLayer = 2;

        internal static void Build()
        {
            var crowd = new GameObject("Pemain lain").AddComponent<OtherPlayers>();
            crowd.transform.SetParent(CityGeometry.Root, false);
            crowd.Ghosts = new Ghost[Count];
            for (int i = 0; i < Count; i++) crowd.Ghosts[i] = Figure(crowd.transform, i);
        }

        private static Ghost Figure(Transform crowd, int number)
        {
            var ghost = new GameObject("Pemain lain " + (number + 1)).AddComponent<Ghost>();
            GameObject body = ToonCharacter.Walker(ToonCharacter.Model, Height, out _);
            body.transform.SetParent(ghost.transform, false);
            TextMesh name = CityGeometry.Sign("Pemain", Vector3.up * Board, 1.1f, Steel.Editor.Toon.ToonPalette.Yellow);
            Object.DestroyImmediate(name.GetComponent<SignVisibility>());
            name.gameObject.AddComponent<WorldLabel>();   // turns to the camera and hides beyond 20 m
            name.transform.SetParent(ghost.transform, false);
            ghost.Label = name;
            ghost.transform.SetParent(crowd, false);
            foreach (Transform part in ghost.GetComponentsInChildren<Transform>()) part.gameObject.layer = IgnoreRaycastLayer;
            return ghost;
        }
    }
}
