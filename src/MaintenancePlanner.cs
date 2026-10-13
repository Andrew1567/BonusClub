using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class MaintenancePlanner
    {
        private CostCalculator _calc = new CostCalculator();

        public List<ServiceTask> Plan(Vehicle v, int fleetSize,
            bool verbose)
        {
            var result = new List<ServiceTask>();
            if (v == null)
            {
                return result;
            }
            if (!v.Active)
            {
                return result;
            }
            var now = DateTime.Now;
            int oilKm = 10000, oilMonths = 12;
            int brakeKm = 30000, brakeMonths = 24;
            int inspKm = 20000, inspMonths = 12;
            if (v.Type == "van")
            {
                oilKm = 15000;
                brakeKm = 40000;
                inspKm = 25000;
            }
            else if (v.Type == "truck")
            {
                oilKm = 20000;
                oilMonths = 6;
                brakeKm = 50000;
                brakeMonths = 12;
                inspKm = 30000;
                inspMonths = 6;
            }

            int km = v.Odometer - v.LastOilKm;
            int mo = (now.Year - v.LastOilDate.Year) * 12
                + now.Month - v.LastOilDate.Month;
            string st = null;
            if (km >= oilKm * 1.1 || mo >= oilMonths * 1.1)
            {
                st = "OVERDUE";
            }
            else if (km >= oilKm || mo >= oilMonths)
            {
                st = "DUE";
            }
            else if (km >= oilKm * 0.9 || mo >= oilMonths * 0.9)
            {
                st = "DUE_SOON";
            }
            if (st != null)
            {
                var t = new ServiceTask();
                t.VehicleId = v.Id;
                t.Kind = "oil";
                t.State = st;
                t.Cost = _calc.Estimate(
                    "oil", v.Type, st, fleetSize);
                t.Reason = "km=" + km + ", months=" + mo;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(v.Plate + ": oil " + st);
                    if (st == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }
            // old rule: tires every 40000 km
            // if (km >= 40000) { tires = true; }

            km = v.Odometer - v.LastBrakeKm;
            mo = (now.Year - v.LastBrakeDate.Year) * 12
                + now.Month - v.LastBrakeDate.Month;
            st = null;
            if (km >= brakeKm * 1.1 || mo >= brakeMonths * 1.1)
            {
                st = "OVERDUE";
            }
            else if (km >= brakeKm || mo >= brakeMonths)
            {
                st = "DUE";
            }
            else if (km >= brakeKm * 0.9 || mo >= brakeMonths * 0.9)
            {
                st = "DUE_SOON";
            }
            if (st != null)
            {
                var t = new ServiceTask();
                t.VehicleId = v.Id;
                t.Kind = "brake";
                t.State = st;
                t.Cost = _calc.Estimate(
                    "brake", v.Type, st, fleetSize);
                t.Reason = "km=" + km + ", months=" + mo;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(v.Plate + ": brake " + st);
                    if (st == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }

            km = v.Odometer - v.LastInspKm;
            mo = (now.Year - v.LastInspDate.Year) * 12
                + now.Month - v.LastInspDate.Month;
            st = null;
            if (km >= inspKm * 1.1 || mo >= inspMonths * 1.1)
            {
                st = "OVERDUE";
            }
            else if (km >= inspKm || mo >= inspMonths)
            {
                st = "DUE";
            }
            else if (km >= inspKm * 0.9 || mo >= inspMonths * 0.9)
            {
                st = "DUE_SOON";
            }
            if (st != null)
            {
                var t = new ServiceTask();
                t.VehicleId = v.Id;
                t.Kind = "insp";
                t.State = st;
                t.Cost = _calc.Estimate(
                    "insp", v.Type, st, fleetSize);
                t.Reason = "km=" + km + ", months=" + mo;
                result.Add(t);
                if (verbose)
                {
                    Console.WriteLine(v.Plate + ": insp " + st);
                    if (st == "OVERDUE")
                    {
                        Console.WriteLine(
                            "  !!! overdue " + t.Reason);
                    }
                }
            }
            return result;
        }

        public bool IsAnyDue(Vehicle v)
        {
            var now = DateTime.Now;
            int oilKm = 10000;
            int oilMonths = 12;
            if (v.Type == "van")
            {
                oilKm = 15000;
            }
            else if (v.Type == "truck")
            {
                oilKm = 20000;
                oilMonths = 6;
            }
            int km = v.Odometer - v.LastOilKm;
            int mo = (now.Year - v.LastOilDate.Year) * 12
                + now.Month - v.LastOilDate.Month;
            if (km >= oilKm || mo >= oilMonths)
            {
                return true;
            }
            int brakeKm = 30000;
            int brakeMonths = 24;
            if (v.Type == "van")
            {
                brakeKm = 40000;
            }
            else if (v.Type == "truck")
            {
                brakeKm = 50000;
                brakeMonths = 12;
            }
            km = v.Odometer - v.LastBrakeKm;
            mo = (now.Year - v.LastBrakeDate.Year) * 12
                + now.Month - v.LastBrakeDate.Month;
            return km >= brakeKm || mo >= brakeMonths;
        }

        public int NextDueKm(Vehicle v, string kind)
        {
            int last = 0;
            int step = 0;
            if (kind == "oil")
            {
                last = v.LastOilKm;
                step = 10000;
                if (v.Type == "van")
                {
                    step = 15000;
                }
                else if (v.Type == "truck")
                {
                    step = 20000;
                }
            }
            else if (kind == "brake")
            {
                last = v.LastBrakeKm;
                step = 30000;
                if (v.Type == "van")
                {
                    step = 40000;
                }
                else if (v.Type == "truck")
                {
                    step = 50000;
                }
            }
            else if (kind == "insp")
            {
                last = v.LastInspKm;
                step = 20000;
                if (v.Type == "van")
                {
                    step = 25000;
                }
                else if (v.Type == "truck")
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
