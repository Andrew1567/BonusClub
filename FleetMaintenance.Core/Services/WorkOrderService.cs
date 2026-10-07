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
    private readonly INotifier _notifier; // Нова залежність

    public WorkOrderService(
        IWorkOrderRepository repository, 
        IPricingPolicy pricing, 
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    public void Place(WorkOrder order, string recipient)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        
        _repository.Add(order);
        _notifier.Notify(recipient, $"Заявка № {order.Id} прийнята");
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