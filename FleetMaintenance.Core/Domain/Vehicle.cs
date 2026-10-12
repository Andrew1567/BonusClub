namespace FleetMaintenance.Core.Domain;

/// <summary>Транспортний засіб автопарку.</summary>
public sealed class Vehicle
{
    public string LicensePlate { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public int Mileage { get; init; }
}
