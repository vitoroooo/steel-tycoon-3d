using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Launch
{
    public sealed class MenuFlowTests
    {
        private static readonly CareerSummary Saved = CareerSummary.Saved(3400000, 12, 2, 5000);
        private static MenuFlow AtMenu(CareerSummary career)
        {
            var flow = new MenuFlow(career, 1.8); flow.Skip(); return flow;
        }

        [Test] public void SplashHoldsThenOpensMenuByItself()
        {
            var flow = new MenuFlow(CareerSummary.Empty, 1.8);
            flow.Tick(1); Assert.AreEqual(MenuStage.Splash, flow.Stage);
            flow.Tick(double.NaN); flow.Tick(-5); Assert.AreEqual(MenuStage.Splash, flow.Stage);
            flow.Play(); flow.AskNewCareer(); flow.ConfirmNewCareer();
            Assert.AreEqual(MenuStage.Splash, flow.Stage, "Nothing behind the splash can be pressed");
            flow.Tick(.8); Assert.AreEqual(MenuStage.Menu, flow.Stage);
        }

        [Test] public void TapSkipsSplash()
        {
            var flow = new MenuFlow(CareerSummary.Empty, 60);
            flow.Skip(); Assert.AreEqual(MenuStage.Menu, flow.Stage);
        }

        [Test] public void FirstCareerStartsInOneTapAndOffersNothingToReplace()
        {
            var flow = AtMenu(CareerSummary.Empty);
            Assert.IsFalse(flow.OffersNewCareer);
            flow.AskNewCareer(); Assert.AreEqual(MenuStage.Menu, flow.Stage);
            flow.Play();
            Assert.AreEqual(MenuStage.Loading, flow.Stage); Assert.IsFalse(flow.Fresh);
        }

        [Test] public void ContinueKeepsTheSavedCareer()
        {
            var flow = AtMenu(Saved); flow.Play();
            Assert.AreEqual(MenuStage.Loading, flow.Stage); Assert.IsFalse(flow.Fresh);
        }

        [Test] public void NewCareerAsksFirstAndBackKeepsTheCareer()
        {
            var flow = AtMenu(Saved);
            flow.ConfirmNewCareer(); Assert.AreEqual(MenuStage.Menu, flow.Stage, "No confirm without the question");
            flow.AskNewCareer(); Assert.AreEqual(MenuStage.Confirm, flow.Stage);
            flow.Play(); Assert.AreEqual(MenuStage.Confirm, flow.Stage, "Menu is inert under the dialog");
            flow.Back(); Assert.AreEqual(MenuStage.Menu, flow.Stage); Assert.IsFalse(flow.Fresh);
            flow.AskNewCareer(); flow.ConfirmNewCareer();
            Assert.AreEqual(MenuStage.Loading, flow.Stage); Assert.IsTrue(flow.Fresh);
        }

        [Test] public void UnreadableSaveCanBePlayedBesideOrReplaced()
        {
            var guest = AtMenu(CareerSummary.Unreadable); guest.Play();
            Assert.IsFalse(guest.Fresh, "Guest session leaves the unreadable files alone");
            var replace = AtMenu(CareerSummary.Unreadable);
            Assert.IsTrue(replace.OffersNewCareer);
            replace.AskNewCareer(); replace.ConfirmNewCareer(); Assert.IsTrue(replace.Fresh);
        }

        [Test] public void ChoiceIsFinalOnceLoading()
        {
            var flow = AtMenu(Saved); flow.Play();
            flow.Back(); flow.AskNewCareer(); flow.ConfirmNewCareer(); flow.Skip(); flow.Tick(100);
            Assert.AreEqual(MenuStage.Loading, flow.Stage); Assert.IsFalse(flow.Fresh);
        }

        [Test] public void RejectsMissingCareerAndBadSplashTime()
        {
            Assert.Throws<ArgumentNullException>(() => new MenuFlow(null, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MenuFlow(Saved, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MenuFlow(Saved, double.NaN));
            var instant = new MenuFlow(Saved, 0); instant.Tick(0);
            Assert.AreEqual(MenuStage.Menu, instant.Stage);
        }
    }
}
