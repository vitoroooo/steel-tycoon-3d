using NUnit.Framework;
using Steel.Presentation.UI.Boot;
using UnityEngine;

namespace Steel.Tests.Launch
{
    /// <summary>The look is data: a new art direction edits Resources/Theme/Boot.json, not the menu code.</summary>
    public sealed class BootThemeTests
    {
        [Test] public void ShippedThemeReadsAndEveryColourIsValid()
        {
            var theme = BootTheme.Load();
            Assert.IsNotEmpty(theme.title); Assert.IsNotEmpty(theme.tagline);
            foreach (var colour in new[] { theme.Background, theme.Panel, theme.Accent, theme.AccentInk, theme.Text,
                theme.Muted, theme.Danger }) Assert.AreNotEqual(Color.magenta, colour);
            Assert.AreNotEqual(theme.Accent, theme.AccentInk, "Button caption is readable on the button");
            Assert.AreNotEqual(theme.Background, theme.Text);
            Assert.That(theme.SplashSeconds, Is.InRange(.5f, 5f));
            Assert.IsNotNull(theme.Font);
        }

        [Test] public void MissingFileOrKeysFallBackToTheDefaultLook()
        {
            var plain = BootTheme.Read(null);
            Assert.AreEqual("STEEL TYCOON 3D", plain.title); Assert.AreEqual(1.8f, plain.SplashSeconds);
            var partial = BootTheme.Read("{\"accent\":\"#00FF00\",\"title\":\"Juragan Besi\"}");
            Assert.AreEqual(Color.green, partial.Accent); Assert.AreEqual("Juragan Besi", partial.title);
            Assert.AreEqual(plain.Background, partial.Background);
        }

        [Test] public void BadValuesAreVisibleOrClampedNeverFatal()
        {
            var theme = BootTheme.Read("{\"accent\":\"kuning\",\"splashSeconds\":-4,\"logo\":\"Theme/tidak-ada\"}");
            Assert.AreEqual(Color.magenta, theme.Accent, "A typo shows up as magenta instead of crashing");
            Assert.AreEqual(0f, theme.SplashSeconds);
            Assert.AreEqual(10f, BootTheme.Read("{\"splashSeconds\":900}").SplashSeconds);
            Assert.IsNull(BootTheme.Picture(theme.logo)); Assert.IsNull(BootTheme.Picture(""));
        }
    }
}
