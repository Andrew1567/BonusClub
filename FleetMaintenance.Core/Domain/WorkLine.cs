namespace FleetMaintenance.Core.Domain;

public sealed record WorkLine
{
    public WorkLine(string serviceCode, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(serviceCode))
        {
            throw new ArgumentException(
                "Порожній код послуги", nameof(serviceCode));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (unitPrice < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        }

        ServiceCode = serviceCode;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string ServiceCode { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    // Обчислюване значення, а не поле: розсинхронізувати нічого
    public decimal Amount => UnitPrice * Quantity;
}
