using UnityEngine;

namespace Steel.Editor.Characters.Toon
{
    /// <summary>The one material shared by every authored cartoon model: colours live in the vertices.</summary>
    internal static class ToonCast
    {
        private const float LineMetres = .011f;

        internal static Material Material => CityMaterials.Outlined("Kartun pemeran", Color.white, LineMetres);
    }
}
