using System;
using System.IO;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;
using FleetMaintenance.Core.Errors;
using Microsoft.Extensions.Logging;

namespace FleetMaintenance.Core.Services;

public sealed class WorkOrderService
{
    private const decimal BonusFrom = 1000m;
    private const decimal BonusAmount = 50m;

    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<WorkOrderService> _log;

    public WorkOrderService(
        IWorkOrderRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<WorkOrderService> log)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _log = log ?? throw new ArgumentNullException(nameof(log));
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
        {
            total += _pricing.PriceOf(line);
        }

        return total > BonusFrom ? total - BonusAmount : total;
    }

    public void AddLine(int orderId, WorkLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        var order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Заявку {orderId} не знайдено");

        if (order.Status != WorkOrderStatus.Draft)
        {
            throw new DomainRuleException("order.editable", $"Стан {order.Status} не дозволяє додавати позиції");
        }

        order.AddLine(line);
    }

    public void Complete(int orderId, decimal amount)
    {
        using var scope = _log.BeginScope("Order:{OrderId}", orderId);
        _log.LogDebug("Початок завершення, сплачено {Amount}", amount);

        var order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Заявку {orderId} не знайдено");

        if (order.Status != WorkOrderStatus.InProgress)
        {
            throw new DomainRuleException("order.completable", $"Стан {order.Status} не дозволяє завершення");
        }

        decimal total = TotalOf(order);
        if (amount < total)
        {
            throw new DomainRuleException("order.amount", $"Сплачено {amount:0.00}, треба {total:0.00}");
        }

        if (amount > total)
        {
            _log.LogWarning("Переплата {Extra}", amount - total);
        }

        order.Complete();
        _log.LogInformation("Заявку завершено на {Total}", total);
    }

    // ВИПРАВЛЕНО: АНТИПАТЕРН 1 (Проковтнутий catch)
    public WorkOrder Load(int orderId)
    {
        return _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Заявку {orderId} не знайдено");
    }

    // ВИПРАВЛЕНО: АНТИПАТЕРНИ 2 і 3 (catch Exception та throw ex)
    public void Cancel(int orderId, string reason)
    {
        try
        {
            var order = _repository.GetById(orderId)
                ?? throw new DomainRuleException("order.exists", $"Заявку {orderId} не знайдено");
            order.Cancel();
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Збій сховища, {OrderId}", orderId);
            throw; // стек збережено
        }
    }
}
