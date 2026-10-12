using System.Collections.Generic;

namespace FleetMaintenance.Core.Jobs;

public sealed record JobLine(string Code, int Quantity, decimal UnitPrice);

public sealed record RepairJob(int Id, int VehicleId)
{
    public List<JobLine> Lines { get; } = new();
}

public interface IJobRepository
{
    void Add(RepairJob job);
    RepairJob? GetById(int id);
    void Save();
}

public interface IJobPricing
{
    decimal PriceOf(JobLine line);
}
