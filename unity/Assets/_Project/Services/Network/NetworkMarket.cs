using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services
{
    public sealed partial class NetworkMarket
    {
        private readonly Uri endpoint;
        private string token, pending;
        public NetworkBoard Board { get; private set; }
        public bool HasSession => token != null;
        public bool HasPending => pending != null;
        public string Host => endpoint.Authority;

        public NetworkMarket(string origin) { endpoint = MarketEndpoint.ForHost(origin); }

        public NetworkMarket(int port = 8765)
        {
            if (port < 1 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            endpoint = new UriBuilder("http", "127.0.0.1", port, "v1/").Uri;
        }

        public void Disconnect() { token = null; Board = null; pending = null; }

        public async Task<string> Connect(string access)
        {
            if (!Regex.IsMatch(access ?? "", @"\A[A-Za-z0-9_-]{43}\z"))
                throw new InvalidOperationException("Masukkan token lengkap dari pengelola pasar lokal.");
            Disconnect();
            token = access;
            return await Refresh();
        }

        public async Task<string> Refresh()
        {
            Board = null;
            if (!HasSession) throw new InvalidOperationException("Hubungkan akun pasar dahulu.");
            try
            {
                var next = await NetworkHttp.Request<NetworkBoard>(endpoint, token, null);
                if (next.id <= 0 || next.lots == null || next.reference <= 0) throw new FormatException();
                next.contracts = next.contracts ?? new NetworkContract[0];
                next.traders = next.traders ?? new string[0];
                NetworkGuild.Fill(next);
                Board = next;
                return "Papan pasar diperbarui.";
            }
            catch (NetworkRejection error)
            {
                if (error.Status == 401 || error.Status == 403) Disconnect();
                throw new InvalidOperationException(error.Message);
            }
        }

        public Task<string> Sell(int kg, int price) => Place(JsonUtility.ToJson(new SellCommand { kg=kg, price=price }));
        public Task<string> Take(long lot, bool cancel) => Place(JsonUtility.ToJson(
            new LotCommand { lot=lot, action=cancel ? "cancel" : "buy" }));

        private Task<string> Place(string command)
        {
            if (Board == null || HasPending) throw new InvalidOperationException("Sinkronkan akun pasar dahulu.");
            pending = command;
            return Retry();
        }

        public async Task<string> Retry()
        {
            if (Board == null || !HasPending) throw new InvalidOperationException("Pesanan belum siap dikirim.");
            string message;
            try
            {
                var reply = await NetworkHttp.Request<NetworkReceipt>(endpoint, token, pending);
                if (string.IsNullOrEmpty(reply.message)) throw new FormatException();
                message = reply.message;
                pending = null;
            }
            catch (NetworkRejection error) when (error.Status >= 400 && error.Status < 500)
            {
                pending = null;
                if (error.Status == 401) { Disconnect(); throw new InvalidOperationException(error.Message); }
                message = error.Message;
            }
            catch
            {
                // The server may have committed. Authenticate again, then retry the identical body.
                Board = null;
                throw;
            }
            await Refresh();
            return message;
        }
    }
}
