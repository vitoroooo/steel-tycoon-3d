using System.Globalization;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private static string Days(double value) =>
            value.ToString("0.#", CultureInfo.GetCultureInfo("id-ID")) + " hari nyata";

        private void DrawFinance()
        {
            var f = game.Finances();
            detail.text = "Level usaha " + f.Level + "/50   ·   Laba seumur hidup " + Rp(f.Lifetime) +
                "\nLaba kuartal " + Rp(f.Profit) + "   ·   Tagihan " + f.Rate + "% " + Rp(f.TaxEstimate) +
                " dalam " + Days(f.DaysToTax) + "\nUtang koperasi " + Rp(f.Debt) + "   ·   Aset " + Rp(f.Assets);
            Key(0, "Rincian koperasi", () => feedback.text = f.Debt > 0
                ? "Bunga 2% dalam " + Days(f.DaysToInterest) + ". Utang >50% aset memicu sitaan harian."
                : "Utang nihil. Pajak terakhir " + Rp(f.LastTax) + ".");
            if (f.ReplaceScale) Key(1, "Timbangan · " + Rp(f.ScalePrice), () => game.ReplaceYardTool(false));
            if (f.ReplaceLoader) Key(2, "Loader · " + Rp(f.LoaderPrice), () => game.ReplaceYardTool(true));
            if (!string.IsNullOrEmpty(f.Notice) && feedback.text == "") feedback.text = f.Notice;
        }
    }
}
