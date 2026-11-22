using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Steel.Services;
using Steel.Presentation.Input;
using Steel.Presentation.Vehicles;
namespace Steel.Presentation.UI
{
    public sealed partial class CityHud : MonoBehaviour
    {
        public Text CashText, LoadText, MarketText, PromptText, ActionText, RouteText, NoticeText, SaleText;
        public GameObject SalePanel;
        public RectTransform SafeRoot;
        private MotorDriver[] drivers;
        private GameHost host;
        private SliceInput input;
        private RouteDesk routes;
        private MarketBoard board;
        private SortingDesk sorting;
        internal void RefreshNow() => nextReadout = 0;
        private string lastSaveNotice;
        private readonly CultureInfo culture = CultureInfo.GetCultureInfo("id-ID");
        private string Money(double value) => "Rp " + value.ToString("N0", culture);
        private void Start()
        {
            host = FindObjectOfType<GameHost>(); input = FindObjectOfType<SliceInput>(); HudLayout.Build(this, input);
            drivers = FindObjectsOfType<MotorDriver>();
            board = new MarketBoard(this, host.Game, input);
            sorting = new SortingDesk(this, host.Game, input);
            new YardDesk(this, host.Game, input);
            routes = new RouteDesk(this, host.Game, input, board.Open);
            NoticeText.text = lastSaveNotice = host.SaveNotice;
        }
        private void Update()
        {
            routes.Tick();
            if (input.Interact) Act();
            if (!DisplayDue()) return;
            var v = host.Game.View; var near = host.Game.Nearby;
            if (lastSaveNotice != host.SaveNotice && (lastSaveNotice = host.SaveNotice) != null)
                NoticeText.text = lastSaveNotice;
            CashText.text = Money(v.Cash);
            LoadText.text = "Buku usaha ›    K" + v.Caste + "    " + v.CarriedKg + " / " + v.CapacityKg + " kg";
            MarketText.text = "IBN  " + Money(v.Ibn) + "/kg\n" + (host.Game.RainHere ? "Hujan · " : "") + "Hari " +
                (1 + Math.Floor(v.GameHours / 24)) + "   " + ((int)v.GameHours % 24).ToString("00") + ":" +
                ((int)(v.GameHours % 1 * 60)).ToString("00");
            RouteText.text = DartoDirections.Hint(host.Game, v);
            string action = ActionLabel(near, v);
            ActionText.text = action;
            ActionPad.blocksRaycasts = near != null; ActionPad.alpha = near != null ? 1 : 0;
            VehicleHud.Refresh(this, host.Game.Vehicle);
            PromptText.text = near == null ? "Geser kiri untuk bergerak · Usap kanan untuk melihat" :
                near.Title + "\n" + near.Detail;
        }
        private void Run(Action action)
        {
            try { action(); }
            catch (InvalidOperationException e) { NoticeText.text = e.Message; }
        }
        public void Act()
        {
            if (input.IsBlocked) return;
            nextReadout = 0;
            Run(() =>
            {
                var near = host.Game.Nearby;
                if (near == null) return;
                if (near.Id == "vehicle") NoticeText.text = VehicleHud.Toggle(drivers, host.Game.Vehicle);
                else if (near.Id == "cart") ToggleCart();
                else if (near.Id == "grant")
                    NoticeText.text = "Hadiah Pak Darto: " + Money(host.Game.ClaimDealerGrant()) + ". Pilih BELI TRISEDA.";
                else if (near.Id == "home" && host.Game.View.CarriedKg == 0) routes.Open();
                else if (near.Id == "buyer" || near.Id == "home" && host.Game.View.LoadSorted) board.Open();
                else if (near.Id == "home") sorting.Open(null);
                else if (near.Id == "factory") ShowOffer();
                else if (!near.Inspected) sorting.Open(near.Id);
                else { host.Game.PickUp(near.Id); NoticeText.text = "Rongsok dipungut. Sortir di lapak Anda."; }
            });
        }
        private void ToggleCart()
        {
            if (host.Game.Cart.Held) host.Game.ReleaseCart(); else host.Game.GrabCart();
            NoticeText.text = host.Game.Cart.Held ? "Gerobak didorong. Muatan sampai 50 kg." : "Gerobak diparkir.";
        }
        private void ShowOffer()
        {
            var offer = host.Game.PreviewSale(); input.IsBlocked = true; SalePanel.SetActive(true);
            SaleText.text = "PENAWARAN PABRIK AI\n" + offer.Kilograms + " kg  ·  " +
                Money(offer.Revenue / Math.Max(.001, offer.Kilograms)) + " / kg\nTotal: " + Money(offer.Revenue) +
                "\nBiaya barang dan jalan: " + Money(offer.Revenue - offer.Profit) +
                "\nMargin bersih: " + Money(offer.Profit) + " · Tanpa pajak NPC";
        }
        public void Confirm() => Run(() =>
        {
            double earned = host.Game.ConfirmSale(); Cancel(); NoticeText.text = "Terjual! Kas bertambah " + Money(earned);
        });
        public void Cancel() { SalePanel.SetActive(false); input.IsBlocked = false; }
    }
}
