using System.Threading.Tasks;
using UnityEngine;

namespace Steel.Services
{
    public sealed partial class NetworkMarket
    {
        /// <summary>A veteran chooses up to three legacies for this season, once (PRD 11.3).</summary>
        public Task<string> PickLegacies(string[] ids) =>
            Place(JsonUtility.ToJson(new LegacyCommand { picks = string.Join(",", ids) }));
    }
}
