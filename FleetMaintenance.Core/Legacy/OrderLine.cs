namespace FleetMaintenance.Core.Legacy;

public sealed class OrderLine
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}