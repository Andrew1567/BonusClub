using System;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;

namespace FleetMaintenance.Core.Services;

public sealed class WorkOrderService
{
    private const decimal BonusFrom = 1000m;
    private const decimal BonusAmount = 50m;

    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;

    public WorkOrderService(
        IWorkOrderRepository repository, IPricingPolicy pricing)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing
            ?? throw new ArgumentNullException(nameof(pricing));
    }

    public void Place(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
    }

    public decimal TotalOf(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        decimal total = 0m;
        foreach (WorkLine line in order.Lines)
            total += _pricing.PriceOf(line);
        return total > BonusFrom ? total - BonusAmount : total;
    }
}