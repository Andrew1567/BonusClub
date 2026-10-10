using System;
using System.Text;
using FleetMaintenance.App;
using FleetMaintenance.Core;
using FleetMaintenance.Core.Abstractions;
using FleetMaintenance.Core.Domain;
using FleetMaintenance.Core.Errors;
using FleetMaintenance.Core.Pricing;
using FleetMaintenance.Core.Services;
using FleetMaintenance.Core.Storage;
using Microsoft.Extensions.Logging;

// Добавляем вывод версии программы:
Console.WriteLine($"FleetMaintenance v{VersionInfo.Current()}");

using var factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

var log = factory.CreateLogger<Program>();

try
{
    using var audit = new FileAuditLog("audit.log");
    audit.Write("AppStarted", 0);
    Console.OutputEncoding = Encoding.UTF8;

    var repository = new InMemoryWorkOrderRepository();
    IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
    var notifier = new ConsoleNotifier();
    var serviceLogger = factory.CreateLogger<WorkOrderService>();

    var service = new WorkOrderService(repository, pricing, notifier, serviceLogger);

    var order = new WorkOrder(1, 100, DateOnly.FromDateTime(DateTime.Today));
    repository.Add(order);

    while (true)
    {
        Console.Write("Код послуги: ");
        string? code = Console.ReadLine();
        Console.Write("Кількість: ");
        string? qty = Console.ReadLine();
        Console.Write("Ціна: ");
        string? price = Console.ReadLine();

        var parsed = WorkLineParser.Parse(code, qty, price);
        if (!parsed.IsSuccess)
        {
            Console.WriteLine($"Помилка вводу: {parsed.Error}");
            continue;
        }

        service.AddLine(1, parsed.Value!);
        break;
    }

    // Штучно викликаємо помилку бізнес-правила (заявка ще Draft, а ми робимо Complete),
    // щоб побачити Warning у глобальному обробнику
    service.Complete(1, 1000m);
    return 0;
}
catch (DomainRuleException ex)
{
    log.LogWarning("Правило {Rule}: {Message}", ex.Rule, ex.Message);
    return 1;
}
catch (Exception ex)
{
    log.LogCritical(ex, "Непередбачений збій");
    return 2;
}

class ConsoleNotifier : INotifier
{
    public void Notify(string recipient, string message)
    {
        Console.WriteLine($"[Лист для {recipient}]: {message}");
    }
}
