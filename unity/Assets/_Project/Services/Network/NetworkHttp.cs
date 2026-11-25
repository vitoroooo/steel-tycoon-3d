using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services
{
    internal sealed class NetworkRejection : Exception
    {
        internal readonly int Status;
        internal NetworkRejection(int status, string message) : base(message) { Status = status; }
    }

    internal static class NetworkHttp
    {
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler {
            AllowAutoRedirect = false, UseProxy = false, UseCookies = false
        }) { Timeout = TimeSpan.FromSeconds(8) };

        internal static Task<T> Request<T>(Uri endpoint, string token, string command) where T : class =>
            Send<T>(new Uri(endpoint, command == null ? "market" : "orders"), token, command);

        /// <summary>GET when there is no body, POST when there is one (an empty body is still a POST).</summary>
        internal static async Task<T> Send<T>(Uri address, string token, string command) where T : class
        {
            bool read = command == null;
            using (var request = new HttpRequestMessage(read ? HttpMethod.Get : HttpMethod.Post, address))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!read) request.Content = new StringContent(command, Encoding.UTF8, "application/json");
                using (var response = await Client.SendAsync(request))
                {
                    string text = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = JsonUtility.FromJson<NetworkReceipt>(text);
                        throw new NetworkRejection((int)response.StatusCode, error?.message ?? "Permintaan ditolak.");
                    }
                    return JsonUtility.FromJson<T>(text) ?? throw new FormatException("Balasan server kosong.");
                }
            }
        }
    }
}
