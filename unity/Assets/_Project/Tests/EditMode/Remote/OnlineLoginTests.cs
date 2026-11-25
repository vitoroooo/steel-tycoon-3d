using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services.Remote;

namespace Steel.Tests.Remote
{
    public sealed class OnlineLoginTests
    {
        [Test] public void AFailedSignInReturnsToTheFormWithTheReason()
        {
            var login = new OnlineLogin(null, _ => Task.FromException<OnlineCareer>(new InvalidOperationException("Token salah.")));
            Assert.AreEqual(CareerEndpoint.Public, login.Account.origin, "A first sign-in starts on the game's own server");
            login.Show();
            login.Submit(" https://pasar.example ", " abc ");
            Assert.AreEqual(LoginStage.Connecting, login.Stage);
            Assert.AreEqual("https://pasar.example", login.Account.origin);
            Assert.AreEqual("abc", login.Account.token);
            Assert.IsFalse(login.Tick());
            Assert.AreEqual(LoginStage.Form, login.Stage);
            Assert.AreEqual("Token salah.", login.Message);
            login.Back();
            Assert.AreEqual(LoginStage.Closed, login.Stage);
        }

        [Test] public void AnUnexpectedFailureIsReportedAsAnUnreachableServer()
        {
            var login = new OnlineLogin(null, _ => throw new System.IO.IOException("socket"));
            login.Show(); login.Submit("", "");
            Assert.IsFalse(login.Tick());
            StringAssert.Contains("tidak terjangkau", login.Message);
        }

        [Test] public void NothingIsSentBeforeTheFormIsOpenOrWhileASignInIsRunning()
        {
            int asked = 0;
            var pending = new TaskCompletionSource<OnlineCareer>();
            var login = new OnlineLogin(null, _ => { asked++; return pending.Task; });
            login.Submit("", "x");
            Assert.AreEqual(0, asked);
            login.Show(); login.Submit("", "x"); login.Submit("", "x"); login.Back();
            Assert.AreEqual(1, asked);
            Assert.AreEqual(LoginStage.Connecting, login.Stage);
            Assert.IsFalse(login.Tick());
        }

        [Test] public void TheCareerAddressSitsBesideTheMarketOnTheSameHost()
        {
            Assert.AreEqual("https://pasar.example/career/open",
                new Uri(CareerEndpoint.ForHost("https://pasar.example"), "open").AbsoluteUri);
            Assert.AreEqual("http://127.0.0.1:8766/career/act", new Uri(CareerEndpoint.ForHost(""), "act").AbsoluteUri);
            Assert.Throws<ArgumentException>(() => CareerEndpoint.ForHost("http://pasar.example"));
            Assert.Throws<ArgumentException>(() => new CareerClient(CareerEndpoint.Local(8766), "pendek"));
        }
    }
}
