using System;
using System.Threading.Tasks;

namespace Steel.Services.Remote
{
    public enum LoginStage { Closed, Form, Connecting, Ready }

    /// <summary>The start menu's "Main online" form without Unity: the view forwards taps and draws Stage and Message.</summary>
    public sealed class OnlineLogin
    {
        public const string Button = "MAIN ONLINE", Title = "MAIN ONLINE", Go = "MASUK", Cancel = "BATAL";
        public const string AddressHint = "Alamat server (https://…), kosong = komputer ini",
            TokenHint = "Token akun dari pengelola server";
        private const string Intro = "Karier online dihitung dan disimpan di server.\nKarier di HP ini tidak berubah.";
        private readonly Func<OnlineAccount, Task<OnlineCareer>> signIn;
        private Task<OnlineCareer> waiting;
        public LoginStage Stage { get; private set; }
        public string Message { get; private set; } = Intro;
        public OnlineAccount Account { get; private set; }
        /// <summary>Set once Stage is Ready: the career the city will play.</summary>
        public OnlineCareer Career { get; private set; }

        public OnlineLogin(OnlineAccount saved, Func<OnlineAccount, Task<OnlineCareer>> signIn)
        {
            Account = saved != null && saved.Known ? saved : new OnlineAccount { origin = CareerEndpoint.Public };
            this.signIn = signIn ?? throw new ArgumentNullException(nameof(signIn));
        }

        public void Show() { if (Stage == LoginStage.Closed) Stage = LoginStage.Form; }
        public void Back() { if (Stage == LoginStage.Form) { Stage = LoginStage.Closed; Message = Intro; } }

        public void Submit(string origin, string token)
        {
            if (Stage != LoginStage.Form) return;
            Account = new OnlineAccount { origin = (origin ?? "").Trim(), token = (token ?? "").Trim() };
            Stage = LoginStage.Connecting; Message = "Menyambung ke server…";
            try { waiting = signIn(Account); }
            catch (Exception refused) { waiting = Task.FromException<OnlineCareer>(refused); }
        }

        /// <summary>Called every frame; true on the frame the career arrives.</summary>
        public bool Tick()
        {
            if (Stage != LoginStage.Connecting || !waiting.IsCompleted) return false;
            if (waiting.Status == TaskStatus.RanToCompletion && waiting.Result != null)
            {
                Career = waiting.Result; Stage = LoginStage.Ready; Message = "Tersambung."; return true;
            }
            Exception why = waiting.Exception?.GetBaseException();
            Message = why is InvalidOperationException || why is ArgumentException ? why.Message
                : "Server karier tidak terjangkau. Periksa alamat dan sambungan.";
            Stage = LoginStage.Form;
            return false;
        }
    }
}
