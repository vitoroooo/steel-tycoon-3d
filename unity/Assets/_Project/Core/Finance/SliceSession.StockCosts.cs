using System;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public void RecordStockPurchase(int grade, double kg, double paid)
        {
            NumberRules.NonNegative(kg, nameof(kg));
            NumberRules.NonNegative(paid, nameof(paid));
            accounts.knownKg[grade] += kg;
            accounts.stockCost[grade] += paid;
            UseAsset("scale");
            if (Yard.hangar) UseAsset("roof");
        }

        // Called on successful dispatch with the stock weight before the shipment.
        // Old saves lack cost basis: only the documented fraction earns taxable margin.
        public void BookShipmentMargin(int grade, double shippedKg, double beforeKg, double net)
        {
            if (shippedKg <= 0 || shippedKg > beforeKg) throw new ArgumentOutOfRangeException(nameof(shippedKg));
            double fraction = shippedKg / beforeKg;
            double knownShare = Math.Min(1, accounts.knownKg[grade] / beforeKg);
            double cost = accounts.stockCost[grade] * fraction;
            accounts.shipmentMargin = net * knownShare - cost;
            accounts.stockCost[grade] -= cost;
            accounts.knownKg[grade] *= 1 - fraction;
            UseAsset("loader");
            if (Yard.paved) UseAsset("pave");
        }

        /// <summary>Takes stock off the yard's books as it is loaded; returns its cost and how much of it has a known cost.</summary>
        private (double Cost, double KnownKg) IssueStock(int grade, double kg)
        {
            double fraction = Math.Min(1, kg / Yard.stock[grade]), cost = accounts.stockCost[grade] * fraction;
            double known = Math.Min(kg, accounts.knownKg[grade] * fraction);
            accounts.stockCost[grade] -= cost; accounts.knownKg[grade] -= known;
            UseAsset("loader");
            if (Yard.paved) UseAsset("pave");
            return (cost, known);
        }

        private void CollectShipment()
        {
            CreditProceeds(Yard.payout, accounts.shipmentMargin);
            CreditSale(Yard.dispatchedKg);
            Yard.payout = 0;
            Yard.dispatchedKg = 0;
            accounts.shipmentMargin = 0;
        }
    }
}
