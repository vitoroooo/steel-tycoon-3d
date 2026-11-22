using UnityEngine;
using Steel.Editor.Characters;
using Steel.Editor.Npc;

namespace Steel.Editor.Map
{
    /// <summary>Who is out in Sukabaja and where: the one place to edit when a street feels empty.</summary>
    internal static class Townsfolk
    {
        private const float Pavement = StreetBuilder.PavementTop;   // top of the main road's sidewalks

        internal static void Create()
        {
            var men = CharacterAnimator.Controller();
            // Walkers: every other waypoint is a stop.
            NpcBuilder.Walker("Ibu belanja", CharacterCatalog.Shopper, CharacterAnimator.Female(), 1.15f,
                P(-9, 48, Pavement), P(-9.6f, 30, Pavement), P(-9, 12, Pavement), P(-8.4f, 30, Pavement));
            NpcBuilder.Walker("Warga gang pasar", CharacterCatalog.Neighbour, men, 1.3f,
                P(-38.6f, 55), P(-38, 25), P(-38.6f, -3), P(-39.2f, 25));
            NpcBuilder.Walker("Mandor pabrik", CharacterCatalog.Worker, men, 1.35f,
                P(25, -33), P(30, -33.2f), P(35.5f, -33), P(30, -34));
            // Pasar Lama: two men chatting, a shopper looking at a window.
            NpcBuilder.Idler("Ngobrol / bicara", CharacterCatalog.RedShirt, "m_gestic_talk_relaxed_01", P(-10.8f, 36, Pavement), 180, .1f);
            NpcBuilder.Idler("Ngobrol / dengar", CharacterCatalog.Neighbour, "m_idle_look_around_01", P(-10.8f, 34.9f, Pavement), 0, .55f);
            NpcBuilder.Idler("Ibu lihat etalase", CharacterCatalog.Shopper, "f_idle_neutral_01", P(-11.4f, 22, Pavement), 270, .3f);
            // The market lane: a man on a bench outside a shop, another squatting by the road.
            Bench(P(-36.3f, 20), 90);
            NpcBuilder.Idler("Bapak duduk", CharacterCatalog.RedShirt, "m_sit_chair_idle_look_around", P(-36.3f, 20), 270, .4f);
            NpcBuilder.Idler("Jongkok pinggir jalan", CharacterCatalog.Neighbour, "m_crouch_idle", P(10.3f, -1.5f, Pavement), 270, .7f);
            NpcBuilder.Idler("Jongkok gang pasar", CharacterCatalog.RedShirt, "m_crouch_idle", P(-36.6f, 6), 250, .2f);
            // The smelter: a hand watching the weighbridge, another strapping billets.
            NpcBuilder.Idler("Petugas timbang", CharacterCatalog.Worker, "m_idle_look_around_01", P(23.5f, -28.6f), 300, .8f);
            NpcBuilder.Idler("Pekerja billet", CharacterCatalog.Worker, "m_work_mid", P(30.5f, -26.4f), 0, .45f);
        }

        private static Vector3 P(float x, float z, float y = 0) => new Vector3(x, y, z);

        /// <summary>A plank bench; yaw turns its length away from the x axis.</summary>
        private static void Bench(Vector3 at, float yaw)
        {
            var wood = CityMaterials.Surface("weathered_planks", new Vector2(1, .3f), "Bangku kayu");
            var bench = new GameObject("Bangku").transform;
            bench.SetParent(CityGeometry.Root, false); bench.SetPositionAndRotation(at, Quaternion.Euler(0, yaw, 0));
            CityGeometry.Box("Bangku / dudukan", new Vector3(0, .43f, 0), new Vector3(1.4f, .05f, .36f), wood).transform.SetParent(bench, false);
            foreach (int side in new[] { -1, 1 })
                CityGeometry.Box("Bangku / kaki", new Vector3(side * .55f, .2f, 0), new Vector3(.06f, .41f, .32f), wood).transform.SetParent(bench, false);
        }
    }
}
