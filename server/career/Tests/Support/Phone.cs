using System.Collections.Generic;
using System.Linq;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Tests.Support
{
    /// <summary>A phone as the game will be one: it runs each call on its own copy first, sends the batch, and takes
    /// the server's career whenever the server says its copy is wrong.</summary>
    public sealed class Phone
    {
        private readonly Town town;
        private readonly long account;
        private readonly List<RemoteCall> waiting = new List<RemoteCall>();
        public LocalGameService Game { get; private set; }
        public CityLayout Map { get; }
        public long Seq { get; private set; }
        public RemoteReply Last { get; private set; }
        public int Corrections { get; private set; }

        public Phone(Town town, string name)
        {
            this.town = town; account = town.Accounts.Find(town.Token(name));
            Map = new CityLayout(town.Rules);
            Take(town.Careers.Open(account));
            Corrections = 0;
        }

        /// <summary>Lets game time run on the phone as frames do.</summary>
        public void Play(double seconds)
        {
            for (double left = seconds; left > 0; left -= 0.25) Game.Advance(System.Math.Min(0.25, left));
            town.Pass(seconds);
        }

        /// <summary>Runs a call on the phone, if the phone's rules allow it, and queues it for the server either way.</summary>
        public void Do(string name, params object[] args)
        {
            try { typeof(IGameService).GetMethod(name).Invoke(Game, args); }
            catch (System.Reflection.TargetInvocationException) { }
            Queue(name, args);
        }

        public void Queue(string name, params object[] args) => waiting.Add(new RemoteCall
            { name = name, args = args.Select(CallText.Write).ToArray(), at = Game.ClockSeconds });

        public void Go(SliceLocation place) => Do("SetPosition", place.X, place.Z);

        public RemoteReply Send(string digest = null)
        {
            var batch = new RemoteBatch { seq = Seq + 1, calls = waiting.ToArray(), digest = digest ?? Game.Digest() };
            waiting.Clear();
            return Take(town.Careers.Act(account, batch));
        }

        public RemoteReply Resend(RemoteBatch batch) => town.Careers.Act(account, batch);
        public RemoteReply Reopen() => Take(town.Careers.Open(account));

        private RemoteReply Take(RemoteReply reply)
        {
            Last = reply; Seq = reply.seq;
            if (reply.save == null) return reply;
            Game = LocalGameService.Resume(town.Rules, reply.save, reply.savedUtc, out _);
            Corrections++;
            return reply;
        }
    }
}
