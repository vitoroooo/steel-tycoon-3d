using Steel.Services.Remote;
using UnityEngine;

namespace Steel.Services
{
    /// <summary>Carries the start-menu choice across the scene change into GameHost.</summary>
    public static class CareerLaunch
    {
        private static bool fresh;
        private static OnlineCareer online;

        /// <summary>Tests and batch runs play a throwaway career and never touch the player's files.</summary>
        public static bool UsesDisk => GameHost.Source == null && !Application.isBatchMode;

        public static CareerSummary Peek() => UsesDisk
            ? new CareerFiles(Application.persistentDataPath).Peek(new ResourceConfigSource())
            : CareerSummary.Empty;

        /// <summary>True asks the next GameHost to set the saved career aside and start over.</summary>
        public static void Choose(bool startFresh) => fresh = startFresh;

        /// <summary>The next GameHost plays this career on the server and leaves the files on the phone alone.</summary>
        public static void GoOnline(OnlineCareer career) => online = career;

        public static OnlineCareer TakeOnline()
        {
            OnlineCareer value = online; online = null; return value;
        }

        public static bool TakeFresh()
        {
            bool value = fresh; fresh = false; return value;
        }
    }
}
