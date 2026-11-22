using System;
using Steel.Core.State;

namespace Steel.Services
{
    public struct MillView
    {
        public bool Filed, Open, Ready, HasLine, Shredder;
        public double Wait, Input, Goods, FinishIn, Batch, Price, UpgradeCost, AmdalFee;
    }
    public partial interface IGameService
    {
        MillView MillInfo(int product, int grade);
        void ApplyAmdal(bool complete);
        void BuildMill();
        void InstallLine(int product);
        void ChargeFurnace(int product, int grade);
        double SellSteel(int product);
    }
    public sealed partial class LocalGameService
    {
        private readonly ConfigValues millRules;
        public MillView MillInfo(int product,int grade)
        {
            if (product < 0 || product > 2 || grade < 0 || grade > 3) throw new ArgumentOutOfRangeException();
            var m = session.Mill;
            return new MillView { Filed = m.filed, Open = m.opened, Ready = m.filed && GameHours >= m.permitAt,
                Wait = Math.Max(0,m.permitAt-GameHours), Input = session.Yard.stock[grade], Goods = m.goods[product],
                HasLine = product == 0 || (product == 1 ? m.rolling : m.bolts), Shredder = m.shredder,
                FinishIn = Math.Max(0,m.completedAt-GameHours), Batch = m.pendingKg,
                Price = Prices.Ibn * (product == 1 ? millRules.Positive("beamRate") :
                    product == 2 ? millRules.Positive("boltRate") : 1), AmdalFee = millRules.Positive("amdal"),
                UpgradeCost = millRules.Positive(product == 1 ? "rolling" : product == 2 ? "bolts" : "shredder") };
        }
        public void ApplyAmdal(bool complete) => session.FileAmdal(millRules,complete,GameHours);
        public void BuildMill() => session.OpenMill(millRules,GameHours);
        public void InstallLine(int product) => session.EquipMill(millRules,product);
        public void ChargeFurnace(int product,int grade) => session.Melt(millRules,product,grade,GameHours);
        public double SellSteel(int product) => session.SellSteel(millRules,product,Prices.Ibn);
    }
}
