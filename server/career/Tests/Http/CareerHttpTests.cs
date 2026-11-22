using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Steel.Career.Tests.Support;
using Steel.Services.Remote;
using UnityEngine;
using Xunit;

namespace Steel.Career.Tests.Http
{
    /// <summary>The career server as a phone reaches it: over HTTP, with the market's account token.</summary>
    public sealed class CareerHttpTests
    {
        private const int Port = 18766;

        private static async Task<(HttpStatusCode, string)> Post(HttpClient http, string path, string token, string body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:" + Port + path)
                { Content = new StringContent(body, Encoding.UTF8, "application/json") };
            if (token != null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await http.SendAsync(request);
            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [Fact] public async Task ATokenOpensItsCareerAndRunsABatchAndNothingElseDoes()
        {
            using var town = new Town();
            string token = town.Token("Budi");
            await using WebApplication app = Program.Build(town.Database, Town.Resources, Port, () => town.NowMs);
            await app.StartAsync();
            using var http = new HttpClient();

            Assert.Equal(HttpStatusCode.Unauthorized, (await Post(http, "/career/open", null, "")).Item1);
            Assert.Equal(HttpStatusCode.Unauthorized, (await Post(http, "/career/open", "salah", "")).Item1);

            (HttpStatusCode status, string body) = await Post(http, "/career/open", token, "");
            Assert.Equal(HttpStatusCode.OK, status);
            RemoteReply opened = JsonUtility.FromJson<RemoteReply>(body);
            Assert.NotNull(opened.save);

            var batch = new RemoteBatch { seq = 1, digest = opened.digest,
                calls = new[] { new RemoteCall { name = "ReadGuide", args = new string[0], at = 0 } } };
            (status, body) = await Post(http, "/career/act", token, JsonUtility.ToJson(batch));
            Assert.Equal(HttpStatusCode.OK, status);
            RemoteReply acted = JsonUtility.FromJson<RemoteReply>(body);
            Assert.True(acted.results[0].ok, acted.results[0].error);
            Assert.Equal(1, acted.seq);

            Assert.Equal(HttpStatusCode.BadRequest, (await Post(http, "/career/act", token, "{bukan json")).Item1);
            town.Market("suspend", "Budi");
            Assert.Equal(HttpStatusCode.Forbidden, (await Post(http, "/career/open", token, "")).Item1);
            await app.StopAsync();
        }
    }
}
