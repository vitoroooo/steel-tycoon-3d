using UnityEngine;
using Steel.Editor.Plan.Places.Builders;

namespace Steel.Editor.Plan.Places
{
    /// <summary>The kinds of place Sukabaja is made of. To change how a district looks, change its sheet here.</summary>
    internal static class Styles
    {
        internal static readonly StyleSheet Tower = new StyleSheet(Towers.Build, Props.Tree, 1.5f, 5, V(24, 20), V(16, 15), V(10, 10), V(7, 7));
        internal static readonly StyleSheet Civic = new StyleSheet(Civics.Hall, Props.Tree, 1.5f, 5, V(34, 24), V(18, 14), V(9, 9));
        internal static readonly StyleSheet Mosque = new StyleSheet(Civics.Mosque, Props.Tree, 2, 5, V(26, 26), V(12, 12), V(8, 8));
        internal static readonly StyleSheet Shop = new StyleSheet(Shops.Build, Props.Stall, .3f, 4, V(18, 13), V(9, 12), V(4.6f, 9), V(4.6f, 6.5f));
        internal static readonly StyleSheet House = new StyleSheet(Houses.Kampung, Props.Garden, .4f, 3, V(9, 10), V(7, 8), V(5.5f, 6.5f));
        internal static readonly StyleSheet Estate = new StyleSheet(Houses.Estate, Props.Tree, .6f, 4, V(8, 11), V(8, 9));
        internal static readonly StyleSheet Shed = new StyleSheet(Sheds.Warehouse, Props.Crate, 1.5f, 4, V(30, 22), V(20, 15), V(11, 10), V(8, 9));
        internal static readonly StyleSheet Factory = new StyleSheet(Sheds.Factory, Props.Crate, 1.5f, 4, V(34, 24), V(20, 15), V(11, 10));
        internal static readonly StyleSheet Yard = new StyleSheet(Yards.Depot, Props.Scrap, .6f, 4, V(24, 20), V(15, 13), V(8, 8));
        internal static readonly StyleSheet Dump = new StyleSheet(Yards.Dump, Props.Scrap, .5f, 3, V(20, 18), V(11, 10));
        internal static readonly StyleSheet LapakPlot = new StyleSheet(Plots.Lapak, Props.Scrap, 1.5f, 5, V(34, 28));
        internal static readonly StyleSheet FactoryPlot = new StyleSheet(Plots.Factory, Props.Crate, 1.5f, 5, V(44, 34), V(30, 24));
        internal static readonly StyleSheet Park = new StyleSheet(Greens.Park, Props.Tree, .5f, 4, V(14, 14), V(8, 8));
        internal static readonly StyleSheet Pitch = new StyleSheet(Greens.Pitch, Props.Tree, 1, 4, V(40, 26), V(14, 12));
        internal static readonly StyleSheet Forest = new StyleSheet(Greens.Park, Props.Tree, 0, 3);
        internal static readonly StyleSheet Farm = new StyleSheet(Greens.Field, Props.Garden, .4f, 4, V(20, 15), V(10, 9));
        internal static readonly StyleSheet Pond = new StyleSheet(Greens.Pond, Props.Tree, 1, 4, V(30, 22), V(14, 12));
        internal static readonly StyleSheet Hill = new StyleSheet(Greens.Mound, Props.Tree, .2f, 4, V(26, 26), V(14, 14), V(7, 7));

        private static Vector2 V(float frontage, float depth) => new Vector2(frontage, depth);
    }
}
