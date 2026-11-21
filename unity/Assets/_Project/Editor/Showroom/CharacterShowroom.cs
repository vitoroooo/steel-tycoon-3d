using UnityEditor;
using UnityEngine;
using Steel.Editor.Characters;
using Steel.Presentation.Characters;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet for one character: front, side, back, face and a walking pose.</summary>
    public static class CharacterShowroom
    {
        private const int Width = ShowroomStage.Width, Height = ShowroomStage.Height;

        [MenuItem("Steel/Showroom/Player")]
        public static void Player() => Render(CharacterCatalog.Player, "player");

        private static void Render(CharacterSpec spec, string name)
        {
            ShowroomStage.Create();
            GameObject character = CharacterRig.Create(spec, CharacterAnimator.Controller());
            foreach (var skin in character.GetComponentsInChildren<SkinnedMeshRenderer>())
                skin.forceMatrixRecalculationPerRender = true; // edit mode keeps the first pose otherwise
            var camera = new ShowroomCamera(Width, Height);
            int tile = Width / 5;
            Vector3 body = Vector3.up * spec.HeightMetres * .52f;
            Vector3 face = Vector3.up * spec.HeightMetres * .92f;
            Pose(character, 0, .5f);
            camera.Shoot(body, 20, 8, 4.3f, new RectInt(0, 0, tile, Height)); // first frame warms up shaders
            camera.Shoot(body, 20, 8, 4.3f, new RectInt(0, 0, tile, Height));
            camera.Shoot(body, 90, 8, 4.3f, new RectInt(tile, 0, tile, Height));
            camera.Shoot(body, 180, 8, 4.3f, new RectInt(tile * 2, 0, tile, Height));
            camera.Shoot(face, 15, 4, 1.1f, new RectInt(tile * 4, Height / 2, tile, Height / 2));
            Pose(character, CharacterAnimator.WalkSpeed, .3f);
            camera.Shoot(body, 60, 8, 4.3f, new RectInt(tile * 3, 0, tile, Height));
            Pose(character, CharacterAnimator.RunSpeed, .25f);
            camera.Shoot(body, 75, 8, 4.3f, new RectInt(tile * 4, 0, tile, Height / 2));
            camera.Save(ShowroomStage.SheetPath(name));
        }

        /// <summary>Advances the real animator, so the sheet shows what the game will play.</summary>
        private static void Pose(GameObject character, float speed, float seconds)
        {
            var animator = character.GetComponent<Animator>();
            animator.Rebind();
            animator.SetFloat("Speed", speed);
            animator.Update(0);
            animator.Update(seconds);
            character.GetComponent<HipsAnchor>().Apply();
        }
    }
}
