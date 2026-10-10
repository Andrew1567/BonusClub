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
    // РЕФАКТОРИНГ (Дефект 4): Заміна магічних чисел на зрозумілі константи
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

        decimal tmpSum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            tmpSum += items[i].Qty * items[i].Price;
        }

        decimal tmpDiscount = 0m;
        if (customer.Kind == "vip")
        {
            tmpDiscount = tmpSum * VipDiscountRate;
            if (tmpDiscount > MaxDiscountLimit) tmpDiscount = MaxDiscountLimit;
        }
        else if (customer.Kind == "staff")
        {
            tmpDiscount = tmpSum * StaffDiscountRate;
            if (tmpDiscount > MaxDiscountLimit) tmpDiscount = MaxDiscountLimit;
        }
        else if (customer.DoneCount > 10)
        {
            tmpDiscount = tmpSum * LoyalDiscountRate;
            if (tmpDiscount > MaxDiscountLimit) tmpDiscount = MaxDiscountLimit;
        }

        decimal ship = 0m;
        if (tmpSum - tmpDiscount < FreeShippingThreshold) ship = StandardShippingCost;

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
        decimal s = 0m;
        foreach (var it in items)
        {
            s += it.Qty * it.Price;
        }

        decimal d = 0m;
        if (clientKind == "vip")
        {
            d = s * VipDiscountRate;
            if (d > MaxDiscountLimit) d = MaxDiscountLimit;
        }
        else if (clientKind == "staff")
        {
            d = s * StaffDiscountRate;
            if (d > MaxDiscountLimit) d = MaxDiscountLimit;
        }
        else if (clientDone > 10)
        {
            d = s * LoyalDiscountRate;
            if (d > MaxDiscountLimit) d = MaxDiscountLimit;
        }

        decimal sh = 0m;
        if (s - d < FreeShippingThreshold) sh = StandardShippingCost;
        return s - d + sh;
    }

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