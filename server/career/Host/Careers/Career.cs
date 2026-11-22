using System.Linq;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    /// <summary>One account's career as the server holds it: the game's own rules, run on the server's clock.</summary>
    public sealed class Career
    {
        private readonly IConfigSource rules;
        private LocalGameService game;
        private CareerClock clock;
        public long Seq { get; private set; }
        public RemoteReply LastReply { get; private set; }
        public long SeenMs => clock.SeenMs;
        public double Cash => game.WalletCash;
        public double StockKg => game.WalletStockKg;
        public int Caste => game.WalletCaste;
        /// <summary>What the market did to this account's purse while the career was not looking.</summary>
        public void Settle(double cash, double kg) => game.SettleMarket(cash, kg);

        public Career(IConfigSource rules, long nowMs)
        {
            this.rules = rules; game = new LocalGameService(rules);
            clock = new CareerClock(game.ClockSeconds, nowMs);
        }

        public Career(IConfigSource rules, CareerRecord saved)
        {
            this.rules = rules; Seq = saved.Seq; LastReply = saved.Reply;
            long savedUtc = saved.SeenMs / 1000;
            game = LocalGameService.Resume(rules, saved.Save, savedUtc, out _);
            clock = new CareerClock(game.ClockSeconds, saved.SeenMs);
        }

        /// <summary>The player comes in: time away is counted with the offline rules, and the phone gets the whole career.</summary>
        public RemoteReply Open(long nowMs)
        {
            ComeBack(nowMs);
            return Answer(new RemoteResult[0], null, nowMs);
        }

        public RemoteReply Act(RemoteBatch batch, long nowMs)
        {
            if (batch.seq == Seq && LastReply != null) return LastReply;
            if (batch.seq != Seq + 1) return Answer(null, null, nowMs);
            ComeBack(nowMs);
            RemoteCall[] calls = batch.calls ?? new RemoteCall[0];
            bool drifted = false;
            RemoteResult[] results = calls.Select(call =>
            {
                drifted |= RunTo(call?.at ?? double.NaN, nowMs);
                return RemoteRunner.Run(game, call);
            }).ToArray();
            drifted |= RunTo(batch.at, nowMs);
            // a phone whose clock is not the server's gets the server's career back, at the server's time
            if (drifted) game.Advance(System.Math.Max(0, clock.Now(nowMs) - game.ClockSeconds));
            Seq++;
            return LastReply = Answer(results, drifted ? null : batch.digest, nowMs);
        }

        /// <summary>Runs the career up to the time the phone names, as far as the server's clock allows. True when it does not.</summary>
        private bool RunTo(double asked, long nowMs)
        {
            double to = clock.Granted(asked, game.ClockSeconds, nowMs);
            game.Advance(to - game.ClockSeconds);
            return !(System.Math.Abs(to - asked) <= CareerClock.Drift);
        }

        public CareerRecord Record() => new CareerRecord
            { Save = game.Save(System.Math.Max(1, clock.SeenMs / 1000)), SeenMs = clock.SeenMs, Seq = Seq, Reply = LastReply };

        private void ComeBack(long nowMs)
        {
            double away = clock.AwayFor(nowMs);
            if (away > CareerClock.AwaySeconds) { game.CatchUp(away); clock.Anchor(game.ClockSeconds, nowMs); }
            clock.Seen(nowMs);
        }

        private RemoteReply Answer(RemoteResult[] results, string phoneDigest, long nowMs)
        {
            string digest = game.Digest();
            long utc = System.Math.Max(1, nowMs / 1000);
            return new RemoteReply
            {
                seq = Seq, results = results, digest = digest, savedUtc = utc,
                save = phoneDigest == digest ? null : game.Save(utc)
            };
        }
    }
}
