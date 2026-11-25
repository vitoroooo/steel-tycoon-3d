using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Models;
using Steel.Editor.Plan.Town;
using Steel.Editor.Vehicles;
using Steel.Presentation.Characters;
using Steel.Presentation.Characters.Toon;
using Steel.Presentation.Vehicles;
using Steel.Presentation.Vehicles.Haul;

namespace Steel.Editor.Plan.Places.Haul
{
    /// <summary>The yard's 3-ton floor scale with its readout, and the collector who pushes a cart onto it when the yard buys.</summary>
    internal static class YardCallerBuilder
    {
        private const float ScaleX = -3.5f, ScaleZ = 2, Deck = 2.6f, CartAhead = 1.72f;

        internal static void Build(TownKit kit, Lot lot, Transform works)
        {
            Shapes.Flat(kit.Trim, lot, Tone.Yellow, ScaleX, ScaleZ, Deck + .3f, Deck + .3f, Lot.Floor + .02f);
            Shapes.Flat(kit.Trim, lot, Tone.Slate, ScaleX, ScaleZ, Deck, Deck, Lot.Floor + .04f);
            Shapes.Drum(kit.Solid, Tone.Slate, lot.Point(ScaleX - 2, Lot.Foot, ScaleZ), .07f, .07f, 2.2f, 5);
            Shapes.Box(kit.Trim, lot, Tone.Navy, new Vector3(ScaleX - 2, 1.5f, ScaleZ), new Vector3(.14f, .9f, 2.2f), Tone.Yellow);
            var caller = works.gameObject.AddComponent<YardCaller>();
            caller.Readout = CityGeometry.Sign("TIMBANGAN\n3 TON", lot.Point(ScaleX - 1.9f, 1.95f, ScaleZ), .2f,
                Tones.Colour(Tone.Yellow), lot.Yaw - 90);
            caller.Gate = new Vector3(-1.5f, 0, lot.Depth / 2 + 1); caller.Scale = new Vector3(ScaleX, 0, ScaleZ - CartAhead / 2);
            var collector = new GameObject("Pengepul setoran").transform;
            collector.SetParent(works, false); caller.Collector = collector;
            GameObject body = ToonCharacter.Walker(ToonCitizen.RedShirt.Model, ToonCitizen.RedShirt.HeightMetres, out ToonJoints joints);
            ToonLimbs.Bind(body, joints);
            body.transform.SetParent(collector, false);
            caller.Arms = body.GetComponent<ArmReach>();
            GameObject cart = AuthoredModel.Create("Vehicles/GerobakKartun", _ => ToonCast.Material);
            cart.name = "Gerobak setoran"; cart.transform.SetParent(collector, false);
            cart.transform.localPosition = new Vector3(0, 0, CartAhead);
            cart.transform.localRotation = Quaternion.Euler(CartPusher.LiftDegrees, 0, 0);
            caller.WheelLeft = cart.transform.Find("Wheel_L"); caller.WheelRight = cart.transform.Find("Wheel_R");
            caller.GripLeft = GerobakBuilder.Grip(cart.transform, -1); caller.GripRight = GerobakBuilder.Grip(cart.transform, 1);
        }
    }
}
