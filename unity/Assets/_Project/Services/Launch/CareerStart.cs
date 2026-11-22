namespace Steel.Services
{
    /// <summary>Opens the career the start menu asked for: the saved one, or a new one in its place.</summary>
    public sealed class CareerStart
    {
        public const string KeptNotice = "Karier lama tidak bisa diganti. Periksa ruang penyimpanan.";
        public CareerFiles Files { get; private set; }
        public LocalGameService Game { get; private set; }
        /// <summary>Seconds the resumed career was away; zero for a new one.</summary>
        public double Elapsed { get; private set; }
        /// <summary>True when the saved career was set aside and Game is a new one that still has to be written.</summary>
        public bool Replaced { get; private set; }
        /// <summary>Set when the player asked to start over but the old career could not be moved.</summary>
        public string Notice { get; private set; }

        public static CareerStart Open(string folder, IConfigSource config, long now, bool fresh)
        {
            var start = new CareerStart { Files = new CareerFiles(folder) };
            start.Replaced = fresh && start.Files.Retire();
            start.Game = start.Files.Open(config, now, out double elapsed);
            start.Elapsed = elapsed;
            if (fresh && !start.Replaced) start.Notice = KeptNotice;
            return start;
        }
    }
}
