using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class CostCalculator
    {
        private const decimal OverdueSurcharge = 1.15m;
        private const decimal FleetDiscount = 0.95m;
        private const int FleetThreshold = 10;

        private readonly Dictionary<string, Dictionary<string, decimal>> _prices = new Dictionary<string, Dictionary<string, decimal>>
        {
            { "oil", new Dictionary<string, decimal> { { "default", 80m }, { "van", 110m }, { "truck", 150m } } },
            { "brake", new Dictionary<string, decimal> { { "default", 200m }, { "van", 260m }, { "truck", 420m } } },
            { "insp", new Dictionary<string, decimal> { { "default", 60m }, { "van", 60m }, { "truck", 90m } } }
        };

        public decimal Estimate(string kind, string type, string state, int fleetSize)
        {
            if (!_prices.ContainsKey(kind))
            {
                return 0m;
            }
            
            var kindPrices = _prices[kind];
            decimal cost = kindPrices.ContainsKey(type) ? kindPrices[type] : kindPrices["default"];

            if (state == "OVERDUE")
            {
                cost = cost * OverdueSurcharge;
            }
            if (fleetSize > FleetThreshold)
            {
                cost = cost * FleetDiscount;
            }
            return Math.Round(cost, 2);
        }
    }
}