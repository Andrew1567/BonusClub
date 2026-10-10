using System;
using System.Collections.Generic;

namespace FleetMaintenance.Core.Legacy;

// Заглушки для компіляції старого коду
public class FileOrderStore { public void SaveToFile(LegacyWorkOrder o, string p) {} }
public class SmtpMailer { public void Send(string to, string subj, string body) {} }

public class WorkOrderManager
{
    private readonly FileOrderStore _store = new FileOrderStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyWorkOrder order, string customerType)
    {
        decimal total = 0m;
        foreach (var line in order.Lines)
            total += line.UnitPrice * line.Quantity;

        if (customerType == "regular")
            total = total * 0.95m;
        else if (customerType == "fleet")
            total = total * 0.90m;
        else if (customerType == "staff")
            total = total * 0.70m;

        if (total > 1000m)
            total = total - 50m;

        order.Total = total;
        order.Status = 1;

        _store.SaveToFile(order, "orders.json");
        _mailer.Send(order.Email, "Order " + order.Id, "Total: " + total);

        return total;
    }

    public string BuildCsvReport(List<LegacyWorkOrder> orders)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in orders)
            text += o.Id + ";" + o.Total + ";" + o.Status;
        
        System.Diagnostics.Debug.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyWorkOrder order)
    {
        LegacyWorkOrder archived = new ArchivedWorkOrder(order.Id);
        foreach (var line in order.Lines)
            archived.AddLine(line);

        _store.SaveToFile(archived, "archive.json");
    }
}