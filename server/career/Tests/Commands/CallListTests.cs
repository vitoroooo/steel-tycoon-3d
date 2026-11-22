using System;
using System.Linq;
using System.Reflection;
using Steel.Career.Commands;
using Steel.Career.Tests.Support;
using Steel.Services;
using Steel.Services.Remote;
using Xunit;

namespace Steel.Career.Tests.Commands
{
    /// <summary>The list that decides what the server must hear about (Services/Remote/RemoteCalls.cs) is kept honest here.</summary>
    public sealed class CallListTests
    {
        private static readonly MethodInfo[] All =
            typeof(IGameService).GetMethods().Where(method => !method.IsSpecialName).ToArray();

        [Fact] public void EveryListedNameIsAMethodOfTheGame()
        {
            string[] names = All.Select(method => method.Name).ToArray();
            foreach (string listed in RemoteCalls.Time.Concat(RemoteCalls.Motion).Concat(RemoteCalls.Queries))
                Assert.Contains(listed, names);
        }

        [Fact] public void ThePhoneCannotSendTimeOrAnUnknownCall()
        {
            Assert.DoesNotContain("Advance", Dispatcher.Names);
            Assert.DoesNotContain("CatchUp", Dispatcher.Names);
            using var town = new Town();
            var game = new LocalGameService(town.Rules);
            foreach (string name in new[] { "Advance", "CatchUp", "Save", "Digest", "get_View", null })
                Assert.False(Dispatcher.Run(game, new RemoteCall { name = name, args = new[] { "100" } }).ok);
            Assert.False(Dispatcher.Run(game, new RemoteCall { name = "SetBid", args = new[] { "1", "NaN" } }).ok);
            Assert.False(Dispatcher.Run(game, new RemoteCall { name = "SetBid", args = new[] { "1" } }).ok);
        }

        /// <summary>A query the phone answers alone must not change the career, whatever it is asked.</summary>
        [Fact] public void NoQueryChangesTheCareer()
        {
            using var town = new Town();
            var game = new LocalGameService(town.Rules);
            game.Advance(3700);
            string before = game.Save(1);
            foreach (MethodInfo query in All.Where(method => RemoteCalls.Queries.Contains(method.Name)))
                foreach (int sample in new[] { 0, 1 })
                {
                    object[] args = query.GetParameters().Select(p => Sample(p.ParameterType, sample, game)).ToArray();
                    try { query.Invoke(game, args); }
                    catch (TargetInvocationException) { }
                    Assert.True(before == game.Save(1), query.Name + " changed the career");
                }
        }

        private static object Sample(Type type, int sample, LocalGameService game)
        {
            if (type == typeof(string)) return sample == 0 ? "p0" : "triseda";
            if (type.IsEnum) return Enum.ToObject(type, sample);
            if (type == typeof(bool)) return sample == 1;
            if (type == typeof(Steel.Core.Scrap.ScrapPile)) return game.CreatePile(10, Steel.Core.Scrap.ScrapGrade.Hms1);
            return type.IsValueType ? Convert.ChangeType(sample, Nullable.GetUnderlyingType(type) ?? type) : null;
        }
    }
}
