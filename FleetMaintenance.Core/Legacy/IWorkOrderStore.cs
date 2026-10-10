namespace FleetMaintenance.Core.Legacy;

public interface IWorkOrderStore
{
    void Add(LegacyWorkOrder order);
    LegacyWorkOrder GetById(int id);
    void SaveToFile(LegacyWorkOrder order, string path);
    void SendEmail(string to, string body);
    string ExportCsv();
    void Backup(string folder);
    void PrintInvoice(LegacyWorkOrder order);
}