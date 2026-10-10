using System;
using System.Globalization;
using System.Text;
using FleetMaintenance.Core.Documents;
using FleetMaintenance.Core.Legacy;

namespace FleetMaintenance.Core.Reporting;

public static class DocumentReport
{
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine($"Документ № {request.DocumentId}");
        text.AppendLine($"Клієнт: {request.Client.Name}");
        foreach (OrderLine line in request.Lines)
        {
            text.AppendLine($"{line.Sku} x {line.Quantity}");
        }
        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine($"Стан: {result.Status}");
        return text.ToString();
    }
}
