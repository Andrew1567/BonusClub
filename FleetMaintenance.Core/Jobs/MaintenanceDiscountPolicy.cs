using System;

namespace FleetMaintenance.Core.Jobs;

public sealed class MaintenanceDiscountPolicy
{
    // Знижка постійному клієнтові
    public const decimal RegularRate = 0.05m;
    // Додаткова знижка за велику заявку
    public const decimal BigOrderRate = 0.10m;
    // Межа великої заявки, грн
    public const decimal BigOrderFrom = 1000m;
    // Максимальна сума знижки, грн
    public const decimal MaxDiscount = 150m;

    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        EnsureAmountIsValid(amount);
        decimal raw = amount * RateFor(amount, isRegular);
        return Math.Min(decimal.Round(raw, 2), MaxDiscount);
    }

    private static void EnsureAmountIsValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");
        }
    }

    private static decimal RateFor(decimal amount, bool isRegular)
    {
        decimal rate = isRegular ? RegularRate : 0m;
        if (amount >= BigOrderFrom)
        {
            rate += BigOrderRate;
        }

        return rate;
    }
}
