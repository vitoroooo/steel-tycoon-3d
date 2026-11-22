using Steel.Services.Map;

namespace Steel.Editor.Plan.Places
{
    /// <summary>Reads a place's name on the plan and says how to build it. The first word that matches wins, so order matters.</summary>
    internal static class StyleBook
    {
        private static readonly (string Word, StyleSheet Sheet)[] Words =
        {
            ("Kavling pabrik", Styles.FactoryPlot), ("Kavling lapak", Styles.LapakPlot), ("Kavling", Styles.Shed),
            ("Bengkel motor", Styles.Shop), ("Pasar", Styles.Shop), ("Pecinan", Styles.Shop), ("Kuliner", Styles.Shop),
            ("Dealer roda", Styles.Shop), ("SPBU", Styles.Shop), ("Toko", Styles.Shop), ("Rest area", Styles.Shop),
            ("Lapak awal", Styles.Shop),
            ("Apartemen", Styles.Tower), ("Hotel", Styles.Tower), ("Perkantoran", Styles.Tower), ("Rusun", Styles.Tower),
            ("Kantor", Styles.Tower),
            ("Masjid", Styles.Mosque), ("Musholla", Styles.House),
            ("Mal", Styles.Civic), ("Balai", Styles.Civic), ("Stasiun", Styles.Civic), ("Sekolah", Styles.Civic),
            ("RSUD", Styles.Civic), ("Politeknik", Styles.Civic), ("Pos polisi", Styles.Civic),
            ("Perumahan", Styles.Estate), ("Cluster", Styles.Estate),
            ("Kampung", Styles.House), ("Rumah", Styles.House), ("Gang", Styles.House),
            ("PT BAJA", Styles.Factory), ("Pabrik", Styles.Factory), ("Peleburan", Styles.Factory),
            ("Gudang", Styles.Shed), ("Bengkel", Styles.Shed), ("karoseri", Styles.Shed), ("Hanggar", Styles.Shed),
            ("Dealer truk", Styles.Shed), ("Gardu", Styles.Shed),
            ("TPA", Styles.Dump), ("Kuburan", Styles.Dump), ("Bank sampah", Styles.Dump), ("Bongkaran", Styles.Dump),
            ("Pool", Styles.Yard), ("Pangkalan", Styles.Yard), ("Proyek", Styles.Yard), ("Lapak", Styles.Yard),
            ("Terminal", Styles.Yard), ("Pos timbang", Styles.Yard), ("Gerbang Tol", Styles.Yard),
            ("Lapangan", Styles.Pitch), ("Taman", Styles.Park), ("Hutan", Styles.Forest),
            ("Sawah", Styles.Farm), ("Kebun", Styles.Farm),
            ("Situ", Styles.Pond), ("Kolam", Styles.Pond),
            ("Bukit", Styles.Hill)
        };

        internal static string Title(MapPlace place) => (place.name + " " + place.detail.Split('&', ',')[0]).Trim().ToUpperInvariant();

        internal static StyleSheet Of(MapPlace place)
        {
            string title = place.name + " " + place.detail;
            foreach ((string word, StyleSheet sheet) in Words)
                if (title.Contains(word)) return sheet;
            return Styles.House;
        }
    }
}
