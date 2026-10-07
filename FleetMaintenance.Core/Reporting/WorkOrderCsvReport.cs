using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FleetMaintenance.Core.Domain;

namespace FleetMaintenance.Core.Reporting;

public sealed class WorkOrderCsvReport
{
    public string BuildCsv(
        IReadOnlyList<WorkOrder> orders,
        Func<WorkOrder, decimal> totalOf)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(totalOf);

        StringBuilder text = new();
        text.AppendLine("id;total;status");
        foreach (WorkOrder o in orders)
        {
            text.AppendLine(string.Create(CultureInfo.InvariantCulture,
                $"{o.Id};{totalOf(o):F2};{o.Status}"));
        }
        return text.ToString();
    }
}