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
            if (vehicle == null)
            {
                return result;
            }
            if (!vehicle.Active)
            {
                return result;
            }
            var current = now ?? DateTime.Now;
            int oilKm = 10000, oilMonths = 12;
            int brakeKm = 30000, brakeMonths = 24;
            int inspKm = 20000, inspMonths = 12;
            if (vehicle.Type == "van")
            {
                oilKm = 15000;
                brakeKm = 40000;
                inspKm = 25000;
            }
            else if (vehicle.Type == "truck")
            {
                oilKm = 20000;
                oilMonths = 6;
                brakeKm = 50000;
                brakeMonths = 12;
                inspKm = 30000;
                inspMonths = 6;
            }

            int kmSince = vehicle.Odometer - vehicle.LastOilKm;
            int monthsSince = (current.Year - vehicle.LastOilDate.Year) * 12
                + current.Month - vehicle.LastOilDate.Month;
            string state = StateFor(kmSince, monthsSince, oilKm, oilMonths);
            
            if (state != null)
            {
                var t = new ServiceTask();
                t.VehicleId = vehicle.Id;
                t.Kind = "oil";
                t.State = state;
                t.Cost = _calc.Estimate(
                    "oil", vehicle.Type, state, fleetSize);
                t.Reason = "km=" + kmSince + ", months=" + monthsSince;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(vehicle.Plate + ": oil " + state);
                    if (state == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }
            // old rule: tires every 40000 km
            // if (km >= 40000) { tires = true; }

            kmSince = vehicle.Odometer - vehicle.LastBrakeKm;
            monthsSince = (current.Year - vehicle.LastBrakeDate.Year) * 12
                + current.Month - vehicle.LastBrakeDate.Month;
            state = StateFor(kmSince, monthsSince, brakeKm, brakeMonths);
            
            if (state != null)
            {
                var t = new ServiceTask();
                t.VehicleId = vehicle.Id;
                t.Kind = "brake";
                t.State = state;
                t.Cost = _calc.Estimate(
                    "brake", vehicle.Type, state, fleetSize);
                t.Reason = "km=" + kmSince + ", months=" + monthsSince;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(vehicle.Plate + ": brake " + state);
                    if (state == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }

            kmSince = vehicle.Odometer - vehicle.LastInspKm;
            monthsSince = (current.Year - vehicle.LastInspDate.Year) * 12
                + current.Month - vehicle.LastInspDate.Month;
            state = StateFor(kmSince, monthsSince, inspKm, inspMonths);
            
            if (state != null)
            {
                var t = new ServiceTask();
                t.VehicleId = vehicle.Id;
                t.Kind = "insp";
                t.State = state;
                t.Cost = _calc.Estimate(
                    "insp", vehicle.Type, state, fleetSize);
                t.Reason = "km=" + kmSince + ", months=" + monthsSince;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(vehicle.Plate + ": insp " + state);
                    if (state == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }
            return result;
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

        public bool IsAnyDue(Vehicle vehicle, DateTime? now = null)
        {
            var current = now ?? DateTime.Now;
            int oilKm = 10000;
            int oilMonths = 12;
            if (vehicle.Type == "van")
            {
                oilKm = 15000;
            }
            else if (vehicle.Type == "truck")
            {
                oilKm = 20000;
                oilMonths = 6;
            }
            int kmSince = vehicle.Odometer - vehicle.LastOilKm;
            int monthsSince = (current.Year - vehicle.LastOilDate.Year) * 12
                + current.Month - vehicle.LastOilDate.Month;
            if (kmSince >= oilKm || monthsSince >= oilMonths)
            {
                return true;
            }
            int brakeKm = 30000;
            int brakeMonths = 24;
            if (vehicle.Type == "van")
            {
                brakeKm = 40000;
            }
            else if (vehicle.Type == "truck")
            {
                brakeKm = 50000;
                brakeMonths = 12;
            }
            kmSince = vehicle.Odometer - vehicle.LastBrakeKm;
            monthsSince = (current.Year - vehicle.LastBrakeDate.Year) * 12
                + current.Month - vehicle.LastBrakeDate.Month;
            return kmSince >= brakeKm || monthsSince >= brakeMonths;
        }

        public int NextDueKm(Vehicle vehicle, string kind)
        {
            int last = 0;
            int step = 0;
            if (kind == "oil")
            {
                last = vehicle.LastOilKm;
                step = 10000;
                if (vehicle.Type == "van")
                {
                    step = 15000;
                }
                else if (vehicle.Type == "truck")
                {
                    step = 20000;
                }
            }
            else if (kind == "brake")
            {
                last = vehicle.LastBrakeKm;
                step = 30000;
                if (vehicle.Type == "van")
                {
                    step = 40000;
                }
                else if (vehicle.Type == "truck")
                {
                    step = 50000;
                }
            }
            else if (kind == "insp")
            {
                last = vehicle.LastInspKm;
                step = 20000;
                if (vehicle.Type == "van")
                {
                    step = 25000;
                }
                else if (vehicle.Type == "truck")
                {
                    step = 30000;
                }
            }
            else
            {
                return -1;
            }
            return last + step;
        }
    }
}