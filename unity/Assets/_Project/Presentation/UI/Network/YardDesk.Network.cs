using System;
using System.Threading.Tasks;
using Steel.Services;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private enum NetworkTab { Account, Lots, Sell, Settings }
        private NetworkMarket network = new NetworkMarket();
        private NetworkTab networkTab;
        private int networkLot, networkRate = 100;
        private bool networkBusy;
        private string networkMessage = "";

        private void DrawNetwork()
        {
            feedback.text = networkMessage;
            if (networkBusy) { detail.text = "Menunggu jawaban " + network.Host + "…"; return; }
            var account = network.Board;
            if (account == null)
            {
                detail.text = "Pasar HMS 1 · " + network.Host + "\n" +
                    "Punya undangan? Pilih alamat HTTPS pengelola sebelum memasukkan token.";
                if (network.HasSession)
                {
                    Key(0,"Sambung ulang",() => NetworkRun(network.Refresh));
                    Key(2,"Lepas akun",NetworkLogout);
                }
                else
                {
                    Key(0,"Hubungkan akun",NetworkLogin);
                    Key(1,"Alamat pasar",NetworkAddress);
                }
                return;
            }
            if (network.HasPending)
            {
                detail.text = "Nota pesanan terakhir belum diterima. Periksa hasilnya dengan ID pesanan yang sama.";
                Key(0,"Periksa pesanan",() => NetworkRun(network.Retry));
                return;
            }
            if (networkTab == NetworkTab.Lots) { DrawNetworkLots(account); return; }
            if (networkTab == NetworkTab.Sell) { DrawNetworkSale(account); return; }
            detail.text = account.name + "   ·   Kas pasar " + Rp(account.cash) + "   ·   HMS 1 " + account.stock +
                " kg\nIBN " + Rp(account.ibn) + "/kg   ·   Acuan " + Rp(account.reference) +
                "/kg   ·   Jam kota " + account.gameHour + "\n" + account.lots.Length + " lot · " +
                account.buyers + " pelanggan berbeda · Pajak 2%";
            if (networkTab == NetworkTab.Settings)
            {
                Key(0,"Muat ulang",() => NetworkRun(network.Refresh));
                Key(1,"Lepas akun",NetworkLogout);
                Key(2,"Kembali",() => NetworkOpen(NetworkTab.Account)); return;
            }
            Key(0,"Papan lot",() => NetworkOpen(NetworkTab.Lots));
            Key(1,"Jual HMS 1",() => NetworkOpen(NetworkTab.Sell));
            Key(2,"Akun & sinkron",() => NetworkOpen(NetworkTab.Settings));
        }

        private void NetworkOpen(NetworkTab tab) { networkTab = tab; networkMessage = ""; }
        private void NetworkLogout() { network.Disconnect(); NetworkOpen(NetworkTab.Account); }

        private async void NetworkRun(Func<Task<string>> request)
        {
            if (networkBusy) return;
            networkBusy = true;
            try { networkMessage = await request(); }
            catch (InvalidOperationException error) { networkMessage = error.Message; }
            catch (Exception) { networkMessage = "Pasar lokal tidak dapat dihubungi. Periksa layanan dan sambungkan ulang."; }
            finally
            {
                networkBusy = false;
                if (panel != null && panel.gameObject.activeInHierarchy) Draw();
            }
        }
    }
}
