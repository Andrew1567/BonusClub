using System.Globalization;
using FleetMaintenance.Core.Domain;
using FleetMaintenance.Core.Errors;

namespace FleetMaintenance.App;

public static class WorkLineParser
{
    private static readonly CultureInfo Ukrainian =
        CultureInfo.GetCultureInfo("uk-UA");

    public static Result<WorkLine> Parse(
        string? code, string? qty, string? price)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<WorkLine>.Fail(
                "Код послуги не може бути порожнім");
        }

        if (!int.TryParse(qty, out var q) || q <= 0)
        {
            return Result<WorkLine>.Fail(
                "Кількість — ціле число більше 0");
        }

        if (!decimal.TryParse(price, NumberStyles.Number,
                Ukrainian, out var p) || p < 0)
        {
            return Result<WorkLine>.Fail(
                "Ціна — число не менше 0, дроби через кому");
        }

        return Result<WorkLine>.Ok(new WorkLine(code, q, p));
    }
}
