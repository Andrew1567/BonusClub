namespace FleetMaintenance.Core.Domain;

/// <summary>Стан заявки на обслуговування.</summary>
public enum MaintenanceStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
}
