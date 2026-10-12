using System;
using System.Collections.Generic;
using FleetMaintenance.Core.Legacy;

namespace FleetMaintenance.Core.Documents;

public sealed record ClientInfo(string Name, string Email, bool IsRegular);

public sealed record TotalRequest(
    int DocumentId,
    ClientInfo Client,
    IReadOnlyList<OrderLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    DocumentStatus Status,
    decimal DeliveryPrice);

public sealed record TotalResult(decimal Total, DocumentStatus Status);
