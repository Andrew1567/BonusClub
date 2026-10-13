using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FleetMaintenance.Tests
{
    public class PlannerTests
    {
        private static readonly DateTime Now =
            new DateTime(2026, 3, 1);
        private readonly MaintenancePlanner _sut =
            new MaintenancePlanner();

        private static Vehicle Fresh(
            string type = "car", int odo = 50000)
        {
            return new Vehicle
            {
                Id = 1, Plate = "AA1", Type = type, Odometer = odo,
                LastOilKm = odo, LastOilDate = Now,
                LastBrakeKm = odo, LastBrakeDate = Now,
                LastInspKm = odo, LastInspDate = Now
            };
        }

        private static string OilState(List<ServiceTask> tasks)
        {
            var oil = tasks.FirstOrDefault(t => t.Kind == "oil");
            return oil == null ? "NONE" : oil.State;
        }

        [Fact]
        public void Plan_FreshVehicle_ReturnsEmpty()
        {
            var actual = _sut.Plan(Fresh(), 1, false, Now);

            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(8999, "NONE")]
        [InlineData(9000, "DUE_SOON")]
        [InlineData(10000, "DUE")]
        [InlineData(10999, "DUE")]
        [InlineData(11000, "OVERDUE")]
        [InlineData(11001, "OVERDUE")]
        public void Plan_CarOilByKm_ReturnsState(
            int kmSince, string expected)
        {
            var v = Fresh();
            v.LastOilKm = v.Odometer - kmSince;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal(expected, OilState(actual));
        }

        [Theory]
        [InlineData(10, "NONE")]
        [InlineData(11, "DUE_SOON")]
        [InlineData(12, "DUE")]
        [InlineData(13, "DUE")]
        [InlineData(14, "OVERDUE")]
        public void Plan_CarOilByMonths_ReturnsState(
            int months, string expected)
        {
            var v = Fresh();
            v.LastOilDate = Now.AddMonths(-months);

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal(expected, OilState(actual));
        }

        // Характеризаційний: 55000 км = 110 % від 50000, але
        // 50000 * 1.1 у double = 55000.00000000001: DUE, не OVERDUE.
        // Див. TD-01.
        [Fact]
        public void Plan_TruckBrake55000Km_ReturnsDueNotOverdue()
        {
            var v = Fresh("truck", 100000);
            v.LastBrakeKm = v.Odometer - 55000;

            var actual = _sut.Plan(v, 1, false, Now);

            var brake = actual.Single(t => t.Kind == "brake");
            Assert.Equal("DUE", brake.State);
        }

        [Fact]
        public void Plan_VanOilByKm_UsesVanInterval()
        {
            var v = Fresh("van");
            v.LastOilKm = v.Odometer - 15000;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal("DUE", OilState(actual));
        }

        [Fact]
        public void Plan_BrakeAndInspNearLimit_ReturnDueSoon()
        {
            var v = Fresh();
            v.LastBrakeKm = v.Odometer - 27000;
            v.LastInspKm = v.Odometer - 18000;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal(new[] { "DUE_SOON", "DUE_SOON" },
                actual.Select(t => t.State).ToArray());
        }

        [Fact]
        public void Plan_Verbose_WritesLogLines()
        {
            var v = Fresh();
            v.LastOilKm = v.Odometer - 12000;
            var original = Console.Out;
            var writer = new System.IO.StringWriter();
            Console.SetOut(writer);
            try
            {
                _sut.Plan(v, 1, true, Now);
            }
            finally
            {
                Console.SetOut(original);
            }

            var text = writer.ToString();
            Assert.Contains("AA1: oil OVERDUE", text);
            Assert.Contains("!!! overdue km=12000", text);
        }

        [Fact]
        public void Plan_TruckOilSixMonths_ReturnsDue()
        {
            var v = Fresh("truck");
            v.LastOilDate = Now.AddMonths(-6);

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal("DUE", OilState(actual));
        }

        [Fact]
        public void Plan_UnknownType_UsesCarIntervals()
        {
            var v = Fresh("bus");
            v.LastOilKm = v.Odometer - 10000;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal("DUE", OilState(actual));
        }

        [Fact]
        public void Plan_NullVehicle_ReturnsEmpty()
        {
            var actual = _sut.Plan(null, 1, false, Now);

            Assert.Empty(actual);
        }

        [Fact]
        public void Plan_InactiveVehicle_ReturnsEmpty()
        {
            var v = Fresh();
            v.Active = false;
            v.LastOilKm = 0;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Empty(actual);
        }

        [Fact]
        public void Plan_OverdueOil_CostHasSurchargeAndFleetDiscount()
        {
            var v = Fresh();
            v.LastOilKm = v.Odometer - 12000;

            var small = _sut.Plan(v, 1, false, Now).Single();
            var large = _sut.Plan(v, 11, false, Now).Single();

            Assert.Equal(92.00m, small.Cost);
            Assert.Equal(87.40m, large.Cost);
        }

        [Fact]
        public void Plan_AllKindsOverdue_ReturnsOilBrakeInspInOrder()
        {
            var v = Fresh();
            v.LastOilKm = 0;
            v.LastBrakeKm = 0;
            v.LastInspKm = 0;

            var actual = _sut.Plan(v, 1, false, Now);

            Assert.Equal(new[] { "oil", "brake", "insp" },
                actual.Select(t => t.Kind).ToArray());
        }

        // Характеризаційний: IsAnyDue не перевіряє техогляд (insp),
        // а Plan перевіряє. Поведінку зафіксовано, див. TD-02.
        [Fact]
        public void IsAnyDue_OnlyInspectionOverdue_ReturnsFalse()
        {
            var v = Fresh();
            v.LastInspKm = 0;

            Assert.False(_sut.IsAnyDue(v, Now));
        }

        [Fact]
        public void IsAnyDue_OilDue_ReturnsTrue()
        {
            var v = Fresh();
            v.LastOilKm = v.Odometer - 10000;

            Assert.True(_sut.IsAnyDue(v, Now));
        }

        [Fact]
        public void IsAnyDue_VanBrakeDue_ReturnsTrue()
        {
            var v = Fresh("van");
            v.LastBrakeKm = v.Odometer - 40000;

            Assert.True(_sut.IsAnyDue(v, Now));
        }

        [Fact]
        public void IsAnyDue_NullVehicle_ThrowsNullReference()
        {
            Assert.Throws<NullReferenceException>(
                () => _sut.IsAnyDue(null, Now));
        }

        [Fact]
        public void NextDueKm_UnknownKind_ReturnsMinusOne()
        {
            Assert.Equal(-1, _sut.NextDueKm(Fresh(), "tires"));
        }

        [Theory]
        [InlineData("van", "brake", 10000, 50000)]
        [InlineData("car", "oil", 5000, 15000)]
        [InlineData("van", "oil", 0, 15000)]
        [InlineData("truck", "insp", 0, 30000)]
        public void NextDueKm_KnownKind_AddsStepOfType(
            string type, string kind, int last, int expected)
        {
            var v = Fresh(type);
            v.LastOilKm = last;
            v.LastBrakeKm = last;
            v.LastInspKm = last;

            Assert.Equal(expected, _sut.NextDueKm(v, kind));
        }
    }
}
