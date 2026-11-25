using Steel.Core.Scrap;

namespace Steel.Services
{
    /// <summary>The last collector who sold to the player's yard: what the scale read and what was paid.</summary>
    public struct YardVisit
    {
        /// <summary>Goes up by one with every lot bought; 0 until the first.</summary>
        public long Serial;
        public string Grade;
        public double Kg, Paid;
    }

    public partial interface IGameService
    {
        YardVisit LastVisit { get; }
    }

    public sealed partial class LocalGameService
    {
        public YardVisit LastVisit => new YardVisit
        {
            Serial = session.LastReceipt.Serial, Kg = session.LastReceipt.Kg, Paid = session.LastReceipt.Paid,
            Grade = ScrapText.Grade((ScrapGrade)session.LastReceipt.Grade)
        };
    }
}
