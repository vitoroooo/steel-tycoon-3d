using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Network
{
    /// <summary>Two Unity clients found and join an association, chat, report, and leave (PRD 8.1).</summary>
    internal static class GuildFlow
    {
        internal static async Task Run(NetworkMarket founder, NetworkMarket joiner)
        {
            await founder.Refresh();
            Assert.IsFalse(founder.Board.association.Joined);
            Assert.AreEqual(1, founder.Board.caste);
            await founder.Found("Besi Tua Jaya");
            Assert.IsTrue(founder.Board.association.Joined);

            await joiner.Refresh();
            Assert.AreEqual("Besi Tua Jaya", joiner.Board.associations[0].name);
            Assert.AreEqual(1, joiner.Board.associations[0].members);
            await joiner.Join("Besi Tua Jaya");
            CollectionAssert.AreEquivalent(new[] { founder.Board.name, joiner.Board.name }, joiner.Board.association.members);

            await founder.Say("Besok harga naik, goblok kalau jual sekarang");
            await joiner.Refresh();
            var line = joiner.Board.association.messages[0];
            Assert.AreEqual(founder.Board.name, line.name);
            Assert.AreEqual("Besok harga naik, ****** kalau jual sekarang", line.text);
            StringAssert.Contains("Laporan", await joiner.Report(line.id));

            // neither is Kasta 3 and the market account cannot pay the package: no blockade, no promotion
            StringAssert.Contains("Kasta 3", await founder.Blockade(joiner.Board.name));
            StringAssert.Contains("kurang", await founder.Promote());
            Assert.IsFalse(joiner.Board.Blocked);
            StringAssert.Contains("tidak sedang diblokade", await joiner.Escort());

            await joiner.Leave();
            Assert.IsFalse(joiner.Board.association.Joined);
            await founder.Leave();
            Assert.AreEqual(0, founder.Board.associations.Length);
        }
    }
}
