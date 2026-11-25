using UnityEngine;

namespace Steel.Editor.Map
{
    /// <summary>Where the seng fences run: they close yards and give the edge of town something to look at.</summary>
    internal static class Fences
    {
        /// <summary>Each run as from-x, from-z, to-x, to-z; the good side faces left of travel.</summary>
        private static readonly Vector4[] Runs =
        {
            new Vector4(-53, -3, -53, 60),        // behind the kampung back yards, clear of Jl. Baja Raya
            new Vector4(-53, 60, -41.6f, 60),     // north end of the kampung
            new Vector4(-33, 21.5f, -26, 21.5f),  // south end of the western ruko row, leaving the alley mouth open
            new Vector4(53, 54, 53, 34),          // east edge, north half, up to Jl. Tepi Kali
            new Vector4(53, 34, 53, 4),           // east edge, beside the container depot
            new Vector4(22, 36, 36, 36),          // between the factory and the northern row's yard
        };

        internal static void Create()
        {
            foreach (Vector4 run in Runs)
                SengFence.Run(new Vector3(run.x, 0, run.y), new Vector3(run.z, 0, run.w));
        }
    }
}
