namespace FleetMaintenance.Core.Domain;

public enum WorkOrderStatus
{
    Draft, Scheduled, InProgress, Completed, Cancelled
}

public sealed class WorkOrder
{
    private readonly List<WorkLine> _lines = new();

    public WorkOrder(int id, int clientId, DateOnly createdAt)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));

        Id = id;
        ClientId = clientId;
        CreatedAt = createdAt;
        Status = WorkOrderStatus.Draft;
    }

    public int Id { get; }
    public int ClientId { get; }
    public DateOnly CreatedAt { get; }
    public WorkOrderStatus Status { get; private set; }

    // Клієнт бачить позиції, але додати їх повз AddLine не може
    public IReadOnlyList<WorkLine> Lines => _lines;

    public void AddLine(WorkLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException(
                "Позиції додають лише до чернетки заявки");
        _lines.Add(line);
    }

    public void Schedule()
    {
        Require(WorkOrderStatus.Draft);
        if (_lines.Count == 0)
            throw new InvalidOperationException("Заявка порожня");
        Status = WorkOrderStatus.Scheduled;
    }

    public void Start()
    {
        Require(WorkOrderStatus.Scheduled);
        Status = WorkOrderStatus.InProgress;
    }

    public void Complete()
    {
        Require(WorkOrderStatus.InProgress);
        Status = WorkOrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status is not (WorkOrderStatus.Draft
                           or WorkOrderStatus.Scheduled))
            throw new InvalidOperationException(
                $"Скасування зі стану {Status} заборонено");
        Status = WorkOrderStatus.Cancelled;
    }

    private void Require(WorkOrderStatus expected)
    {
        if (Status != expected)
            throw new InvalidOperationException(
                $"Перехід зі стану {Status} заборонено");
    }
}