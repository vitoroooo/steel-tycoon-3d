using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services.Remote
{
    /// <summary>One account's line to the career server over HTTP.</summary>
    public sealed class CareerClient
    {
        private readonly Uri endpoint;
        private readonly string token;
        public string Host => endpoint.Authority;

        public CareerClient(Uri endpoint, string token)
        {
            this.endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            if (!Regex.IsMatch(token ?? "", @"\A[A-Za-z0-9_-]{43}\z"))
                throw new ArgumentException("Masukkan token lengkap dari pengelola server.");
            this.token = token;
        }

        /// <summary>The market between players for the same account: it answers under /v1/ beside /career/.</summary>
        public NetworkMarket Market() => new NetworkMarket(new Uri(endpoint, "../v1/"), token);

        /// <summary>The whole career, after the server has counted the time away.</summary>
        public async Task<RemoteReply> Open()
        {
            try
            {
                RemoteReply reply = await NetworkHttp.Send<RemoteReply>(new Uri(endpoint, "open"), token, "");
                return reply.HasSave ? reply : throw new FormatException("Server tidak mengirim karier.");
            }
            catch (NetworkRejection refused) { throw new InvalidOperationException(refused.Message); }
            catch (Exception lost) when (!(lost is FormatException))
            { throw new InvalidOperationException("Server karier tidak terjangkau. Periksa alamat dan sambungan."); }
        }

        public Task<RemoteReply> Act(RemoteBatch batch) =>
            NetworkHttp.Send<RemoteReply>(new Uri(endpoint, "act"), token, JsonUtility.ToJson(batch));
    }
}
