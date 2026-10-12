using System.IO;
using FleetMaintenance.Core.Domain;
using FleetMaintenance.Core.Services;

namespace FleetMaintenance.App;

public class ReportExporter
{
    private readonly WorkOrderService _service;

    public ReportExporter(WorkOrderService service)
    {
        _service = service;
    }

    // СТАЛО: using звільняє потік навіть при винятку
    public void ExportReport(string path, WorkOrder order)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine($"Заявка № {order.Id}");
        writer.WriteLine(_service.TotalOf(order));
    }
}
