namespace FleetMaintenance.Core.Domain;

public sealed record FleetClient
{
    public FleetClient(int id, string name, string email)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Порожнє ім’я", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }

    // Необов’язкова ознака задається лише в ініціалізаторі
    public bool IsRegular { get; init; }
}
