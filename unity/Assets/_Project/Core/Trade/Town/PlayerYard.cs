using System;

namespace Steel.Core.Trade.Town
{
    /// <summary>Another player's yard as its buying board shows it (PRD 5.2): what he pays per kg of each grade.</summary>
    [Serializable] public sealed class PlayerYard
    {
        public string owner;
        /// <summary>The numbered yard plot in the city where his board stands.</summary>
        public int plot;
        public double[] bids = new double[4];
        /// <summary>How much more his land holds.</summary>
        public double roomKg;
    }

    /// <summary>One load weighed in at another player's yard: kilograms and money per grade.</summary>
    [Serializable] public sealed class Delivery
    {
        public int plot;
        public double[] kg = new double[4], paid = new double[4];
        public double cost;
        public double Kg => kg[0] + kg[1] + kg[2] + kg[3];
        public double Pay => paid[0] + paid[1] + paid[2] + paid[3];
    }
}
