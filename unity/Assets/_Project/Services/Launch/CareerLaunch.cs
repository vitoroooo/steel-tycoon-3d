using UnityEngine;

namespace Steel.Services
{
    /// <summary>Carries the start-menu choice across the scene change into GameHost.</summary>
    public static class CareerLaunch
    {
        private static bool fresh;

        /// <summary>Tests and batch runs play a throwaway career and never touch the player's files.</summary>
        public static bool UsesDisk => GameHost.Source == null && !Application.isBatchMode;

        public static CareerSummary Peek() => UsesDisk
            ? new CareerFiles(Application.persistentDataPath).Peek(new ResourceConfigSource())
            : CareerSummary.Empty;

        /// <summary>True asks the next GameHost to set the saved career aside and start over.</summary>
        public static void Choose(bool startFresh) => fresh = startFresh;

        public static bool TakeFresh()
        {
            bool value = fresh; fresh = false; return value;
        }
    }
}
