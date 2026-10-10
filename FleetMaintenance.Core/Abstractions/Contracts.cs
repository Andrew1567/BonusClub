using System.Collections.Generic;
using FleetMaintenance.Core.Domain;

namespace FleetMaintenance.Core.Abstractions;

public interface IWorkOrderRepository
{
    void Add(WorkOrder order);
    WorkOrder? GetById(int id);
    IReadOnlyList<WorkOrder> GetAll();
}

public interface IPricingPolicy
{
    decimal PriceOf(WorkLine line);
}