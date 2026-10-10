using System;
using System.Collections.Generic;

namespace FleetMaintenance.LegacyModule;

public class FleetCustomer
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;
}

public class ServiceLine
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyMaintenanceProcessor
{
    private const decimal VipDiscountRate = 0.15m;
    private const decimal StaffDiscountRate = 0.30m;
    private const decimal LoyalDiscountRate = 0.05m;
    private const decimal MaxDiscountLimit = 500m;
    private const decimal FreeShippingThreshold = 1000m;
    private const decimal StandardShippingCost = 60m;

    private readonly List<string> _log = new();

    public string Handle(int docId, FleetCustomer customer, 
        List<ServiceLine>? items, string state, 
        string currency, bool sendMail)
    {
        if (items == null) return "ERR: null";
        if (items.Count == 0) return "ERR: empty";
        if (state != "new" && state != "paid") return "ERR: state";
        if (customer.Email == null || !customer.Email.Contains("@")) return "ERR: mail";

        _log.Add("ok " + docId);

        // РЕФАКТОРИНГ (Дефект 5): Використовуємо виділені методи
        decimal tmpSum = CalculateSubtotal(items);
        decimal tmpDiscount = CalculateDiscount(customer.Kind, customer.DoneCount, tmpSum);
        decimal ship = CalculateShipping(tmpSum, tmpDiscount);
        decimal total = tmpSum - tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + customer.Name + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            txt += items[i].Code + " x" + items[i].Qty + " = " +
                   (items[i].Qty * items[i].Price).ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + tmpDiscount.ToString("0.00") + " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") + " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") + " " + currency + "\n";

        if (sendMail)
        {
            _log.Add("mail -> " + customer.Email);
        }
        return txt;
    }

    public decimal Preview(string clientKind, int clientDone, List<ServiceLine> items)
    {
        // РЕФАКТОРИНГ (Дефект 5): Використовуємо виділені методи
        decimal s = CalculateSubtotal(items);
        decimal d = CalculateDiscount(clientKind, clientDone, s);
        decimal sh = CalculateShipping(s, d);
        return s - d + sh;
    }

    // --- НОВІ ПРИВАТНІ МЕТОДИ (Extract Method) ---

    private decimal CalculateSubtotal(List<ServiceLine> items)
    {
        decimal sum = 0m;
        foreach (var it in items)
        {
            sum += it.Qty * it.Price;
        }
        return sum;
    }

    private decimal CalculateDiscount(string kind, int doneCount, decimal subtotal)
    {
        decimal discount = 0m;
        if (kind == "vip")
        {
            discount = subtotal * VipDiscountRate;
        }
        else if (kind == "staff")
        {
            discount = subtotal * StaffDiscountRate;
        }
        else if (doneCount > 10)
        {
            discount = subtotal * LoyalDiscountRate;
        }

        if (discount > MaxDiscountLimit) discount = MaxDiscountLimit;
        return discount;
    }

    private decimal CalculateShipping(decimal subtotal, decimal discount)
    {
        if (subtotal - discount < FreeShippingThreshold) return StandardShippingCost;
        return 0m;
    }

    // ----------------------------------------------

    public string DescribeClient(FleetCustomer c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == "vip") s += " [VIP]";
        if (c.DoneCount > 10) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}