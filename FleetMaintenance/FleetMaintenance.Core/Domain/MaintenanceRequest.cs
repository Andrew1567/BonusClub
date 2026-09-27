namespace FleetMaintenance.Core.Domain;

/// <summary>Заявка на обслуговування автопарку.</summary>
public sealed class MaintenanceRequest
{
    private readonly List<ServiceLine> _lines = new();

    public int Id { get; init; }

    public string VehicleLicensePlate { get; init; } = string.Empty;

    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<ServiceLine> Lines => _lines;

    /// <summary>Додає виконану роботу до заявки.</summary>
    public void AddLine(ServiceLine line) => _lines.Add(line);

    /// <summary>Обчислює загальну вартість робіт.</summary>
    /// <returns></returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (ServiceLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
