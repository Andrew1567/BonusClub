namespace FleetMaintenance.Core.Domain;

/// <summary>Рядок заявки: опис послуги, години та ставка.</summary>
public sealed class ServiceLine
{
    public string Description { get; init; } = string.Empty;

    public decimal Hours { get; init; }

    public decimal HourlyRate { get; init; }

    public decimal Amount => Hours * HourlyRate;
}
