using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Npc;
using Steel.Editor.Plan.Places.Builders;
using Steel.Editor.Plan.Town;
using Steel.Presentation.Characters.Toon;
using Steel.Presentation.World.Risk;

namespace Steel.Editor.Plan.Places.Risk
{
    /// <summary>The people of a yard's trouble: a gang outside its gate and guards inside it, hidden until the game shows them.</summary>
    internal static class GateTroubleBuilder
    {
        private const int Gang = 6, Watch = 4;
        private static readonly ToonCitizen[] Faces = { ToonCitizen.RedShirt, ToonCitizen.Neighbour, ToonCitizen.Worker };
        private static readonly Stance[] Poses = { Stance.Stand, Stance.Talk, Stance.Squat };

        internal static void Build(Lot lot)
        {
            var trouble = new GameObject("Gerbang lapak").AddComponent<GateTrouble>();
            trouble.transform.SetParent(CityGeometry.Root, false);
            trouble.Thugs = new GameObject[Gang]; trouble.Guards = new GameObject[Watch];
            float front = lot.Depth / 2;
            for (int i = 0; i < Gang; i++)   // on the verge either side of the gateway, facing the yard; the gateway itself stays clear
                trouble.Thugs[i] = Person("Preman " + (i + 1), Faces[i % Faces.Length], Poses[i % Poses.Length],
                    lot, Beside(i), front + 1.6f + i % 2 * .5f, 180, i);
            for (int i = 0; i < Watch; i++)   // inside the fence either side of the gateway, facing the street
                trouble.Guards[i] = Person("Satpam " + (i + 1), ToonCitizen.Worker, Stance.Stand, lot, Beside(i), front - 1.6f, 0, i + Gang);
        }

        /// <summary>Alternately left and right of the gateway, each pair a step further out.</summary>
        private static float Beside(int index) => (index % 2 == 0 ? -1 : 1) * (Yards.Gate / 2 + .9f + index / 2 * 1.3f);

        private static GameObject Person(string name, ToonCitizen who, Stance stance, Lot lot, float right, float forward, float turn, int index)
        {
            Vector3 at = lot.Point(right, 0, forward);
            GameObject person = NpcBuilder.Idler(name, who, stance, Vehicles.ParkingSpot.On(at.x, at.z), lot.Yaw + turn, index * .17f % 1);
            person.SetActive(false);
            return person;
        }
    }
}
