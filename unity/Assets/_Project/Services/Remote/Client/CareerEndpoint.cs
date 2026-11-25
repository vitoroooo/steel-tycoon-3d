using System;

namespace Steel.Services.Remote
{
    /// <summary>Where the career server is. A public server shares its HTTPS address with the market, which answers
    /// under /v1/ while the career server answers under /career/. Without an address it is this machine, for testing
    /// over USB (adb reverse tcp:8766 tcp:8766).</summary>
    public static class CareerEndpoint
    {
        public const int LocalPort = 8766;
        /// <summary>The game's own server; the sign-in form starts with it and the player may type another.</summary>
        public const string Public = "https://steel.vitooel.com";

        public static Uri ForHost(string origin) => string.IsNullOrWhiteSpace(origin)
            ? Local(LocalPort)
            : new Uri(MarketEndpoint.ForHost(origin.Trim()), "../career/");

        public static Uri Local(int port)
        {
            if (port < 1 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            return new UriBuilder("http", "127.0.0.1", port, "career/").Uri;
        }
    }
}
