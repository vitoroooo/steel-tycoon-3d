namespace Steel.Services
{
    public enum CareerState { Empty, Saved, Unreadable }

    /// <summary>The saved career as the start menu needs it, without opening the game.</summary>
    public sealed class CareerSummary
    {
        public static readonly CareerSummary Empty = new CareerSummary(CareerState.Empty, 0, 0, 0, 0);
        public static readonly CareerSummary Unreadable = new CareerSummary(CareerState.Unreadable, 0, 0, 0, 0);
        public CareerState State { get; }
        public double Cash { get; }
        public int Day { get; }
        public int Caste { get; }
        public long SavedUtc { get; }

        private CareerSummary(CareerState state, double cash, int day, int caste, long savedUtc)
        {
            State = state; Cash = cash; Day = day; Caste = caste; SavedUtc = savedUtc;
        }
        public static CareerSummary Saved(double cash, int day, int caste, long savedUtc) =>
            new CareerSummary(CareerState.Saved, cash, day, caste, savedUtc);
    }
}
