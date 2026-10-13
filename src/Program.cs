using System;
using System.Linq; // Важливий рядок для роботи .Count()

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
            
            // Використовуємо All.Count() завдяки System.Linq
            int fleetSize = repo.All.Count(); 
            
            foreach (var v in repo.All) // Змінили Items на All
            {
                planner.Plan(v, fleetSize, true);
            }
            var report = new FleetReport(repo);
            report.Build(fleetSize, true, true, true, false);
        }
    }
}