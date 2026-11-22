using System;
using System.Linq;
using Steel.Core.Fleet;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private static double CargoCost(Backpack cargo) => cargo.Items.Sum(p => p.AcquisitionCost);
        private double WorkingCapital() => Yard.payout + accounts.stockCost.Sum() +
            CargoCost(backpack) + CargoCost(Cart.Cargo) + CargoCost(Covered) +
            Fleet.All.Sum(m => CargoCost(m.Cargo)) + Routes.Orders.Sum(o => o.UnitCost * o.KgLeft);

        private void TakeVehicle(MotorState motor)
        {
            string id = motor.Rules.Id;
            double inTransit = Routes.Orders.Where(o => o.Carrier == id && o.SecondsLeft > 0)
                .Sum(o => o.UnitCost * o.KgLeft);
            CreditCash(CargoCost(motor.Cargo) + inTransit);
            Routes.CancelInTransit(id);
            motor.Repossess();
        }

        private void LiquidateStock()
        {
            if (Yard.payout > 0) CollectShipment();
            if (CooperativeDebt == 0) return;
            CreditCash(WorkingCapital());
            backpack.Clear(); Cart.Cargo.Clear(); Covered.Clear(); Routes.ForfeitOrders();
            foreach (var motor in Fleet.All) motor.Cargo.Clear();
            Array.Clear(accounts.stockCost, 0, accounts.stockCost.Length);
            Array.Clear(accounts.knownKg, 0, accounts.knownKg.Length);
            Array.Clear(Yard.stock, 0, Yard.stock.Length);
            Yard.exposed.Clear();
        }
    }
}
