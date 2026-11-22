using System;
using Steel.Core.State;

namespace Steel.Core.Equipment
{
    public enum RepairMethod { Original, Donor, Emergency }

    public sealed class MaintenanceRules
    {
        public readonly double NormalWear, HeavyWear, HeavyAbove, RiskHours, BreakdownChance, AccidentChance;
        public readonly double ScaleCapacity, ScaleHours, TrailerCapacity, LoaderRate;
        private readonly double originalShare, emergencyShare, originalHours, donorHours, emergencyHours;
        private readonly double loaderPrice, scalePrice;

        public MaintenanceRules(string json, ConfigValues business)
        {
            var v = new ConfigValues(json);
            NormalWear = v.Positive("wearPerHour");
            HeavyWear = v.Positive("heavyWearPerHour");
            HeavyAbove = v.Fraction("heavyAbove");
            RiskHours = v.Positive("riskHours");
            BreakdownChance = v.Fraction("breakdownChance");
            AccidentChance = v.Fraction("accidentChance");
            ScaleCapacity = v.Positive("scaleCapacityKg");
            ScaleHours = v.Positive("scaleHoursPerLot");
            TrailerCapacity = v.Positive("trailerCapacityKg");
            LoaderRate = v.Positive("loaderKgPerHour");
            originalShare = v.Fraction("originalCostShare");
            emergencyShare = v.Fraction("emergencyCostShare");
            originalHours = v.Positive("originalHours");
            donorHours = v.Positive("donorHours");
            emergencyHours = v.Positive("emergencyHours");
            loaderPrice = business.Positive("loader");
            scalePrice = business.Positive("scale");
        }

        public double RepairHours(RepairMethod method)
        {
            switch (method)
            {
                case RepairMethod.Original: return originalHours;
                case RepairMethod.Donor: return donorHours;
                case RepairMethod.Emergency: return emergencyHours;
                default: throw new ArgumentOutOfRangeException(nameof(method));
            }
        }

        public double RepairCost(bool loader, RepairMethod method)
        {
            double price = loader ? loaderPrice : scalePrice;
            switch (method)
            {
                case RepairMethod.Original: return price * originalShare;
                case RepairMethod.Donor: return 0;
                case RepairMethod.Emergency: return price * emergencyShare;
                default: throw new ArgumentOutOfRangeException(nameof(method));
            }
        }
    }
}
