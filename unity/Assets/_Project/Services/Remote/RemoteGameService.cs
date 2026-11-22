using System;
using System.Linq;

namespace Steel.Services.Remote
{
    /// <summary>The game as a phone plays it against the career server (PRD 12). Every call runs first on the phone's own
    /// copy of the career, so the screen answers at once, and is then sent to the server. When the server's result is not
    /// the phone's, the phone's copy is replaced by the server's. The members that forward calls are generated.</summary>
    public sealed partial class RemoteGameService : IGameService
    {
        private readonly IConfigSource rules;
        private readonly RemoteLink link;
        private LocalGameService game;
        private string refusal;
        /// <summary>How many times the server's career replaced the phone's guess.</summary>
        public int Corrections { get; private set; }
        /// <summary>Why the server refused the last call, or that it cannot be reached.</summary>
        public string Notice => link.Notice ?? refusal;
        public string Digest => game.Digest();

        public RemoteGameService(IConfigSource rules, RemoteReply opened, RemoteLink link)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.link = link ?? throw new ArgumentNullException(nameof(link));
            if (opened == null || !opened.HasSave) throw new FormatException("Server tidak mengirim karier.");
            game = LocalGameService.Resume(rules, opened.save, opened.savedUtc, out _);
        }

        public void Advance(double realSeconds)
        {
            game.Advance(realSeconds);
            Take(link.Pump(realSeconds, game.ClockSeconds, game.Digest));
        }

        /// <summary>Time away is counted by the server; the phone learns the result when it opens the career again.</summary>
        public double CatchUp(double elapsedRealSeconds) => 0;

        private void Sent(string name, params object[] args) => link.Queue(Call(name, args));
        private void Moved(string name, params object[] args) => link.Move(name + (args[0] as string), Call(name, args));

        private RemoteCall Call(string name, object[] args) =>
            new RemoteCall { name = name, args = args.Select(CallText.Write).ToArray(), at = game.ClockSeconds };

        private void Take(RemoteReply reply)
        {
            if (reply == null) return;
            refusal = reply.Refusal();
            if (!reply.HasSave) return;
            game = LocalGameService.Resume(rules, reply.save, reply.savedUtc, out _);
            Corrections++;
            // what the player did since that batch left is done again on the server's career
            foreach (RemoteCall call in link.Unsent.ToArray()) RemoteRunner.Run(game, call);
            link.Restamp(game.ClockSeconds);
        }
    }
}
