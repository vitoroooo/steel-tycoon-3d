using Steel.Services;
using Steel.Services.Remote;
using UnityEngine;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>"Main online": signs in on the career server, then loads the city on the server's career.</summary>
    public sealed partial class BootScreen
    {
        public OnlineLogin Login { get; private set; }
        public OnlineView Online { get; private set; }
        /// <summary>Tests sign in against their own server; left null the game uses the address the player typed.</summary>
        public static System.Func<OnlineAccount, System.Threading.Tasks.Task<OnlineCareer>> SignIn { get; set; }

        private static OnlineLogin NewLogin()
        {
            var rules = GameHost.Source ?? new ResourceConfigSource();
            OnlineAccount saved = CareerLaunch.UsesDisk ? OnlineAccount.Read(Application.persistentDataPath) : null;
            return new OnlineLogin(saved, SignIn ?? (account => OnlineCareer.SignIn(account, rules)));
        }

        private void TickOnline()
        {
            bool arrived = Login.Tick();
            Online.Screen.Visible = Login.Stage != LoginStage.Closed && Flow.Stage == MenuStage.Menu;
            Online.Draw(Login);
            if (!arrived) return;
            if (CareerLaunch.UsesDisk) Login.Account.Write(Application.persistentDataPath);
            CareerLaunch.GoOnline(Login.Career);
            Flow.Play();
        }
    }
}
