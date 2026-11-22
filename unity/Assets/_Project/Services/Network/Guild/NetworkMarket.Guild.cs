using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services
{
    /// <summary>Associations, their chat and route blockades (PRD 6.3 and 8.1) on the market server.</summary>
    public sealed partial class NetworkMarket
    {
        public Task<string> Found(string name) => Named("found", name);
        public Task<string> Join(string name) => Named("join", name);
        public Task<string> Leave() => Plain("leave");
        public Task<string> Say(string text) => Place(JsonUtility.ToJson(new TextCommand { text = text }));
        public Task<string> Report(long message) => Place(JsonUtility.ToJson(new ReportCommand { message = message }));

        /// <summary>Buys the next caste package from the market account.</summary>
        public Task<string> Promote() => Plain("promote");
        /// <summary>A Kasta 3 member pays to close a Kasta 3 rival's route for one game day.</summary>
        public Task<string> Blockade(string target) => Named("blockade", target);
        /// <summary>The blockaded trader hires an escort team that opens his route again.</summary>
        public Task<string> Escort() => Plain("escort");

        private Task<string> Plain(string action) => Place(JsonUtility.ToJson(new PlainCommand { action = action }));
        private Task<string> Named(string action, string name) =>
            Place(JsonUtility.ToJson(new NameCommand { action = action, name = name }));
    }
}
