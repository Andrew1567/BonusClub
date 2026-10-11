using System;
using System.Collections.Generic;
using System.IO;
using FleetMaintenance.Core.Documents;
using FleetMaintenance.Core.Legacy;
using FleetMaintenance.Core.Reporting;
using Xunit;

namespace FleetMaintenance.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        TotalRequest request = new(
            DocumentId: 1,
            Client: new ClientInfo("Іваненко", "i@ex.com", true),
            Lines: new List<OrderLine>
            {
                new OrderLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
                new OrderLine { Sku = "B2", Quantity = 1, UnitPrice = 250m }
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: string.Empty,
            Status: DocumentStatus.New,
            DeliveryPrice: 60m);

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(DocumentStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_SmallOrder_AddsDelivery()
    {
        TotalRequest request = new(
            DocumentId: 2,
            Client: new ClientInfo("Петренко", "p@ex.com", false),
            Lines: new List<OrderLine>
            {
                new OrderLine { Sku = "A1", Quantity = 2, UnitPrice = 200m }
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: DocumentStatus.Paid,
            DeliveryPrice: 60m);

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(DocumentStatus.Shipped, result.Status);
    }

    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            DocumentGuards.EnsureLinesValid(new List<OrderLine>()));
    }

    [Fact]
    public void Calculate_ZeroQuantity_ThrowsRange()
    {
        OrderLine line = new()
        {
            Sku = "A1",
            Quantity = 0,
            UnitPrice = 10m
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentGuards.EnsureLineValid(line));
    }
    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = new(
            DocumentId: 1,
            Client: new ClientInfo("Іваненко", "i@ex.com", true),
            Lines: new List<OrderLine>
            {
                new OrderLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
                new OrderLine { Sku = "B2", Quantity = 1, UnitPrice = 250m }
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: string.Empty,
            Status: DocumentStatus.New,
            DeliveryPrice: 60m);

        TotalResult result = DocumentTotalCalculator.Calculate(request);
        string text = DocumentReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        DocumentReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }
}
