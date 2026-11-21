using System;
using Steel.Core.State.Saving;
using UnityEngine;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        public string Save(long now)
        {
            var state = session.Snapshot(Areas.Length);
            state.seconds = clock.RealSeconds; state.savedUtc = now;
            return JsonUtility.ToJson(state);
        }

        public static LocalGameService Resume(IConfigSource config, string json, long now, out double elapsed)
        {
            var s = JsonUtility.FromJson<CareerSave>(json);
            if (s == null || s.version != 1 || s.savedUtc <= 0 || s.sites == null || s.routes == null ||
                s.quota == null || s.bag == null || s.cart == null || s.motors == null)
                throw new FormatException("Simpanan tidak valid atau tidak kompatibel.");
            var game = new LocalGameService(config);
            game.clock.Restore(s.seconds);
            game.market.Refresh(s.seconds, game.supplyTargetKg);
            game.session.Load(s, Areas.Length, (long)Math.Floor(game.GameHours / 24));
            elapsed = game.CatchUp(Math.Max(0d, (double)now - s.savedUtc));
            return game;
        }
    }
}
