using System;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;

namespace FleetMaintenance.Core.Pricing;

public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > 0.5m)
            throw new ArgumentOutOfRangeException(nameof(rate));
        _rate = rate;
    }

    public decimal PriceOf(WorkLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}