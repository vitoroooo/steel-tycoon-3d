namespace Steel.Core.Events
{
    /// <summary>"Temuan barang reuse" (PRD 5.2, appendix E): what sorting a deposit turned up, and what was kept for a special order.</summary>
    public sealed partial class EventSave
    {
        /// <summary>A find waiting for an answer: its weight, where in the 160-250% price band it sits (0..1), and when it sells itself.</summary>
        public double reuseKg, reuseQuality, reuseUntil;
        /// <summary>Reuse goods kept in the yard for a special SPK.</summary>
        public double reuseStore;
        /// <summary>A special SPK for the kept goods: kg wanted, price per kg, deadline; and the last game day one was rolled for.</summary>
        public bool reuseOrder;
        public double reuseOrderKg, reuseOrderPrice, reuseOrderUntil;
        public long reuseDay = -1;

        public bool ReuseWaiting => reuseKg > 0;

        private bool ReuseBroken()
        {
            bool Bad(double value) => !(value >= 0) || double.IsInfinity(value);
            return Bad(reuseKg) || Bad(reuseQuality) || reuseQuality > 1 || Bad(reuseUntil) || Bad(reuseStore) ||
                Bad(reuseOrderKg) || Bad(reuseOrderPrice) || Bad(reuseOrderUntil) ||
                reuseOrder && !(reuseOrderKg > 0 && reuseOrderPrice > 0);
        }
    }
}
