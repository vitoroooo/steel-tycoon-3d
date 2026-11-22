using Steel.Core.Trade.Street;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places.Doors
{
    /// <summary>Reads a place's name on the plan and says who sells scrap at a door there, if anyone. The first word that matches wins.</summary>
    internal static class DoorBook
    {
        private static readonly (string Word, SellerKind Kind)[] Words =
        {
            ("Proyek", SellerKind.Project), ("Bongkaran", SellerKind.Project), ("Kuburan mobil", SellerKind.Project),
            ("Bengkel", SellerKind.Workshop), ("bengkel", SellerKind.Workshop), ("karoseri", SellerKind.Workshop),
            ("Pabrik paku", SellerKind.Workshop),
            ("Pasar", SellerKind.Market), ("Pecinan", SellerKind.Market), ("Toko bahan", SellerKind.Market),
            ("ruko onderdil", SellerKind.Market),
            ("Kampung", SellerKind.House), ("Rumah", SellerKind.House), ("rumah", SellerKind.House), ("Gang", SellerKind.House),
            ("Perumahan", SellerKind.House), ("Cluster", SellerKind.House), ("Rusun", SellerKind.House)
        };

        internal static SellerKind? Of(MapPlace place)
        {
            string title = place.name + " " + place.detail;
            foreach ((string word, SellerKind kind) in Words)
                if (title.Contains(word)) return kind;
            return null;
        }
    }
}
