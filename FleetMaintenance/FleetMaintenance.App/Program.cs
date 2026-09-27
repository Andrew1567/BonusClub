using System.Globalization;
using FleetMaintenance.Core.Domain;

MaintenanceRequest request = new()
{
    Id = 1,
    VehicleLicensePlate = "KA1234AB",
    CreatedAt = DateTimeOffset.Now,
};

request.AddLine(new ServiceLine
{
    Description = "Заміна мастила",
    Hours = 1.5m,
    HourlyRate = 500.00m,
});

request.AddLine(new ServiceLine
{
    Description = "Діагностика ходової",
    Hours = 1.0m,
    HourlyRate = 400.00m,
});

string total = request.Total().ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Заявка #{request.Id}");
Console.WriteLine($"Стан: {request.Status}");
Console.WriteLine($"Позицій: {request.Lines.Count}");
Console.WriteLine($"Сума: {total}");
