using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Models;
using Steel.Editor.Npc;
using Steel.Presentation.Characters.Toon;

namespace Steel.Editor.Map
{
    /// <summary>Everything standing round the wreck: the excavator, the remains next door, salvage for resale, the office, the crew.</summary>
    internal static class DemolitionYard
    {
        internal static void Create()
        {
            Excavator(new Vector3(-13.2f, 0, -43), 270);          // its breaker reaches the foot of the rubble
            Remains(new Vector3(-42, 0, -60), 8);
            ContainerDepot.Stack(new Vector3(-13.5f, 0, -27), 0, 1, 2);   // the site office
            var salvage = Solid.Stand(AuthoredModel.Create("Props/BahanBekas", DemolitionMaterials.Paint), new Vector3(-34, 0, -25), 0);
            Solid.Box(salvage, new Vector3(-1.95f, .5f, .05f), new Vector3(2.5f, 1, 1.1f));   // brick pallets
            Solid.Box(salvage, new Vector3(1, 1, 0), new Vector3(2.2f, 2, 1));                // door rack
            var bars = AuthoredModel.Create("Props/TumpukanSkrap", SmelterMaterials.Paint);   // reinforcement already pulled out
            Solid.Stand(bars, new Vector3(-46, 0, -36), 70, .55f);
            Solid.Box(bars, Vector3.up, new Vector3(7, 2, 5));
            NpcBuilder.Idler("Kuli besi beton", ToonCitizen.Worker, Stance.Work, new Vector3(-43.6f, 0, -38.2f), 300, .15f);
            NpcBuilder.Idler("Kuli istirahat", ToonCitizen.Neighbour, Stance.Squat, new Vector3(-21.5f, 0, -35), 200, .6f);
            NpcBuilder.Idler("Mandor bongkaran", ToonCitizen.Worker, Stance.Stand, new Vector3(-15.5f, 0, -38.5f), 215, .35f);
        }

        private static void Excavator(Vector3 at, float yaw)
        {
            var machine = Solid.Stand(AuthoredModel.Create("Props/Ekskavator", DemolitionMaterials.Paint), at, yaw);
            Solid.Box(machine, new Vector3(0, 1.3f, -.4f), new Vector3(2.7f, 2.6f, 4.3f));
            Solid.Box(machine, new Vector3(.3f, 1.2f, 6.2f), new Vector3(.6f, 2.4f, 1));   // the stick and breaker, at walking height
        }

        /// <summary>Knee-high walls round a broken floor; the gap in the north wall is the way in.</summary>
        private static void Remains(Vector3 at, float yaw)
        {
            const float half = 4.5f, height = .9f;
            var remains = Solid.Stand(AuthoredModel.Create("Buildings/Pondasi", DemolitionMaterials.Paint), at, yaw);
            Solid.Box(remains, new Vector3(0, height / 2, -half), new Vector3(half * 2, height, .2f));
            Solid.Box(remains, new Vector3(-3, height / 2, half), new Vector3(3, height, .2f));
            Solid.Box(remains, new Vector3(2.625f, height / 2, half), new Vector3(3.75f, height, .2f));
            foreach (int side in new[] { -1, 1 })
                Solid.Box(remains, new Vector3(side * half, height / 2, 0), new Vector3(.2f, height, half * 2));
        }
    }
}
