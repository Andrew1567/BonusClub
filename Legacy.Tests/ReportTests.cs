using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace FleetMaintenance.Tests
{
    public class ReportTests
    {
        public ReportTests()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private static FleetRepository RepoWithOverdueOil()
        {
            var now = DateTime.Now;
            var repo = new FleetRepository();
            repo.Add(new Vehicle
            {
                Id = 1, Plate = "AA1", Type = "car", Odometer = 50000,
                LastOilKm = 38000, LastOilDate = now,
                LastBrakeKm = 50000, LastBrakeDate = now,
                LastInspKm = 50000, LastInspDate = now
            });
            return repo;
        }

        // Характеризаційний: у звіті націнка за прострочення йде
        // двічі (Estimate уже додав 15 %, Build додає ще раз): TD-03.
        [Fact]
        public void Build_OverdueOil_AppliesSurchargeTwice()
        {
            var report = new FleetReport(RepoWithOverdueOil());

            var text = report.Build(1, true, false, false, false);

            var line = text.Split('\n')
                .Single(l => l.StartsWith("Total: "));
            var total = decimal.Parse(line.Substring(7).Trim(),
                CultureInfo.InvariantCulture);
            Assert.Equal(105.80m, total);
        }

        [Fact]
        public void Build_ShortForm_HasPlateAndKindOnly()
        {
            var report = new FleetReport(RepoWithOverdueOil());

            var text = report.Build(1, false, false, false, true);

            Assert.Contains("AA1 oil", text);
            Assert.DoesNotContain("OVERDUE", text);
            Assert.Contains("Tasks: 1", text);
        }
    }
}
