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

    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");

        decimal rate = 0m;
        if (isRegular)
            rate += RegularRate;
        if (amount >= BigOrderFrom)
            rate += BigOrderRate;

        return decimal.Round(amount * rate, 2);
    }
}