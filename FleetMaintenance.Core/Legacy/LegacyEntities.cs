using System;
using System.Collections.Generic;

namespace FleetMaintenance.Core.Legacy;

public class LegacyWorkLine
{
    public string ServiceCode { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class LegacyWorkOrder
{
    public int Id { get; set; }
    public int Status { get; set; }
    public decimal Total { get; set; }
    public string Email { get; set; } = "";
    public List<LegacyWorkLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyWorkLine line) => Lines.Add(line);
}

public class ArchivedWorkOrder : LegacyWorkOrder
{
    public ArchivedWorkOrder(int id) => Id = id;

    public override void AddLine(LegacyWorkLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}