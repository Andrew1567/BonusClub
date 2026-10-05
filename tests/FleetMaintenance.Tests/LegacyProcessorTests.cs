using System;
using System.Collections.Generic;
using FleetMaintenance.Core.Legacy;
using Xunit;

namespace FleetMaintenance.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
            new OrderLine { Sku = "B2", Quantity = 1, UnitPrice = 250m }
        };

        decimal total = LegacyProcessor.Process(
            1, "Іваненко", "i@ex.com",
            true, lines,
            new DateOnly(2026, 3, 10),
            string.Empty, "New", false,
            60m, out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { Sku = "A1", Quantity = 2, UnitPrice = 200m }
        };

        decimal total = LegacyProcessor.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", "Paid",
            false, 60m, out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Shipped", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3, "Коваль", "k@ex.com", false,
            new List<OrderLine>(),
            new DateOnly(2026, 3, 10), string.Empty, "New", false,
            60m, out string next);

        Assert.Equal(-1m, total);
    }
}