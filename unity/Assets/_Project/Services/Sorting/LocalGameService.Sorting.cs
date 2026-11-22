using Steel.Core.Scrap;
using Steel.Core.State;

namespace Steel.Services
{
    public partial interface IGameService
    {
        string SampleClue(string id);
        string InspectMagnet(string id);
        string Classify(string id, int bin, bool scan);
        void BuyScanner(bool xrf);
        bool OwnScanner(bool xrf);
        double ScannerCost(bool xrf);
    }
    public sealed partial class LocalGameService
    {
        private readonly ConfigValues business;
        private static readonly string[] Clues = {
            "Pelat baja tebal, tepinya bersih", "Lembaran seng tipis dan bengkok",
            "Blok mesin cor, permukaan berpori", "Potongan besi kecil bercampur",
            "Baut utuh; ulir masih bisa dipakai", "Segumpal tanah dan plastik" };
        public string SampleClue(string id) => Clues[session.MaterialIndex(id)];
        public bool OwnScanner(bool xrf) => xrf ? session.Xrf : session.Magnet;
        public double ScannerCost(bool xrf) => business.Positive(xrf ? "xrf" : "magnet");
        public void BuyScanner(bool xrf) => session.BuyScanner(xrf, ScannerCost(xrf));
        public string InspectMagnet(string id) => session.Magnetic(id)
            ? "Menempel. Pilih grade dari bentuk dan kondisi." : "Tidak menempel. Periksa bak kotoran.";
        public string Classify(string id, int bin, bool scan)
        {
            var grade = (ScrapGrade)session.MaterialIndex(id);
            double waste = session.Classify(id, bin, scan);
            return ScrapText.Grade(grade) + " · " + waste.ToString("0.##") +
                (id == null ? " kg kotoran dipisahkan." : scan ? " kg kotoran terdeteksi." : " kg estimasi kotoran.");
        }
    }
}
