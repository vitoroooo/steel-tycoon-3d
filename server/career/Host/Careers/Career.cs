using System.Linq;
using Steel.Career.Commands;
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
                double asked = call?.at ?? double.NaN, to = clock.Granted(asked, game.ClockSeconds, nowMs);
                // a phone whose clock is not the server's gets the server's career back, clock and all
                drifted |= !(System.Math.Abs(to - asked) <= CareerClock.Drift);
                game.Advance(to - game.ClockSeconds);
                return Dispatcher.Run(game, call);
            }).ToArray();
            Seq++;
            return LastReply = Answer(results, drifted ? null : batch.digest, nowMs);
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
