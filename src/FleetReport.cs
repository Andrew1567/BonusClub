using System;
using System.Collections.Generic;
using System.Text;

namespace FleetMaintenance
{
    public class FleetReport
    {
        private const decimal OverdueSurcharge = 1.15m;
        private MaintenancePlanner _planner = new MaintenancePlanner();
        private FleetRepository _repo;

        public FleetReport(FleetRepository repo)
        {
            _repo = repo;
        }

        public string Build(int fleetSize, bool withCosts,
            bool onlyDue, bool print, bool shortForm)
        {
            var sb = new StringBuilder();
            decimal total = 0m;
            int count = 0;
            foreach (var v in _repo.Items)
            {
                var tasks = _planner.Plan(v, fleetSize, false);
                if (onlyDue && tasks.Count == 0)
                {
                    continue;
                }
                foreach (var t in tasks)
                {
                    count++;
                    if (shortForm)
                    {
                        sb.AppendLine(v.Plate + " " + t.Kind);
                    }
                    else
                    {
                        sb.AppendLine(v.Plate + " " + t.Kind + " "
                            + t.State + " " + t.Reason);
                    }
                    if (withCosts)
                    {
                        total += ReportedCost(t);
                    }
                }
            }
            sb.AppendLine("Tasks: " + count);
            if (withCosts)
            {
                sb.AppendLine("Total: " + total);
            }
            if (print)
            {
                Console.WriteLine(sb.ToString());
            }
            return sb.ToString();
        }

        private decimal ReportedCost(ServiceTask t)
        {
            decimal c = t.Cost;
            if (t.State == "OVERDUE")
            {
                c = c * OverdueSurcharge;
            }
            return c;
        }

        public string Summary(List<ServiceTask> tasks)
        {
            int soon = 0;
            int due = 0;
            int over = 0;
            foreach (var t in tasks)
            {
                if (t.State == "DUE_SOON")
                {
                    soon++;
                }
                else if (t.State == "DUE")
                {
                    due++;
                }
                else if (t.State == "OVERDUE")
                {
                    over++;
                }
            }
            return "soon=" + soon + ", due=" + due
                + ", overdue=" + over;
        }
    }
}