using System;

namespace FleetMaintenance
{
    public class CostCalculator
    {
        public decimal Estimate(string kind, string type,
            string state, int fleetSize)
        {
            decimal cost = 0m;
            if (kind == "oil")
            {
                cost = 80m;
                if (type == "van")
                {
                    cost = 110m;
                }
                else if (type == "truck")
                {
                    cost = 150m;
                }
            }
            else if (kind == "brake")
            {
                cost = 200m;
                if (type == "van")
                {
                    cost = 260m;
                }
                else if (type == "truck")
                {
                    cost = 420m;
                }
            }
            else if (kind == "insp")
            {
                cost = 60m;
                if (type == "truck")
                {
                    cost = 90m;
                }
            }
            if (state == "OVERDUE")
            {
                cost = cost * 1.15m;
            }
            if (fleetSize > 10)
            {
                cost = cost * 0.95m;
            }
            return Math.Round(cost, 2);
        }
    }
}
