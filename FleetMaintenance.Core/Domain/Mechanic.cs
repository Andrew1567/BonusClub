namespace FleetMaintenance.Core.Domain;

/// <summary>Механік, який виконує роботи.</summary>
public sealed class Mechanic
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;
}
