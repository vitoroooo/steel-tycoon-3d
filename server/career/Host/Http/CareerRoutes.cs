using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Steel.Career.Accounts;
using Steel.Career.Careers;
using Steel.Services.Remote;
using UnityEngine;

namespace Steel.Career.Http
{
    /// <summary>POST /career/open gives the phone its career; POST /career/act runs a batch of calls on it.</summary>
    public static class CareerRoutes
    {
        private const int MaxBody = 64 * 1024, MaxCalls = 64;

        public static void Map(WebApplication app, AccountBook accounts, CareerBook careers)
        {
            app.MapPost("/career/open", (HttpContext http) => Serve(http, accounts, (id, _) => careers.Open(id)));
            app.MapPost("/career/act", (HttpContext http) => Serve(http, accounts, (id, body) =>
            {
                RemoteBatch batch = JsonUtility.FromJson<RemoteBatch>(body);
                return batch == null || batch.calls?.Length > MaxCalls ? null : careers.Act(id, batch);
            }));
        }

        private static async Task Serve(HttpContext http, AccountBook accounts, System.Func<long, string, RemoteReply> work)
        {
            string auth = http.Request.Headers.Authorization.ToString();
            long account = accounts.Find(auth.StartsWith("Bearer ") ? auth.Substring(7) : "");
            if (account <= 0) { await Refuse(http, account < 0 ? 403 : 401, "Akun tidak dikenal atau ditangguhkan."); return; }
            if (http.Request.ContentLength > MaxBody) { await Refuse(http, 413, "Kiriman terlalu besar."); return; }
            string body;
            using (var reader = new StreamReader(http.Request.Body)) body = await reader.ReadToEndAsync();
            RemoteReply reply;
            try { reply = body.Length > MaxBody ? null : work(account, body); }
            catch (System.Text.Json.JsonException) { reply = null; }
            if (reply == null) { await Refuse(http, 400, "Kiriman tidak terbaca."); return; }
            http.Response.ContentType = "application/json";
            await http.Response.WriteAsync(JsonUtility.ToJson(reply));
        }

        private static Task Refuse(HttpContext http, int status, string why)
        {
            http.Response.StatusCode = status; http.Response.ContentType = "application/json";
            return http.Response.WriteAsync(JsonUtility.ToJson(new RemoteResult { ok = false, error = why }));
        }
    }
}
