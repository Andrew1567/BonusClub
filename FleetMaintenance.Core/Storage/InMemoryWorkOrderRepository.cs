using System;
using System.Collections.Generic;
using System.Linq;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;

namespace FleetMaintenance.Core.Storage;

public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly Dictionary<int, WorkOrder> _items = new();

    public void Add(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_items.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Заявка {order.Id} уже існує");
        }
    }

    public WorkOrder? GetById(int id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<WorkOrder> GetAll() => _items.Values.ToList();
}
