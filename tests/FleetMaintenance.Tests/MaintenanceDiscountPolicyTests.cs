using System;
using FleetMaintenance.Core.Jobs;
using Xunit;

namespace FleetMaintenance.Tests;

public class MaintenanceDiscountPolicyTests
{
    [Fact]
    public void DiscountFor_NegativeAmount_ThrowsOutOfRange()
    {
        var policy = new MaintenanceDiscountPolicy();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => policy.DiscountFor(-0.01m, true));
    }

    [Fact]
    public void DiscountFor_ZeroAmountNewClient_ReturnsZero()
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor(0m, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_RegularClientSmallOrder_Returns5Percent()
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor(400m, true);

        Assert.Equal(20m, actual);
    }

    [Fact]
    public void DiscountFor_JustBelowBigOrder_ReturnsZero()
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor(999.99m, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_ExactBigOrderNewClient_Returns10Percent()
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor(1000m, false);

        Assert.Equal(100m, actual);
    }

    [Fact]
    public void DiscountFor_ExactBigOrderRegularClient_Returns15Percent()
    {
        var policy = new MaintenanceDiscountPolicy();

        decimal actual = policy.DiscountFor(1000m, true);

        Assert.Equal(150m, actual);
    }
}