namespace FleetMaintenance.Core.Abstractions;

public interface INotifier
{
    void Notify(string recipient, string message);
}
