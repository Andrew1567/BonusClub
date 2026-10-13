using System;

namespace FleetMaintenance
{
    public class Program
    {
        private static string Ago(int months)
        {
            return DateTime.Now.AddMonths(-months).ToString("O");
        }

        public static void Main(string[] args)
        {
            var lines = new string[]
            {
                "1;AA1001KA;car;52000;40000;" + Ago(8),
                "2;AA2002KB;van;98000;80000;" + Ago(14),
                "3;AA3003KC;truck;250000;200000;" + Ago(7),
                "4;AA4004KE;car;12000;11000;" + Ago(2),
                "bad;line",
                "5;AA5005KH;van;60000;55000;" + Ago(11)
            };
            var repo = new FleetRepository();
            foreach (var v in new FleetImporter().LoadAll(lines))
            {
                repo.Add(v);
            }
            var planner = new MaintenancePlanner();
            foreach (var v in repo.Items)
            {
                planner.Plan(v, repo.Items.Count, true);
            }
            var report = new FleetReport(repo);
            report.Build(repo.Items.Count, true, true, true, false);
        }
    }
}
