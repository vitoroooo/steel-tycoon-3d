using System;
using Steel.Core.State;
using Steel.Core.Trade.Street;

namespace Steel.Core.Trade.Spk
{
    /// <summary>Terms of the mill's work orders, read from Spk.json, and the board it posts each game day.</summary>
    public sealed class SpkRules
    {
        public const int Slots = 3, HoursPerDay = 24;
        public readonly double Bond, LatePerHour, LoaderCondition;
        public readonly int OnTimeReputation, FailReputation;
        private readonly ConfigValues values;

        public SpkRules(string json)
        {
            values = new ConfigValues(json);
            Bond = values.Fraction("bond"); LatePerHour = values.Fraction("latePerHour");
            LoaderCondition = values.Positive("loaderCondition");
            OnTimeReputation = values.Integer("onTimeReputation"); FailReputation = values.Integer("failReputation");
        }

        /// <summary>Hours past the deadline at which the late penalty has eaten the whole bond and the order fails.</summary>
        public double Grace => Bond / LatePerHour;
        public static long Day(double hour) => (long)Math.Floor(hour / HoursPerDay);

        /// <summary>The same board for everyone on the same day: a small, a medium and a large order.</summary>
        /// <param name="rate">The day's reference price per kg of a grade.</param>
        public SpkOffer[] Offers(long day, Func<int, double> rate)
        {
            var offers = new SpkOffer[Slots];
            for (int slot = 0; slot < Slots; slot++)
            {
                string key = "slot" + slot;
                double Roll(int salt) => SellerRoll.Of(slot, day, 500 + salt);
                double Between(string name, int salt) => values.Positive(key + "Min" + name) +
                    Roll(salt) * (values.Positive(key + "Max" + name) - values.Positive(key + "Min" + name));
                int grade = Roll(1) < .55 ? 0 : 1 + (int)(Roll(2) * 3);   // HMS 1 is what a mill asks for most
                double tonnes = Math.Round(Between("Tonnes", 3) / 5) * 5, days = Math.Round(Between("Days", 4));
                double premium = values.Positive("minPremium") + Roll(5) * (values.Positive("maxPremium") - values.Positive("minPremium"));
                offers[slot] = new SpkOffer(day * 10 + slot, grade, tonnes * 1000, Math.Round(rate(grade) * premium / 10) * 10,
                    days * HoursPerDay, values.Integer(key + "Reputation"));
            }
            return offers;
        }
    }
}
