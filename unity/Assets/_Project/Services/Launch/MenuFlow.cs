using System;

namespace Steel.Services
{
    public enum MenuStage { Splash, Menu, Confirm, Loading }

    /// <summary>Start-screen rules without Unity: the view forwards taps and time, then draws Stage.</summary>
    public sealed class MenuFlow
    {
        private readonly double splashSeconds;
        private double shown;
        public MenuStage Stage { get; private set; } = MenuStage.Splash;
        public CareerSummary Career { get; }
        /// <summary>Valid once Stage is Loading: the saved career is set aside before the game opens.</summary>
        public bool Fresh { get; private set; }
        /// <summary>Starting over is only offered, and only confirmed, when something would be replaced.</summary>
        public bool OffersNewCareer => Career.State != CareerState.Empty;

        public MenuFlow(CareerSummary career, double splashSeconds)
        {
            Career = career ?? throw new ArgumentNullException(nameof(career));
            if (double.IsNaN(splashSeconds) || splashSeconds < 0) throw new ArgumentOutOfRangeException(nameof(splashSeconds));
            this.splashSeconds = splashSeconds;
        }

        public void Tick(double seconds)
        {
            if (Stage != MenuStage.Splash || double.IsNaN(seconds)) return;
            shown += Math.Max(0, seconds);
            if (shown >= splashSeconds) Stage = MenuStage.Menu;
        }

        public void Skip() { if (Stage == MenuStage.Splash) Stage = MenuStage.Menu; }

        /// <summary>The big button: continue, first career, or a guest session beside an unreadable save.</summary>
        public void Play() { if (Stage == MenuStage.Menu) Stage = MenuStage.Loading; }

        public void AskNewCareer() { if (Stage == MenuStage.Menu && OffersNewCareer) Stage = MenuStage.Confirm; }

        public void ConfirmNewCareer()
        {
            if (Stage != MenuStage.Confirm) return;
            Fresh = true; Stage = MenuStage.Loading;
        }

        /// <summary>Dialog "Batal" and the Android back key.</summary>
        public void Back() { if (Stage == MenuStage.Confirm) Stage = MenuStage.Menu; }
    }
}
