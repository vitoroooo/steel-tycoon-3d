using Steel.Core.Season;

namespace Steel.Services
{
    /// <summary>What each legacy does to a career, in the player's words.</summary>
    public static class LegacyText
    {
        public static string Effect(string id)
        {
            switch (id)
            {
                case LegacyRules.Triseda: return "triseda sudah di dealer atas nama Anda";
                case LegacyRules.Reputation: return "reputasi awal 15";
                case LegacyRules.Sorting: return "rute kumpul-sortir 10% lebih cepat";
                case LegacyRules.Queue: return "antrean offline +2 jam game";
                case LegacyRules.Company: return "izin usaha sudah terdaftar";
                default: return id;
            }
        }
    }
}
