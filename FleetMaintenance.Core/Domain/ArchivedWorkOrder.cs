using System;
using System.Collections.Generic;

namespace FleetMaintenance.Core.Domain;

// Архівна заявка НЕ є заявкою: вона лише показує її дані
public sealed class ArchivedWorkOrder
{
    private readonly WorkOrder _order;

    public ArchivedWorkOrder(WorkOrder order, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(order);
        _order = order;
        ArchivedAt = archivedAt;
    }

    public DateOnly ArchivedAt { get; }
    public int Id => _order.Id;
    public WorkOrderStatus Status => _order.Status;
    public IReadOnlyList<WorkLine> Lines => _order.Lines;

    // Методу AddLine немає взагалі: порушити контракт нічим
}