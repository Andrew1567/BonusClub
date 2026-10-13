using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class MaintenancePlanner
    {
        private const double OverdueFactor = 1.1;
        private const double DueSoonFactor = 0.9;
        private CostCalculator _calc = new CostCalculator();

        private readonly Dictionary<string, int[]> _oilLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 10000, 12 } }, { "van", new[] { 15000, 12 } }, { "truck", new[] { 20000, 6 } }
        };
        
        private readonly Dictionary<string, int[]> _brakeLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 30000, 24 } }, { "van", new[] { 40000, 24 } }, { "truck", new[] { 50000, 12 } }
        };
        
        private readonly Dictionary<string, int[]> _inspLimits = new Dictionary<string, int[]>
        {
            { "default", new[] { 20000, 12 } }, { "van", new[] { 25000, 12 } }, { "truck", new[] { 30000, 6 } }
        };

        public List<ServiceTask> Plan(Vehicle vehicle, int fleetSize,
            bool verbose, DateTime? now = null)
        {
            var result = new List<ServiceTask>();
            if (vehicle == null || !vehicle.Active)
            {
                return result;
            }
            var current = now ?? DateTime.Now;
            
            GetLimits(vehicle.Type, out int oilKm, out int oilMonths, out int brakeKm, out int brakeMonths, out int inspKm, out int inspMonths);

            CheckTask(result, vehicle, "oil", oilKm, oilMonths, vehicle.LastOilKm, vehicle.LastOilDate, current, fleetSize, verbose);
            CheckTask(result, vehicle, "brake", brakeKm, brakeMonths, vehicle.LastBrakeKm, vehicle.LastBrakeDate, current, fleetSize, verbose);
            CheckTask(result, vehicle, "insp", inspKm, inspMonths, vehicle.LastInspKm, vehicle.LastInspDate, current, fleetSize, verbose);

            return result;
        }

        private void GetLimits(string type, out int oilKm, out int oilMonths, out int brakeKm, out int brakeMonths, out int inspKm, out int inspMonths)
        {
            var oil = _oilLimits.ContainsKey(type) ? _oilLimits[type] : _oilLimits["default"];
            var brake = _brakeLimits.ContainsKey(type) ? _brakeLimits[type] : _brakeLimits["default"];
            var insp = _inspLimits.ContainsKey(type) ? _inspLimits[type] : _inspLimits["default"];

            oilKm = oil[0]; oilMonths = oil[1];
            brakeKm = brake[0]; brakeMonths = brake[1];
            inspKm = insp[0]; inspMonths = insp[1];
        }

        private void CheckTask(List<ServiceTask> result, Vehicle vehicle, string kind, int limitKm, int limitMonths, int lastKm, DateTime lastDate, DateTime current, int fleetSize, bool verbose)
        {
            int kmSince = vehicle.Odometer - lastKm;
            int monthsSince = MonthsSince(lastDate, current);
            string state = StateFor(kmSince, monthsSince, limitKm, limitMonths);
            CreateTask(result, vehicle, kind, state, kmSince, monthsSince, fleetSize, verbose);
        }

        private void CreateTask(List<ServiceTask> result, Vehicle vehicle, string kind, string state, int kmSince, int monthsSince, int fleetSize, bool verbose)
        {
            if (state == null)
            {
                return;
            }
            var t = new ServiceTask();
            t.VehicleId = vehicle.Id;
            t.Kind = kind;
            t.State = state;
            t.Cost = _calc.Estimate(kind, vehicle.Type, state, fleetSize);
            t.Reason = "km=" + kmSince + ", months=" + monthsSince;
            result.Add(t);
            
            Log(vehicle.Plate, kind, state, t.Reason, verbose);
        }

        private void Log(string plate, string kind, string state, string reason, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(plate + ": " + kind + " " + state);
                if (state == "OVERDUE")
                {
                    Console.WriteLine("  !!! overdue " + reason);
                }
            }
        }

        private string StateFor(int kmSince, int monthsSince, int limitKm, int limitMonths)
        {
            if (IsReached(kmSince, monthsSince, limitKm, limitMonths, OverdueFactor))
            {
                return "OVERDUE";
            }
            if (IsReached(kmSince, monthsSince, limitKm, limitMonths, 1.0))
            {
                return "DUE";
            }
            if (IsReached(kmSince, monthsSince, limitKm, limitMonths, DueSoonFactor))
            {
                return "DUE_SOON";
            }
            return null;
        }

        private bool IsReached(int kmSince, int monthsSince, int limitKm, int limitMonths, double factor)
        {
            bool byKm = kmSince >= limitKm * factor;
            bool byMonths = monthsSince >= limitMonths * factor;
            return byKm || byMonths;
        }

        private int MonthsSince(DateTime past, DateTime current)
        {
            return (current.Year - past.Year) * 12 + current.Month - past.Month;
        }

        public bool IsAnyDue(Vehicle vehicle, DateTime? now = null)
        {
            var current = now ?? DateTime.Now;
            GetLimits(vehicle.Type, out int oilKm, out int oilMonths, out int brakeKm, out int brakeMonths, out int _, out int _);
            
            int kmSince = vehicle.Odometer - vehicle.LastOilKm;
            int monthsSince = MonthsSince(vehicle.LastOilDate, current);
            if (kmSince >= oilKm || monthsSince >= oilMonths)
            {
                return true;
            }
            
            kmSince = vehicle.Odometer - vehicle.LastBrakeKm;
            monthsSince = MonthsSince(vehicle.LastBrakeDate, current);
            return kmSince >= brakeKm || monthsSince >= brakeMonths;
        }

        public int NextDueKm(Vehicle vehicle, string kind)
        {
            GetLimits(vehicle.Type, out int oilKm, out int _, out int brakeKm, out int _, out int inspKm, out int _);
            
            var config = new Dictionary<string, int[]>
            {
                { "oil", new[] { vehicle.LastOilKm, oilKm } },
                { "brake", new[] { vehicle.LastBrakeKm, brakeKm } },
                { "insp", new[] { vehicle.LastInspKm, inspKm } }
            };
            
            if (config.ContainsKey(kind))
            {
                var data = config[kind];
                return data[0] + data[1];
            }
            return -1;
        }
    }
}