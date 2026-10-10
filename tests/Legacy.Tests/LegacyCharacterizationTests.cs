using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using FleetMaintenance.LegacyModule; // <--- Ось цей рядок бачить ServiceLine!

namespace FleetMaintenance.LegacyModule.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<ServiceLine> TwoLines() => new()
    {
        new ServiceLine { Code = "OIL-1", Qty = 2, Price = 150m },
        new ServiceLine { Code = "BRK-2", Qty = 1, Price = 400m }
    };

    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, string state, string mail, string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<ServiceLine>(),
            _ => null
        };
        var sut = new LegacyMaintenanceProcessor();
        var actual = sut.Handle(1001, 7, "Іван", mail, "regular", 0,
            items, state, "UAH", new DateTime(2026, 3, 1), false);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyMaintenanceProcessor();
        var report = sut.Handle(1001, 7, "Іван", "a@b.c",
            "vip", 3, TwoLines(), "new", "UAH",
            new DateTime(2026, 3, 1), false);

        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "OIL-1 x2 = 300.00 UAH\n" +
            "BRK-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";

        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int done, decimal expected)
    {
        var sut = new LegacyMaintenanceProcessor();
        var total = sut.Preview(kind, done, TwoLines());
        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_FreeShippingBoundary_ReturnsExactTotal()
    {
        var items = new List<ServiceLine> { new ServiceLine { Code = "TEST", Qty = 10, Price = 100m } };
        var sut = new LegacyMaintenanceProcessor();
        var total = sut.Preview("regular", 0, items);
        Assert.Equal(1000m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new FleetCustomer
        {
            Name = " Іван ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };
        var years = DateTime.Now.Year - 2020;
        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        var actual = new LegacyMaintenanceProcessor().DescribeClient(c);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DumpLog_WithLogs_ReturnsNewlineSeparatedString()
    {
        var sut = new LegacyMaintenanceProcessor();
        sut.Handle(1001, 7, "Іван", "a@b.c", "regular", 0, TwoLines(), "new", "UAH", DateTime.Now, true);
        
        var expected = "ok 1001\nmail -> a@b.c\n";
        var actual = sut.DumpLog();
        Assert.Equal(expected, actual);
    }
}