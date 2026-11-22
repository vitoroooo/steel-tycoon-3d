using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Plan
{
    /// <summary>Builds what runs through greater Sukabaja from the baked plan: streets, the roundabout, the river and the railway.</summary>
    internal static class PlanBuilder
    {
        internal static MapPlan Plan { get; private set; }

        internal static void Create()
        {
            Plan = MapPlan.Load();
            Strip.Parent = new GameObject("Jalan dan kali").transform;
            Strip.Parent.SetParent(CityGeometry.Root, false);
            var paving = new Roads.Paving(Plan);
            var kit = new Roads.RoadKit();
            Roads.RoadBuilder.Create(paving, kit);
            foreach (MapRoundabout ring in Plan.roundabouts) Roads.Roundabout.Create(ring, kit);
            foreach (WayLine river in paving.Rivers) Water.RiverBuilder.Create(river, paving, kit);
            foreach (WayLine rail in paving.Rails) Rail.RailBuilder.Create(rail, paving, kit);
            kit.Build();
            Places.Builders.Plots.Reset();
            Places.PlaceFiller.Create(Plan);
            Debug.Log("PLAN_BUILT: " + paving.Roads.Count + " streets, " + Strip.Parent.childCount + " sector meshes");
        }
    }
}
