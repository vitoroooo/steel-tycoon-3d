using Steel.Core.State;
using Steel.Core.Trade.Street;
using Steel.Services.Map;

namespace Steel.Services
{
    /// <summary>Where the doors that sell scrap stand and which place on the plan each belongs to, read from Sellers.json.</summary>
    public sealed class StreetLayout
    {
        private readonly ConfigValues values;
        private MapPlace[] places;
        public int Count { get; }

        public StreetLayout(IConfigSource source)
        {
            values = new ConfigValues(source.Read("Sellers"));
            Count = values.Integer("count");
        }

        public SliceLocation Position(int index) => new SliceLocation((float)values.Get("s" + index + "x"),
            (float)values.Get("s" + index + "z"));

        public SellerKind Kind(int index) => (SellerKind)values.Integer("s" + index + "kind");

        /// <summary>Name of the place on the plan the door stands in.</summary>
        public string Place(string id)
        {
            places = places ?? MapPlan.Load().places;
            int place = values.Integer(id + "place");
            return place < places.Length ? places[place].name + " " + places[place].detail.Split('&', ',')[0].Trim() : "Sukabaja";
        }
    }
}
