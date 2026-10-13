using System;

namespace FleetMaintenance
{
    public class CostCalculator
    {
        public decimal Estimate(string kind, string type, string state, int fleetSize)
        {
            if (!ServicePolicy.Prices.ContainsKey(kind))
            {
                return 0m;
            }
            
            var kindPrices = ServicePolicy.Prices[kind];
            decimal cost = kindPrices.ContainsKey(type) ? kindPrices[type] : kindPrices["default"];

            if (state == "OVERDUE")
            {
                cost = cost * ServicePolicy.OverdueSurcharge;
            }
            if (fleetSize > ServicePolicy.FleetThreshold)
            {
                cost = cost * ServicePolicy.FleetDiscount;
            }
            return Math.Round(cost, 2);
        }
    }
}