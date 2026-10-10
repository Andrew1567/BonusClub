using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FleetMaintenance.LegacyModule;

public class FleetCustomer
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;

    // РЕФАКТОРИНГ (Дефект 8): Перемістили метод сюди (Move Method)
    public string Describe()
    {
        var sb = new StringBuilder();
        sb.Append(Name.Trim().ToUpper());
        
        if (Kind == "vip") sb.Append(" [VIP]");
        if (DoneCount > 10) sb.Append(" [ЛОЯЛЬНИЙ]");
        
        sb.Append($" <{Email.ToLower()}>");
        
        int years = DateTime.Now.Year - SinceUtc.Year;
        sb.Append($" стаж {years}");
        
        return sb.ToString();
    }
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

        decimal subtotal = CalculateSubtotal(items);
        decimal discount = CalculateDiscount(customer.Kind, customer.DoneCount, subtotal);
        decimal shipping = CalculateShipping(subtotal, discount);
        decimal total = subtotal - discount + shipping;

        var sb = new StringBuilder();
        sb.Append($"Документ #{docId}\n");
        sb.Append($"Клієнт: {customer.Name}\n");
        
        foreach (var item in items)
        {
            sb.Append($"{item.Code} x{item.Qty} = {(item.Qty * item.Price).ToString("0.00")} {currency}\n");
        }
        
        sb.Append($"Знижка: {discount.ToString("0.00")} {currency}\n");
        sb.Append($"Доставка: {shipping.ToString("0.00")} {currency}\n");
        sb.Append($"Разом: {total.ToString("0.00")} {currency}\n");

        if (sendMail)
        {
            _log.Add("mail -> " + customer.Email);
        }
        
        return sb.ToString();
    }

    public decimal Preview(string clientKind, int clientDone, List<ServiceLine> items)
    {
        decimal subtotal = CalculateSubtotal(items);
        decimal discount = CalculateDiscount(clientKind, clientDone, subtotal);
        decimal shipping = CalculateShipping(subtotal, discount);
        return subtotal - discount + shipping;
    }

    private decimal CalculateSubtotal(List<ServiceLine> items)
    {
        return items.Sum(item => item.Qty * item.Price);
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

    public string DumpLog()
    {
        var sb = new StringBuilder();
        foreach (var l in _log) 
        {
            sb.Append(l).Append('\n');
        }
        return sb.ToString();
    }
}