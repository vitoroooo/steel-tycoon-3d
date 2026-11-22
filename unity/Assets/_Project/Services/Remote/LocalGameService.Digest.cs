using System;
using System.Globalization;
using System.Linq;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        /// <summary>The game clock in real seconds since the career began.</summary>
        public double ClockSeconds => clock.RealSeconds;

        /// <summary>The career's money and goods in one short line. Phone and server compare it after every command; rounded,
        /// so the last digits of a phone's arithmetic never count as a difference.</summary>
        public string Digest()
        {
            var s = session;
            return string.Join("|", new object[]
            {
                Math.Round(s.Cash), s.Caste, Math.Round(View.CarriedKg), Math.Round(s.Yard.stock.Sum()), s.Yard.reputation,
                Math.Round(s.Mill.goods.Sum()), Math.Round(s.Risk.heat), s.Events.sequence, Math.Round(s.Events.reuseStore),
                s.Fleet.All.Count(m => m.Owned)
            }.Select(value => Convert.ToString(value, CultureInfo.InvariantCulture)));
        }
    }
}
