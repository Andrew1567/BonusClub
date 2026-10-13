using System;
using System.Collections.Generic;
using Xunit;

namespace FleetMaintenance.Tests
{
    public class ImporterRepositoryTests
    {
        [Fact]
        public void Parse_ValidLine_FillsAllLastServiceFields()
        {
            var line = "7;AA7;van;60000;55000;2025-06-15";
            var v = new FleetImporter().Parse(line);

            Assert.NotNull(v);
            Assert.Equal(7, v.Id);
            Assert.Equal("van", v.Type);
            Assert.Equal(60000, v.Odometer);
            Assert.Equal(55000, v.LastBrakeKm);
            Assert.Equal(new DateTime(2025, 6, 15), v.LastInspDate);
        }

        [Theory]
        [InlineData("bad;line")]
        [InlineData(null)]
        [InlineData("x;AA;car;1;2;2026-01-01")]
        [InlineData("1;AA;car;1;2;not-a-date")]
        public void Parse_InvalidLine_ReturnsNull(string line)
        {
            Assert.Null(new FleetImporter().Parse(line));
        }

        [Fact]
        public void UpdateOdometer_UnknownPlate_ReturnsFalse()
        {
            var repo = new FleetRepository();

            Assert.False(repo.UpdateOdometer("NONE", 100));
        }

        [Fact]
        public void UpdateOdometer_Increase_IsAllowed()
        {
            var repo = new FleetRepository();
            repo.Add(new Vehicle { Plate = "AA1", Odometer = 5000 });

            Assert.True(repo.UpdateOdometer("AA1", 6000));
            Assert.Equal(6000, repo.FindByPlate("AA1").Odometer);
        }

        // Характеризаційний: зменшення пробігу дозволене, TD-04.
        [Fact]
        public void UpdateOdometer_Rollback_IsAllowed()
        {
            var repo = new FleetRepository();
            repo.Add(new Vehicle { Plate = "AA1", Odometer = 5000 });

            Assert.True(repo.UpdateOdometer("AA1", 100));
            Assert.Equal(100, repo.FindByPlate("AA1").Odometer);
        }

        [Fact]
        public void AverageOdometer_EmptyFleet_ReturnsZero()
        {
            var stats = new FleetStatistics();

            var empty = new List<Vehicle>();

            Assert.Equal(0, stats.AverageOdometer(empty));
        }
    }
}
