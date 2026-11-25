using UnityEngine;
using Steel.Core.Trade.Street;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Npc;
using Steel.Editor.Plan.Town;
using Steel.Presentation.Characters.Toon;
using Steel.Presentation.World.Street;

namespace Steel.Editor.Plan.Places.Doors
{
    /// <summary>What a player sees where scrap is sold at the door: a weighing post, a board, the seller and the goods.</summary>
    internal static class DoorStall
    {
        internal static readonly Vector2[] Size = { new Vector2(5, 4) };
        private static readonly ToonCitizen[] Sellers =
            { ToonCitizen.Shopper, ToonCitizen.RedShirt, ToonCitizen.Worker, ToonCitizen.Neighbour };

        internal static void Build(TownKit kit, Lot lot, SellerKind kind, int place, DoorLedger ledger)
        {
            Vector3 stand = Ground(lot.Point(-.9f, 0, 1.1f)), heap = Ground(lot.Point(1.1f, 0, .2f));
            string id = ledger.Add(stand, kind, place);
            Scale(kit, lot);
            Shapes.Box(kit.Trim, lot, Tone.Navy, new Vector3(-1.7f, 1.5f, 1.7f), new Vector3(1.5f, .5f, .08f), Tone.Yellow);
            CityGeometry.Sign("JUAL ROSOK", lot.Point(-1.7f, 1.95f, 1.76f), .2f, Tones.Colour(Tone.Yellow), lot.Yaw + 180);
            Shapes.Drum(kit.Solid, Tone.Slate, lot.Point(-1.7f, Lot.Foot, 1.7f), .06f, .06f, 1.9f, 5);
            NpcBuilder.Idler("Penjual " + id, Sellers[(int)kind], Stance.Stand, stand, lot.Yaw, lot.Roll(7));
            var marker = new GameObject("Rongsok " + id).AddComponent<SellerMarker>();
            marker.transform.SetParent(CityGeometry.Root, false);
            marker.transform.SetPositionAndRotation(heap, Quaternion.Euler(0, lot.Yaw, 0));
            marker.Id = id;
            DoorGoods.Fill(marker.transform, kind, lot);
        }

        /// <summary>A stall can reach onto the pavement, so people and goods stand on whatever is really under them.</summary>
        private static Vector3 Ground(Vector3 at) => Vehicles.ParkingSpot.On(at.x, at.z);

        /// <summary>A hanging scale: a post, an arm and a yellow dial with a hook plate under it.</summary>
        private static void Scale(TownKit kit, Lot lot)
        {
            Shapes.Drum(kit.Solid, Tone.Wood, lot.Point(.1f, Lot.Foot, 1.4f), .07f, .07f, 2.5f, 5);
            Shapes.Box(kit.Trim, lot, Tone.Wood, new Vector3(.5f, 2.2f, 1.4f), new Vector3(1, .08f, .08f));
            Shapes.Drum(kit.Trim, Tone.Yellow, lot.Point(.9f, 1.65f, 1.4f), .16f, .16f, .34f, 8);
            Shapes.Drum(kit.Trim, Tone.Slate, lot.Point(.9f, 1.2f, 1.4f), .26f, .26f, .04f, 8);
        }
    }
}
