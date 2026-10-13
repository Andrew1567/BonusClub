using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class MaintenancePlanner
    {
        private const double OverdueFactor = 1.1;
        private const double DueSoonFactor = 0.9;
        private CostCalculator _calc = new CostCalculator();

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
            oilKm = 10000; oilMonths = 12;
            brakeKm = 30000; brakeMonths = 24;
            inspKm = 20000; inspMonths = 12;
            
            if (type == "van")
            {
                oilKm = 15000;
                brakeKm = 40000;
                inspKm = 25000;
            }
            else if (type == "truck")
            {
                oilKm = 20000;
                oilMonths = 6;
                brakeKm = 50000;
                brakeMonths = 12;
                inspKm = 30000;
                inspMonths = 6;
            }
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
            return kmSince >= limitKm * factor || monthsSince >= limitMonths * factor;
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
            
            int last = 0;
            int step = 0;
            if (kind == "oil")
            {
                last = vehicle.LastOilKm;
                step = oilKm;
            }
            else if (kind == "brake")
            {
                last = vehicle.LastBrakeKm;
                step = brakeKm;
            }
            else if (kind == "insp")
            {
                last = vehicle.LastInspKm;
                step = inspKm;
            }
            else
            {
                return -1;
            }
            return last + step;
        }
    }
}