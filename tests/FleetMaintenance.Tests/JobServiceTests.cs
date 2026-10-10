using System;
using System.Collections.Generic;
using FleetMaintenance.Core.Jobs;
using Xunit;

namespace FleetMaintenance.Tests;

public class JobServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        var repository = new FakeJobRepository();
        var job = new RepairJob(7, 1);
        job.Lines.Add(new JobLine("OIL-1", 2, 10m));
        job.Lines.Add(new JobLine("BRK-2", 1, 30m));
        repository.Add(job);
        var service = new JobService(repository, new StubJobPricing(25m));

        decimal actual = service.TotalOf(7);

        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_JobWithLines_StoresAndSavesOnce()
    {
        var repository = new FakeJobRepository();
        var service = new JobService(repository, new StubJobPricing(1m));
        var job = new RepairJob(1, 5);
        job.Lines.Add(new JobLine("OIL-1", 1, 5m));

        service.Place(job);

        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public void TotalOf_UnknownId_ThrowsKeyNotFound()
    {
        var service = new JobService(new FakeJobRepository(), new StubJobPricing(1m));

        Assert.Throws<KeyNotFoundException>(() => service.TotalOf(404));
    }
    [Fact]
    public void Place_JobWithoutLines_ThrowsInvalidOperation()
    {
        var service = new JobService(
            new FakeJobRepository(), new StubJobPricing(1m));
        var job = new RepairJob(2, 5);

        Assert.Throws<InvalidOperationException>(
            () => service.Place(job));
    }
}