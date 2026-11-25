using Steel.Core.Economy;
using Steel.Core.Scrap;

namespace Steel.Services
{
    public partial interface IGameService
    {
        double GameHours { get; }
        AiQuote Prices { get; }
        ScrapPile CreatePile(double kilograms, ScrapGrade grade);
        void Advance(double realSeconds);
        double CatchUp(double elapsedRealSeconds);
        double Quote(ScrapPile pile);
    }
}
