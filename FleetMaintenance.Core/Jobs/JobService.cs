using System;
using System.Collections.Generic;

namespace FleetMaintenance.Core.Jobs;

public sealed class JobService
{
    private readonly IJobRepository _repository;
    private readonly IJobPricing _pricing;

    public JobService(IJobRepository repository, IJobPricing pricing)
    {
        _repository = repository;
        _pricing = pricing;
    }

    public void Place(RepairJob job)
    {
        if (job.Lines.Count == 0)
        {
            throw new InvalidOperationException("Заявка без позицій неможлива.");
        }

        _repository.Add(job);
        _repository.Save();
    }

    public decimal TotalOf(int jobId)
    {
        RepairJob? job = _repository.GetById(jobId);
        if (job is null)
        {
            throw new KeyNotFoundException($"Заявку {jobId} не знайдено.");
        }

        decimal total = 0m;
        foreach (JobLine line in job.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return decimal.Round(total, 2);
    }
}
