using System;
using System.Threading.Tasks;

namespace Steel.Services.Remote
{
    /// <summary>A career opened on the server at the start menu, waiting for the city to load.</summary>
    public sealed class OnlineCareer
    {
        public CareerClient Client { get; }
        public RemoteReply Opened { get; }
        public CareerSummary Summary { get; }

        private OnlineCareer(CareerClient client, RemoteReply opened, IConfigSource rules)
        {
            Client = client; Opened = opened;
            GameView view = LocalGameService.Resume(rules, opened.save, opened.savedUtc, out _).View;
            Summary = CareerSummary.Saved(view.Cash, 1 + (int)Math.Floor(view.GameHours / 24), view.Caste, opened.savedUtc);
        }

        public static async Task<OnlineCareer> SignIn(OnlineAccount account, IConfigSource rules)
        {
            CareerClient client;
            try { client = account.Client(); }
            catch (ArgumentException wrong) { throw new InvalidOperationException(wrong.Message); }
            return await Open(client, rules);
        }

        public static async Task<OnlineCareer> Open(CareerClient client, IConfigSource rules) =>
            new OnlineCareer(client, await client.Open(), rules);

        /// <summary>The game service the city plays on: the phone guesses, the server decides.</summary>
        public RemoteGameService Play(IConfigSource rules) =>
            new RemoteGameService(rules, Opened, new RemoteLink(Client.Act, Opened.seq));
    }
}
