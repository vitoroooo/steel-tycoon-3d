using Steel.Core.Scrap;

namespace Steel.Services
{
    internal static class ScrapText
    {
        internal static string Grade(ScrapGrade grade)
        {
            switch (grade)
            {
                case ScrapGrade.Hms1: return "HMS 1";
                case ScrapGrade.Hms2: return "HMS 2";
                case ScrapGrade.CastIron: return "Besi cor";
                case ScrapGrade.Mixed: return "Campur";
                case ScrapGrade.Reuse: return "Layak pakai";
                default: return "Kotoran";
            }
        }
    }
}
