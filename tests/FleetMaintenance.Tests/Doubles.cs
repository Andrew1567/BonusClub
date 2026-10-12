using System.Collections.Generic;
using FleetMaintenance.Core.Jobs;

namespace FleetMaintenance.Tests;

internal sealed class FakeJobRepository : IJobRepository
{
    private readonly Dictionary<int, RepairJob> _items = new();

    public int SaveCalls { get; private set; }

    public void Add(RepairJob job) => _items[job.Id] = job;

    public RepairJob? GetById(int id) =>
        _items.TryGetValue(id, out RepairJob? found) ? found : null;

    public void Save() => SaveCalls++;
}

internal sealed class StubJobPricing : IJobPricing
{
    private readonly decimal _price;

    public StubJobPricing(decimal price) => _price = price;

    public decimal PriceOf(JobLine line) => _price;
}
