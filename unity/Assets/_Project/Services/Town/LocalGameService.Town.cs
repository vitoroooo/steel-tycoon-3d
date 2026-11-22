using System;
using System.Linq;
using Steel.Core.State.Slice;
using Steel.Core.Trade.Town;
using Steel.Services.Map;

namespace Steel.Services
{
    /// <summary>A yard plot another player trades from, as the city shows it to this player.</summary>
    public struct PlayerYardView
    {
        public string Owner;
        public int Plot;
        public double[] Bids;
        /// <summary>Where to stand to sell, and what the carried load would fetch there (0 with the reason why not).</summary>
        public float X, Z;
        public double Offer;
        public string Why;
    }

    public partial interface IGameService
    {
        PlayerYardView[] PlayerYards { get; }
        double SellToPlayerYard(int plot);
    }

    public sealed partial class LocalGameService
    {
        private static MapSite[] plots;
        private static MapSite Plot(int number)
        {
            plots = plots ?? MapPlan.Load().sites.Where(site => site.kind == MapSite.PlayerLapak).ToArray();
            return number >= 1 && number <= plots.Length ? plots[number - 1] :
                throw new InvalidOperationException("Kavling tidak dikenal.");
        }
        private static WorldPoint PlotPost(int number)
        {
            var at = Plot(number).Point(MapSite.PostRight, MapSite.PostForward);
            return new WorldPoint(at.x, at.y);
        }

        public PlayerYardView[] PlayerYards => session.Yards.Select(yard =>
        {
            WorldPoint post = PlotPost(yard.plot);
            var view = new PlayerYardView { Owner = yard.owner, Plot = yard.plot, Bids = yard.bids, X = (float)post.X, Z = (float)post.Z };
            try { view.Offer = session.WeighFor(yard.plot).Pay; }
            catch (InvalidOperationException why) { view.Why = why.Message; }
            return view;
        }).ToArray();

        /// <summary>Another player's buying board, when the player stands at its post.</summary>
        private InteractionView NearbyYard()
        {
            foreach (PlayerYardView yard in PlayerYards)
                if (session.Position.Distance(new WorldPoint(yard.X, yard.Z)) <= session.Haul.DeskReach)
                    return new InteractionView { Id = "playeryard", Title = "Lapak " + yard.Owner + " (pemain)",
                        Detail = yard.Offer > 0 ? "Muatan Anda dibeli Rp" + yard.Offer.ToString("N0", Id) : yard.Why };
            return null;
        }
        private static readonly System.Globalization.CultureInfo Id = System.Globalization.CultureInfo.GetCultureInfo("id-ID");

        public double SellToPlayerYard(int plot) => session.DeliverTo(plot, PlotPost(plot));

        /// <summary>The boards the server shows this player; the budgets are known to the server alone.</summary>
        public void PostYards(PlayerYard[] yards, double[] canPay = null) => session.PostYards(yards, canPay);

        /// <summary>This player's own board for the others, or null while his yard is not trading.</summary>
        public PlayerYard Board(out double canPay)
        {
            canPay = session.Cash;
            var yard = session.Yard;
            if (!yard.bandar || GameHours >= yard.leaseUntil || !yard.bids.Any(bid => bid > 0)) return null;
            return new PlayerYard { bids = yard.bids.ToArray(), roomKg = Math.Max(0, business.Positive("capacityKg") - yard.stock.Sum()) };
        }

        public Delivery[] TakeDeliveries()
        {
            Delivery[] sent = session.Delivered.ToArray();
            session.Delivered.Clear();
            return sent;
        }

        public void TakeDelivery(Delivery note) => session.TakeDelivery(note);
    }
}
