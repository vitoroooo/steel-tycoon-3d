using System;

namespace Steel.Services
{
    public static class MarketEndpoint
    {
        public static Uri ForHost(string origin)
        {
            if (string.IsNullOrWhiteSpace(origin) || origin.Length > 253 ||
                !Uri.TryCreate(origin,UriKind.Absolute,out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
                uri.HostNameType != UriHostNameType.Dns || uri.IsLoopback || uri.AbsolutePath != "/" ||
                uri.UserInfo.Length > 0 || uri.Query.Length > 0 || uri.Fragment.Length > 0 ||
                origin.IndexOfAny(new[] {'\\','@','?','#'}) >= 0)
                throw new ArgumentException("Masukkan alamat HTTPS pengelola, tanpa nama akun atau jalur tambahan.");
            return new Uri(uri,"v1/");
        }
    }
}
