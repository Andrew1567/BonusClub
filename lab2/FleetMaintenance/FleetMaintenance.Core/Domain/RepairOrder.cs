using System;
using System.Collections.Generic;

namespace FleetMaintenance.Domain;

/// <summary>
/// Заявка на ремонт та обслуговування автопарку.
/// </summary>
public class RepairOrder
{
    private const decimal VatRate = 0.2m;
    private const decimal RegularDiscountThreshold = 1000m;
    private const decimal RegularDiscountAmount = 100m;
    private const decimal LargeOrderThreshold = 5000m;
    private const decimal LargeOrderDiscountRate = 0.85m;
    private const int BulkLineCount = 10;
    private const decimal BulkDiscountAmount = 100m;
    private const int MaxLineCount = 100;
    private const int MinCustomerNameLength = 2;

    public const int StatusNew = 0;
    public const int StatusPaid = 1;
    public const int StatusSent = 2;
    public const int StatusCancelled = 3;

    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Ініціалізує нову заявку на ремонт.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор заявки.</param>
    /// <param name="customerName">Ім'я клієнта.</param>
    public RepairOrder(string id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Отримує ідентифікатор заявки.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Отримує ім'я клієнта.
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Отримує поточний статус заявки.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Отримує дату та час створення заявки.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Додає до заявки рядок послуги.
    /// </summary>
    /// <param name="serviceCode">Код послуги.</param>
    /// <param name="quantity">Кількість одиниць послуги.</param>
    /// <param name="unitPrice">Ціна за одиницю, грн.</param>
    public void AddLine(string serviceCode, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = serviceCode;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює загальну суму заявки з урахуванням знижок та ПДВ.
    /// </summary>
    /// <param name="isRegularCustomer">Чи має клієнт статус постійного.</param>
    /// <returns>Загальна вартість ремонту.</returns>
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total += quantity * unitPrice;
            lineCount++;
        }

        // Знижки не сумуються: діє лише одна з двох
        if (isRegularCustomer && total > RegularDiscountThreshold)
        {
            total -= RegularDiscountAmount;
        }
        else if (total > LargeOrderThreshold)
        {
            total *= LargeOrderDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан заявки, якщо перехід дозволений правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан заявки.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusPaid)
        {
            Status = StatusPaid;
            return true;
        }

        if (Status == StatusPaid && newStatus == StatusSent)
        {
            Status = StatusSent;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи є заявка валідною для подальшої обробки.
    /// </summary>
    /// <returns>true, якщо заявка коректна; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && CustomerName != null && CustomerName.Length > MinCustomerNameLength
            && _lines.Count > 0 && _lines.Count < MaxLineCount
            && Status >= StatusNew && Status <= StatusCancelled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий звіт по заявці.
    /// </summary>
    /// <returns>Рядок із переліком послуг та підсумковою сумою.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            report += "Послуга: " + _lines[i][0] + "; кількість: " + _lines[i][1]
                      + "; ціна: " + _lines[i][2] + "; сума: "
                      + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report += "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }

    /// <summary>
    /// Шукає заявку за ідентифікатором у наданому списку.
    /// </summary>
    /// <param name="orders">Список заявок для пошуку.</param>
    /// <param name="orderId">Ідентифікатор шуканої заявки.</param>
    /// <returns>Знайдена заявка або null, якщо не знайдено.</returns>
    public static RepairOrder FindById(List<RepairOrder> orders, string orderId)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == orderId)
            {
                return orders[i];
            }
        }

        return null;
    }
}