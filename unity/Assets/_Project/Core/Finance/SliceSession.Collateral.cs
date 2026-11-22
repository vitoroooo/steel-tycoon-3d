using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private sealed class SecurityAsset
        {
            internal readonly string Id, Label;
            internal readonly double Value;
            internal readonly Action Take;
            internal SecurityAsset(string id, string label, double value, Action take)
            { Id = id; Label = label; Value = value; Take = take; }
        }

        public bool HasScale => Yard.bandar && !accounts.scaleSeized;
        public bool HasLoader => Yard.bandar && !accounts.loaderSeized;
        private int Uses(string id) => accounts.activity.FirstOrDefault(a => a.id == id)?.uses ?? 0;

        private void UseAsset(string id)
        {
            var activity = Array.Find(accounts.activity, a => a.id == id);
            if (activity == null)
            {
                activity = new AssetUse { id = id };
                Array.Resize(ref accounts.activity, accounts.activity.Length + 1);
                accounts.activity[accounts.activity.Length - 1] = activity;
            }
            if (activity.uses < int.MaxValue) activity.uses++;
        }

        public void ReplaceYardTool(bool loader, ConfigValues prices)
        {
            RequireNear(Rules.Home);
            if (!Yard.bandar || !(loader ? accounts.loaderSeized : accounts.scaleSeized))
                throw new InvalidOperationException("Alat ini belum perlu diganti.");
            Pay(prices.Positive(loader ? "loader" : "scale"));
            Yard.Machine(loader,0);
            Yard.machines[loader ? 1 : 0] = new Equipment.MachineState { randomState = loader ? 73 : 31 };
            if (loader) accounts.loaderSeized = false; else accounts.scaleSeized = false;
        }

        public double BusinessAssetValue(ConfigValues prices) => Collateral(prices).Sum(a => a.Value) + WorkingCapital();

        private IEnumerable<SecurityAsset> Collateral(ConfigValues v)
        {
            if (Magnet) yield return new SecurityAsset("magnet", "magnet", v.Positive("magnet"), () => Magnet = false);
            if (Xrf) yield return new SecurityAsset("xrf", "XRF", v.Positive("xrf"), () => Xrf = false);
            if (HasScale) yield return new SecurityAsset("scale", "timbangan", v.Positive("scale"),
                () => accounts.scaleSeized = true);
            if (HasLoader) yield return new SecurityAsset("loader", "loader", v.Positive("loader"),
                () => accounts.loaderSeized = true);
            if (Yard.hangar) yield return new SecurityAsset("roof", "hanggar", v.Positive("roof"), () => Yard.hangar = false);
            if (Yard.paved) yield return new SecurityAsset("pave", "cor beton", v.Positive("pave"), () => Yard.paved = false);
            foreach (var motor in Fleet.All)
            {
                if (!motor.Owned) continue;
                yield return new SecurityAsset(motor.Rules.Id, motor.Rules.Label, motor.Rules.Price,
                    () => TakeVehicle(motor));
            }
        }
    }
}
