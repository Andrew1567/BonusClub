using System.Collections.Generic;

namespace FleetMaintenance
{
    public static class ServicePolicy
    {
        public const double OverdueFactor = 1.1;
        public const double DueSoonFactor = 0.9;
        public const decimal OverdueSurcharge = 1.15m;
        public const decimal FleetDiscount = 0.95m;
        public const int FleetThreshold = 10;

        public static readonly Dictionary<string, Dictionary<string, decimal>> Prices = new Dictionary<string, Dictionary<string, decimal>>
        {
            { "oil", new Dictionary<string, decimal> { { "default", 80m }, { "van", 110m }, { "truck", 150m } } },
            { "brake", new Dictionary<string, decimal> { { "default", 200m }, { "van", 260m }, { "truck", 420m } } },
            { "insp", new Dictionary<string, decimal> { { "default", 60m }, { "van", 60m }, { "truck", 90m } } }
        };

        public static readonly Dictionary<string, int[]> OilLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 10000, 12 } }, { "van", new[] { 15000, 12 } }, { "truck", new[] { 20000, 6 } }
        };
        
        public static readonly Dictionary<string, int[]> BrakeLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 30000, 24 } }, { "van", new[] { 40000, 24 } }, { "truck", new[] { 50000, 12 } }
        };
        
        public static readonly Dictionary<string, int[]> InspLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 20000, 12 } }, { "van", new[] { 25000, 12 } }, { "truck", new[] { 30000, 6 } }
        };
    }
}