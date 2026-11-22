using UnityEngine;
using System;

namespace Steel.Services
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameHost : MonoBehaviour
    {
        public IGameService Game { get; private set; }
        public CityLayout Layout { get; private set; }
        public bool Restored { get; private set; }
        public string SaveNotice => remote?.Notice ?? files?.Notice ?? offlineNotice;
        /// <summary>True when the career is the server's and this phone only shows it (PRD 12).</summary>
        public bool Online => remote != null;
        private Remote.RemoteGameService remote;
        private CareerFiles files;
        private LocalGameService local;
        private float checkpoint;
        private long pausedAt, latest;
        private string offlineNotice;
        private static long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        /// <summary>Config to boot from; left null the game reads Resources/Config.</summary>
        public static IConfigSource Source { get; set; }

        private void Awake()
        {
            var source = Source ?? new ResourceConfigSource();
            Layout = new CityLayout(source);
            bool fresh = CareerLaunch.TakeFresh(), replaced = false;
            Remote.OnlineCareer online = CareerLaunch.TakeOnline();
            if (online != null) { Game = remote = online.Play(source); return; }
            if (!CareerLaunch.UsesDisk) local = new LocalGameService(source);
            else
            {
                var start = CareerStart.Open(Application.persistentDataPath, source, Now, fresh);
                files = start.Files; local = start.Game; replaced = start.Replaced;
                latest = files.LastUtc;
                Restored = files.HasCareer;
                if (Restored) Report(start.Elapsed);
                offlineNotice = start.Notice ?? offlineNotice;
            }
            Game = local;
            if (Restored || replaced) Save();
        }
        private void Update()
        {
            if (pausedAt != 0) return;
            Game.Advance(Time.deltaTime);
            if (files != null && (checkpoint += Time.unscaledDeltaTime) >= 30) Save();
        }
        private void Report(double elapsed)
        {
            var q = local.Route("cart", 0, false);
            offlineNotice = "Kembali ke Sukabaja · " + Math.Floor(elapsed / 60) +
                " menit offline · Kiriman di lapak: " + q.DepotKg.ToString("N0") + " kg.";
        }
        private void Save()
        {
            latest = Math.Max(latest, Now);
            files?.Write(local, latest); checkpoint = 0;
        }
        private void OnApplicationPause(bool paused)
        {
            if (files == null) return;
            if (paused && pausedAt == 0) { Save(); pausedAt = latest; }
            else if (!paused && pausedAt != 0)
            {
                Report(local.CatchUp(Math.Max(0, Now - pausedAt))); pausedAt = 0; Save();
            }
        }
        private void OnApplicationQuit()
        {
            if (pausedAt == 0 && local != null) Save();
        }
    }
}
