using System;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;
using FleetMaintenance.Core.Pricing;
using FleetMaintenance.Core.Services;
using FleetMaintenance.Core.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FleetMaintenance.Tests;

// Заглушка-сповіщувач, яка нічого не робить, щоб тести працювали швидко і без консолі
public class NullNotifier : INotifier
{
    public void Notify(string recipient, string message) { }
}

public class DomainInvariantTests
{
    [Fact]
    public void WorkLine_WithZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WorkLine("OIL-CHANGE", 0, 10m));
    }

    [Fact]
    public void WorkOrder_AfterSchedule_RejectsNewLines()
    {
        var order = NewOrder(1);
        order.AddLine(new WorkLine("OIL-CHANGE", 1, 10m));
        order.Schedule();

        Assert.Throws<InvalidOperationException>(
            () => order.AddLine(new WorkLine("BRAKE-PADS", 1, 5m)));
    }

    [Fact]
public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        var service = new WorkOrderService(
            new InMemoryWorkOrderRepository(),
            new DiscountPricingPolicy(0.10m),
            new NullNotifier(),
            NullLogger<WorkOrderService>.Instance); // <-- Додали тільки цей рядок
        
        var order = NewOrder(2);
        order.AddLine(new WorkLine("OIL-CHANGE", 2, 100m));

        Assert.Equal(180m, service.TotalOf(order));
    }

    private static WorkOrder NewOrder(int id) =>
        new WorkOrder(id, 100, DateOnly.FromDateTime(DateTime.Today));
}