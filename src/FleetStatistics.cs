using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetStatistics
    {
        public double AverageOdometer(List<Vehicle> fleet)
        {
            int sum = 0;
            int n = 0;
            foreach (var v in fleet)
            {
                if (v.Active)
                {
                    sum += v.Odometer;
                    n++;
                }
            }
            if (n == 0)
            {
                return 0;
            }
            return (double)sum / n;
        }

        public Vehicle MostMileage(List<Vehicle> fleet)
        {
            Vehicle best = null;
            foreach (var v in fleet)
            {
                if (best == null || v.Odometer > best.Odometer)
                {
                    best = v;
                }
            }
            return best;
        }

        public Dictionary<string, int> ByType(List<Vehicle> fleet)
        {
            var d = new Dictionary<string, int>();
            foreach (var v in fleet)
            {
                if (!d.ContainsKey(v.Type))
                {
                    d[v.Type] = 0;
                }
                d[v.Type] = d[v.Type] + 1;
            }
            return d;
        }
    }
}
