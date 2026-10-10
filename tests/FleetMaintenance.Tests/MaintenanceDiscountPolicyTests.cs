using System;
using FleetMaintenance.Core.Jobs;
using Xunit;

namespace FleetMaintenance.Tests;

public class MaintenanceDiscountPolicyTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    public void DiscountFor_Boundaries_MatchesTable(
        double amount, bool isRegular, double expected)
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor((decimal)amount, isRegular);

        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void DiscountFor_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        var policy = new MaintenanceDiscountPolicy();

        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => policy.DiscountFor((decimal)amount, true));

        Assert.Equal("amount", ex.ParamName);
    }
    [Fact]
    public void DiscountFor_HugeOrder_IsCappedAtMax()
    {
        var policy = new MaintenanceDiscountPolicy();

        // Замовлення на 5000 грн для постійного клієнта дало б 750 грн знижки,
        // але ми очікуємо, що спрацює стеля і знижка буде лише 150 грн.
        decimal actual = policy.DiscountFor(5000m, true);

        Assert.Equal(150m, actual);
    }
}