using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services
{
    public sealed partial class NetworkMarket
    {
        /// <summary>Offers another trader a fixed weight at a fixed price every game day, for 3 to 10 game days.</summary>
        public Task<string> Offer(string buyer, int kg, int price, int days) => Place(JsonUtility.ToJson(
            new OfferCommand { buyer = buyer, kg = kg, price = price, days = days }));

        /// <summary>The buyer accepts an offer, which locks the money for the first delivery.</summary>
        public Task<string> Accept(long contract) => Place(JsonUtility.ToJson(
            new ContractCommand { contract = contract, action = "accept" }));

        /// <summary>Withdraws an offer for free, or ends a running contract for 10% of what is left of it.</summary>
        public Task<string> End(long contract) => Place(JsonUtility.ToJson(
            new ContractCommand { contract = contract, action = "end" }));
    }
}
